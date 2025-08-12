using Qx.Tests.Helpers;

namespace Qx.Tests.Helpers;

public sealed class ConsoleOutputTests
{
    [Fact]
    public void ConsoleCaptureShouldCaptureStandardOutput()
    {
        // Arrange & Act
        var (output, error) = ConsoleCapture.Capture(() =>
        {
            Console.WriteLine("Test output");
            Console.Write("More output");
        });

        // Assert
        output.Should().Contain("Test output");
        output.Should().Contain("More output");
        error.Should().BeEmpty();
    }

    [Fact]
    public void ConsoleCaptureShouldCaptureErrorOutput()
    {
        // Arrange & Act
        var (output, error) = ConsoleCapture.Capture(() =>
        {
            Console.Error.WriteLine("Test error");
            Console.Error.Write("More error");
        });

        // Assert
        output.Should().BeEmpty();
        error.Should().Contain("Test error");
        error.Should().Contain("More error");
    }

    [Fact]
    public async Task ConsoleCaptureShouldCaptureAsyncOutput()
    {
        // Arrange & Act
        var (output, error) = await ConsoleCapture.CaptureAsync(async () =>
        {
            await Console.Out.WriteLineAsync("Async output").ConfigureAwait(false);
            await Console.Error.WriteLineAsync("Async error").ConfigureAwait(false);
        });

        // Assert
        output.Should().Contain("Async output");
        error.Should().Contain("Async error");
    }

    [Fact]
    public void ConsoleCaptureShouldRestoreOriginalConsoleAfterDispose()
    {
        // Arrange
        var originalOut = Console.Out;
        var originalError = Console.Error;

        // Act
        using (var capture = new ConsoleCapture())
        {
            Console.Out.Should().NotBeSameAs(originalOut);
            Console.Error.Should().NotBeSameAs(originalError);
        }

        // Assert
        Console.Out.Should().BeSameAs(originalOut);
        Console.Error.Should().BeSameAs(originalError);
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Test literal")]
    public void ConsoleCaptureClearShouldResetCapturedContent()
    {
        // Arrange
        using var capture = new ConsoleCapture();
        
        // Act
        Console.WriteLine("First output");
        capture.GetOutput().Should().Contain("First output");
        
        capture.Clear();
        capture.GetOutput().Should().BeEmpty();
        
        Console.WriteLine("Second output");
        
        // Assert
        capture.GetOutput().Should().Contain("Second output");
        capture.GetOutput().Should().NotContain("First output");
    }
}