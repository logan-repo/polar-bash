using System.Runtime.InteropServices;

namespace PolarBash;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        string startDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (args.Length > 0)
        {
            if (args.Length != 2 || !string.Equals(args[0], "--cwd", StringComparison.OrdinalIgnoreCase)
                || !Directory.Exists(args[1]))
            {
                MessageBox.Show("시작 폴더를 찾을 수 없습니다. 오른쪽 메뉴에서 다시 실행해 주세요.",
                    "Polar bash", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            startDirectory = Path.GetFullPath(args[1]);
        }

        // Git for Windows normally moves a login shell to HOME unless this is set.
        Environment.SetEnvironmentVariable("CHERE_INVOKING", "1");
        bool ownsConsole = false;
        if (GetConsoleWindow() == IntPtr.Zero)
        {
            ownsConsole = AllocConsole();
            if (ownsConsole) ShowWindow(GetConsoleWindow(), 0);
        }
        try { Application.Run(new TerminalForm(startDirectory)); }
        finally { if (ownsConsole) FreeConsole(); }
    }

    [DllImport("kernel32.dll")] private static extern bool AllocConsole();
    [DllImport("kernel32.dll")] private static extern bool FreeConsole();
    [DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr window, int command);
}
