using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Qx.Tests.Helpers;

/// <summary>
/// Base class for all test classes providing common test utilities
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "Base class for test classes")]
public abstract class TestBase : IDisposable
{
    private readonly List<IDisposable> _disposables = new();
    protected IServiceProvider Services { get; }
    protected IServiceCollection ServiceCollection { get; }

    protected TestBase()
    {
        ServiceCollection = new ServiceCollection();
        
        // Initialize services directly without calling virtual method
        var services = new ServiceCollection();
        ServiceCollection = services;
        Services = services.BuildServiceProvider();
    }

    /// <summary>
    /// Override to configure services for the test
    /// </summary>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Base implementation - override in derived classes
    }

    /// <summary>
    /// Register an object for disposal at the end of the test
    /// </summary>
    protected T RegisterForDisposal<T>(T disposable) where T : IDisposable
    {
        _disposables.Add(disposable);
        return disposable;
    }

    /// <summary>
    /// Create a temporary file that will be deleted after the test
    /// </summary>
    protected string CreateTempFile(string content = "")
    {
        string tempFile = Path.GetTempFileName();
        if (!string.IsNullOrEmpty(content))
        {
            File.WriteAllText(tempFile, content);
        }
        RegisterForDisposal(new TempFileDisposable(tempFile));
        return tempFile;
    }

    /// <summary>
    /// Create a temporary directory that will be deleted after the test
    /// </summary>
    protected string CreateTempDirectory()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        RegisterForDisposal(new TempDirectoryDisposable(tempDir));
        return tempDir;
    }

    /// <summary>
    /// Set an environment variable that will be restored after the test
    /// </summary>
    protected void SetEnvironmentVariable(string name, string? value)
    {
        string? originalValue = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
        RegisterForDisposal(new EnvironmentVariableDisposable(name, originalValue));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose in reverse order
            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i]?.Dispose();
            }
            _disposables.Clear();

            if (Services is IDisposable disposableServices)
            {
                disposableServices.Dispose();
            }
        }
    }

    private sealed class TempFileDisposable : IDisposable
    {
        private readonly string _filePath;

        public TempFileDisposable(string filePath) => _filePath = filePath;

        public void Dispose()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                }
            }
            catch (IOException)
            {
                // Best effort - ignore IO errors
            }
            catch (UnauthorizedAccessException)
            {
                // Best effort - ignore access errors
            }
        }
    }

    private sealed class TempDirectoryDisposable : IDisposable
    {
        private readonly string _directoryPath;

        public TempDirectoryDisposable(string directoryPath) => _directoryPath = directoryPath;

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_directoryPath))
                {
                    Directory.Delete(_directoryPath, recursive: true);
                }
            }
            catch (IOException)
            {
                // Best effort - ignore IO errors
            }
            catch (UnauthorizedAccessException)
            {
                // Best effort - ignore access errors
            }
        }
    }

    private sealed class EnvironmentVariableDisposable : IDisposable
    {
        private readonly string _name;
        private readonly string? _originalValue;

        public EnvironmentVariableDisposable(string name, string? originalValue)
        {
            _name = name;
            _originalValue = originalValue;
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(_name, _originalValue);
        }
    }
}