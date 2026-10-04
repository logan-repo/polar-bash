using Microsoft.Win32.SafeHandles;
using System.Text;
using MiniTerm;
using MiniTerm.Native;

namespace JinTerm;

// The ConPTY setup uses Microsoft's MiniTerm sample. This class only adapts its pipes
// to the app's UTF-8 message stream.
internal sealed class ConPtySession : IDisposable
{
    private readonly PseudoConsolePipe inputPipe;
    private readonly PseudoConsolePipe outputPipe;
    private readonly PseudoConsole pseudoConsole;
    private readonly MiniTerm.Process child;
    private readonly StreamWriter writer;
    private readonly FileStream output;
    private readonly object inputGate = new();
    private bool disposed;
    private bool started;

    public event Action<string>? Output;
    public event Action? Exited;

    public ConPtySession(string shellPath, string workingDirectory, short cols = 100, short rows = 30,
        string arguments = "--login -i")
    {
        inputPipe = new PseudoConsolePipe();
        outputPipe = new PseudoConsolePipe();
        pseudoConsole = PseudoConsole.Create(inputPipe.ReadSide, outputPipe.WriteSide, cols, rows);
        string command = arguments.Length == 0 ? $"\"{shellPath}\"" : $"\"{shellPath}\" {arguments}";
        child = ProcessFactory.Start(command, PseudoConsole.PseudoConsoleThreadAttribute,
            pseudoConsole.Handle, workingDirectory);
        writer = new StreamWriter(new FileStream(inputPipe.WriteSide, FileAccess.Write),
            new UTF8Encoding(false)) { AutoFlush = true };
        output = new FileStream(outputPipe.ReadSide, FileAccess.Read);
    }

    public void Start()
    {
        if (started) return;
        started = true;
        _ = Task.Run(ReadOutput);
    }

    public Task WriteAsync(string value)
    {
        if (disposed) return Task.CompletedTask;
        lock (inputGate)
        {
            try { writer.Write(value); }
            catch (IOException) { }
            catch (ObjectDisposedException) { }
        }
        return Task.CompletedTask;
    }

    public void Resize(int cols, int rows)
    {
        if (disposed || cols < 1 || rows < 1) return;
        PseudoConsoleApi.ResizePseudoConsole(pseudoConsole.Handle,
            new PseudoConsoleApi.COORD
            {
                X = (short)Math.Min(cols, short.MaxValue),
                Y = (short)Math.Min(rows, short.MaxValue)
            });
    }

    private void ReadOutput()
    {
        using var reader = new StreamReader(output, new UTF8Encoding(false, false), false, 4096, true);
        char[] buffer = new char[4096];
        try
        {
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) > 0)
                Output?.Invoke(new string(buffer, 0, count));
        }
        catch (IOException) { }
        catch (ObjectDisposedException) { }
        finally { Exited?.Invoke(); }
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        child.Dispose();
        pseudoConsole.Dispose();
        try { writer.Dispose(); } catch (IOException) { }
        try { output.Dispose(); } catch (IOException) { }
        inputPipe.Dispose();
        outputPipe.Dispose();
    }
}
