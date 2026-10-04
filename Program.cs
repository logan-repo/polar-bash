using System.Runtime.InteropServices;

namespace JinTerm;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        bool ownsConsole = false;
        if (GetConsoleWindow() == IntPtr.Zero)
        {
            ownsConsole = AllocConsole();
            if (ownsConsole) ShowWindow(GetConsoleWindow(), 0);
        }
        try { Application.Run(new TerminalForm()); }
        finally { if (ownsConsole) FreeConsole(); }
    }

    [DllImport("kernel32.dll")] private static extern bool AllocConsole();
    [DllImport("kernel32.dll")] private static extern bool FreeConsole();
    [DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr window, int command);
}
