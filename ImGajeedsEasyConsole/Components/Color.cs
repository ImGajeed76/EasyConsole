namespace ImGajeedsEasyConsole.Components
{
    public class Color
    {
        private ConsoleColor foreground;
        private ConsoleColor background;

        public Color()
        {
            foreground = ConsoleColor.Gray;
            background = ConsoleColor.Black;
        }

        public Color(ConsoleColor foreground)
        {
            this.foreground = foreground;
            background = ConsoleColor.Black;
        }

        public Color(ConsoleColor foreground, ConsoleColor background)
        {
            this.foreground = foreground;
            this.background = background;
        }

        public ConsoleColor GetForeground()
        {
            return foreground;
        }

        public ConsoleColor GetBackground()
        {
            return background;
        }
    }
}