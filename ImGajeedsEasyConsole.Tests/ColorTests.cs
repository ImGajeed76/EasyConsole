using ImGajeedsEasyConsole.Components;

namespace ImGajeedsEasyConsole.Tests;


public class ColorTests
{
    [Fact]
    public void DefaultConstructor_ShouldSetDefaultColors()
    {
        // Arrange & Act
        var color = new Color();

        // Assert
        Assert.Equal(ConsoleColor.Gray, color.GetForeground());
        Assert.Equal(ConsoleColor.Black, color.GetBackground());
    }

    [Fact]
    public void ForegroundConstructor_ShouldSetForegroundAndDefaultBackground()
    {
        // Arrange & Act
        var color = new Color(ConsoleColor.Red);

        // Assert
        Assert.Equal(ConsoleColor.Red, color.GetForeground());
        Assert.Equal(ConsoleColor.Black, color.GetBackground());
    }

    [Theory]
    [InlineData(ConsoleColor.Blue, ConsoleColor.White)]
    [InlineData(ConsoleColor.Red, ConsoleColor.Gray)]
    [InlineData(ConsoleColor.Yellow, ConsoleColor.Black)]
    public void FullConstructor_ShouldSetBothColors(ConsoleColor foreground, ConsoleColor background)
    {
        // Arrange & Act
        var color = new Color(foreground, background);

        // Assert
        Assert.Equal(foreground, color.GetForeground());
        Assert.Equal(background, color.GetBackground());
    }
}