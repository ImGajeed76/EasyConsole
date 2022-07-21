namespace ImGajeedsEasyConsole.Components
{
    public class Options
    {
        public string[] options;

        public Options(string[] options)
        {
            this.options = options;
        }

        public int Length()
        {
            return options.Length;
        }
    }
}