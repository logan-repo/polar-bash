using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.Json;

namespace PolarBash;

internal sealed class TerminalForm : Form
{
    private readonly WebView2 browser = new() { Dock = DockStyle.Fill };
    private readonly string workingDirectory;
    private readonly List<string> pendingMessages = new();
    private ConPtySession? session;
    private bool webReady;
    private int firstOutputLogged;

    public TerminalForm(string workingDirectory)
    {
        this.workingDirectory = workingDirectory;
        Text = "Polar bash";
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Width = 1120;
        Height = 720;
        MinimumSize = new Size(600, 400);
        BackColor = Color.FromArgb(13, 17, 23);
        Controls.Add(browser);
        Shown += (_, _) =>
        {
            Log("window shown");
            StartShell();
            _ = InitializeAsync();
        };
        FormClosing += (_, _) => session?.Dispose();
    }

    private async Task InitializeAsync()
    {
        try
        {
            Log("initializing webview");
            string webData = Path.Combine(AppContext.BaseDirectory, "webview-data");
            Directory.CreateDirectory(webData);
            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: webData);
            await browser.EnsureCoreWebView2Async(environment);
            Log("webview created");
            browser.CoreWebView2.Settings.AreDevToolsEnabled = false;
            browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            browser.CoreWebView2.WebMessageReceived += OnMessage;
            browser.CoreWebView2.SetVirtualHostNameToFolderMapping("polarbash.local",
                Path.Combine(AppContext.BaseDirectory, "wwwroot"),
                CoreWebView2HostResourceAccessKind.DenyCors);
            browser.CoreWebView2.NavigationCompleted += async (_, args) =>
            {
                if (!args.IsSuccess)
                {
                    Log("navigation failed: " + args.WebErrorStatus);
                    session?.Dispose();
                    ShowStartupError("터미널 화면을 불러오지 못했습니다: " + args.WebErrorStatus);
                    return;
                }
                Log("navigation completed");
                webReady = true;
                foreach (string message in pendingMessages)
                    browser.CoreWebView2.PostWebMessageAsJson(message);
                pendingMessages.Clear();
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("POLARBASH_DIAGNOSTICS")))
                {
                    try
                    {
                        await Task.Delay(300);
                        Log("browser status: " + await browser.ExecuteScriptAsync(
                            "document.getElementById('status')?.textContent || 'missing'"));
                    }
                    catch (Exception error) { Log("browser diagnostic error: " + error.Message); }
                }
            };
            browser.CoreWebView2.Navigate("https://polarbash.local/index.html");
        }
        catch (Exception error)
        {
            Log("webview error: " + error);
            session?.Dispose();
            ShowStartupError("터미널 화면을 시작하지 못했습니다: " + error.Message);
        }
    }

    private void StartShell()
    {
        Log("starting shell");
        Log("working directory: " + workingDirectory);
        string? shell = FindGitBash();
        if (shell is null)
        {
            Post(new { type = "fatal", message = "Git for Windows가 설치되어 있지 않습니다. gitforwindows.org에서 설치한 뒤 다시 실행하세요." });
            return;
        }

        try
        {
            session = new ConPtySession(shell, workingDirectory);
            Log("shell process created");
            session.Output += data =>
            {
                if (Interlocked.Exchange(ref firstOutputLogged, 1) == 0)
                    Log("first shell output");
                if (IsHandleCreated && !IsDisposed)
                    try { BeginInvoke(() => Post(new { type = "output", data })); }
                    catch (InvalidOperationException) { }
            };
            session.Exited += () =>
            {
                Log("shell exited");
                if (IsHandleCreated && !IsDisposed)
                    try { BeginInvoke(() => Post(new { type = "exited" })); }
                    catch (InvalidOperationException) { }
            };
            session.Start();
            Post(new { type = "ready", shell, workingDirectory });
            _ = session.WriteAsync("bind 'set enable-bracketed-paste on'\r");
        }
        catch (Exception error)
        {
            Log("shell error: " + error);
            Post(new { type = "fatal", message = error.Message });
        }
    }

    private static string? FindGitBash()
    {
        var roots = new List<string>
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Git"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Git"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Git")
        };
        foreach (RegistryHive hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
        {
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using var key = RegistryKey.OpenBaseKey(hive, view).OpenSubKey(@"Software\GitForWindows");
                    if (key?.GetValue("InstallPath") is string path && !string.IsNullOrWhiteSpace(path))
                        roots.Add(path);
                }
                catch (System.Security.SecurityException) { }
                catch (UnauthorizedAccessException) { }
                catch (IOException) { }
            }
        }
        foreach (string root in roots)
        {
            string bash = Path.Combine(root, "bin", "bash.exe");
            if (File.Exists(bash)) return bash;
        }
        return null;
    }

    private static void Log(string message)
    {
        string? path = Environment.GetEnvironmentVariable("POLARBASH_DIAGNOSTICS");
        if (string.IsNullOrWhiteSpace(path)) return;
        try { File.AppendAllText(path, DateTime.Now.ToString("O") + " " + message + Environment.NewLine); }
        catch (IOException) { }
    }

    private void ShowStartupError(string message)
    {
        var notice = new Label
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(13, 17, 23),
            ForeColor = Color.FromArgb(230, 237, 245),
            Font = new Font("Malgun Gothic", 12),
            TextAlign = ContentAlignment.MiddleCenter,
            Text = message
        };
        Controls.Add(notice);
        notice.BringToFront();
    }

    private void OnMessage(object? sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        try
        {
            using var doc = JsonDocument.Parse(args.WebMessageAsJson);
            var root = doc.RootElement;
            string type = root.GetProperty("type").GetString() ?? "";
            switch (type)
            {
                case "input":
                    _ = session?.WriteAsync(root.GetProperty("data").GetString() ?? "");
                    break;
                case "run":
                    string script = Normalize(root.GetProperty("data").GetString() ?? "");
                    if (script.Length > 0)
                        _ = session?.WriteAsync("\u001b[200~" + script.TrimEnd('\n') + "\u001b[201~\r");
                    break;
                case "resize":
                    session?.Resize(root.GetProperty("cols").GetInt32(), root.GetProperty("rows").GetInt32());
                    break;
                case "clipboard-read":
                    Post(new { type = "clipboard", data = Clipboard.ContainsText() ? Clipboard.GetText() : "" });
                    break;
                case "clipboard-write":
                    string selected = root.GetProperty("data").GetString() ?? "";
                    if (selected.Length > 0) Clipboard.SetText(selected);
                    break;
                case "open-folder":
                    var explorer = new ProcessStartInfo("explorer.exe") { UseShellExecute = true };
                    explorer.ArgumentList.Add(workingDirectory);
                    Process.Start(explorer);
                    break;
            }
        }
        catch (Exception error)
        {
            Post(new { type = "error", message = error.Message });
        }
    }

    private static string Normalize(string value) => value.Replace("\r\n", "\n").Replace('\r', '\n');

    private void Post(object message)
    {
        if (IsDisposed) return;
        string json = JsonSerializer.Serialize(message);
        if (webReady && browser.CoreWebView2 is not null)
            browser.CoreWebView2.PostWebMessageAsJson(json);
        else
            pendingMessages.Add(json);
    }
}
