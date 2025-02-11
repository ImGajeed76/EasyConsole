using ImGajeedsEasyConsole.Components;

namespace ImGajeedsEasyConsole.Tests;

public class EConsoleTests
    {
        [Fact]
        public void GetColor_ShouldReturnCurrentConsoleColors()
        {
            // Arrange
            var originalForeground = Console.ForegroundColor;
            var originalBackground = Console.BackgroundColor;
            
            try
            {
                // Act
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Blue;
                var color = EConsole.GetColor();

                // Assert
                Assert.Equal(ConsoleColor.Red, color.GetForeground());
                Assert.Equal(ConsoleColor.Blue, color.GetBackground());
            }
            finally
            {
                // Cleanup
                Console.ForegroundColor = originalForeground;
                Console.BackgroundColor = originalBackground;
            }
        }

        [Fact]
        public void SetColor_WithNullColor_ShouldUseCurrentConsoleColors()
        {
            // Arrange
            var originalForeground = Console.ForegroundColor;
            var originalBackground = Console.BackgroundColor;
            
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.BackgroundColor = ConsoleColor.DarkBlue;

                // Act
                EConsole.SetColor(null);

                // Assert
                Assert.Equal(ConsoleColor.Yellow, Console.ForegroundColor);
                Assert.Equal(ConsoleColor.DarkBlue, Console.BackgroundColor);
            }
            finally
            {
                // Cleanup
                Console.ForegroundColor = originalForeground;
                Console.BackgroundColor = originalBackground;
            }
        }

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("invalid-email", false)]
        [InlineData("", false)]
        [InlineData("test@test@test.com", false)]
        [InlineData("test.com", false)]
        [InlineData("test@.com", false)]
        public void IsValidEmail_ShouldValidateEmailCorrectly(string email, bool expected)
        {
            // Act
            var result = EConsole.IsValidEmail(email);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RemoveEscapeCharacter_ShouldRemoveAllEscapeCharacters()
        {
            // Arrange
            var input = "Hello\n\t\r\v\f\aWorld\b";
            var expected = "HelloWorld";

            // Act
            var result = typeof(EConsole)
                .GetMethod("RemoveEscapeCharacter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?.Invoke(null, new object[] { input }) as string;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SelectOption_ShouldReturnValidIndex()
        {
            // Note: This test is limited since it depends on console input
            // In a real application, you might want to abstract the console input
            // to make it more testable
        }
    }