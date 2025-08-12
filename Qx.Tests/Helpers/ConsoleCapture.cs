using System.Text;

namespace Qx.Tests.Helpers;

/// <summary>
/// Helper class to capture console output during tests
/// </summary>
internal sealed class ConsoleCapture : IDisposable
{
    private readonly TextWriter _originalOut;
    private readonly TextWriter _originalError;
    private readonly StringWriter _capturedOut;
    private readonly StringWriter _capturedError;
    private bool _disposed;

    public ConsoleCapture()
    {
        _originalOut = Console.Out;
        _originalError = Console.Error;
        _capturedOut = new StringWriter();
        _capturedError = new StringWriter();
        
        Console.SetOut(_capturedOut);
        Console.SetError(_capturedError);
    }

    /// <summary>
    /// Get the captured standard output
    /// </summary>
    public string GetOutput() => _capturedOut.ToString();

    /// <summary>
    /// Get the captured error output
    /// </summary>
    public string GetError() => _capturedError.ToString();

    /// <summary>
    /// Clear captured output
    /// </summary>
    public void Clear()
    {
        _capturedOut.GetStringBuilder().Clear();
        _capturedError.GetStringBuilder().Clear();
    }

    /// <summary>
    /// Create a capture and execute an action, returning the captured output
    /// </summary>
    public static (string output, string error) Capture(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        using var capture = new ConsoleCapture();
        action();
        return (capture.GetOutput(), capture.GetError());
    }

    /// <summary>
    /// Create a capture and execute an async action, returning the captured output
    /// </summary>
    public static async Task<(string output, string error)> CaptureAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        
        using var capture = new ConsoleCapture();
        await action().ConfigureAwait(false);
        return (capture.GetOutput(), capture.GetError());
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Console.SetOut(_originalOut);
            Console.SetError(_originalError);
            _capturedOut?.Dispose();
            _capturedError?.Dispose();
            _disposed = true;
        }
    }
}