using JinTerm;
using System.Text;

string bashPath = Environment.GetEnvironmentVariable("GIT_BASH_PATH")
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Git", "bin", "bash.exe");
if (!File.Exists(bashPath))
{
    Console.Error.WriteLine($"Git Bash was not found: {bashPath}");
    return 1;
}

string workingDirectory = Path.Combine(Path.GetTempPath(), "polar-cmd-bridge-check-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(workingDirectory);
try
{
    var output = new StringBuilder();
    using var shell = new ConPtySession(bashPath, workingDirectory);
    shell.Output += text => { lock (output) output.Append(text); };
    shell.Start();
    await Task.Delay(400);

    await shell.WriteAsync("\u001b[200~printf 'BRIDGE_FIRST_OK\\n'\nprintf 'BRIDGE_SECOND_OK\\n'\u001b[201~\r");
    if (!await WaitForAsync(() => Snapshot().Contains("BRIDGE_SECOND_OK"), TimeSpan.FromSeconds(5)))
        return Fail("Multiline paste did not finish.");

    await File.WriteAllTextAsync(Path.Combine(workingDirectory, "bridge-unique-file.txt"), "BRIDGE_TAB_OK");
    await shell.WriteAsync("cat bridge-uni\t\r");
    if (!await WaitForAsync(() => Snapshot().Contains("BRIDGE_TAB_OK"), TimeSpan.FromSeconds(5)))
        return Fail("Tab completion did not finish.");

    Console.WriteLine("PASS: multiline paste and Tab completion");
    return 0;

    string Snapshot() { lock (output) return output.ToString(); }
}
catch (Exception error)
{
    return Fail($"{error.GetType().Name}: {error.Message}");
}
finally
{
    Directory.Delete(workingDirectory, recursive: true);
}

static async Task<bool> WaitForAsync(Func<bool> condition, TimeSpan timeout)
{
    var deadline = DateTime.UtcNow + timeout;
    while (DateTime.UtcNow < deadline)
    {
        if (condition()) return true;
        await Task.Delay(50);
    }
    return condition();
}

static int Fail(string message)
{
    Console.Error.WriteLine("FAIL: " + message);
    return 1;
}
