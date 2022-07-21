using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace ImGajeedsEasyConsole.Components
{
    public class Console
    {
        public static int CursorTop()
        {
            return System.Console.CursorTop;
        }

        public static void CursorTop(int n)
        {
            System.Console.CursorTop = n;
        }

        public static int CursorLeft()
        {
            return System.Console.CursorLeft;
        }

        public static void CursorLeft(int value)
        {
            System.Console.CursorLeft = value;
        }

        public static int BufferWidth()
        {
            return System.Console.BufferWidth;
        }

        public static int BufferHeight()
        {
            return System.Console.BufferHeight;
        }

        public static void Clear()
        {
            System.Console.Clear();
        }

        public static void ClearLine(int line)
        {
            var lastTop = CursorTop();
            var lastLeft = CursorLeft();
            CursorTop(line);
            CursorLeft(0);
            WriteLine(new string(' ', BufferWidth()));
            CursorTop(lastTop);
            CursorLeft(lastLeft);
        }

        public static void OverwriteLine(int line, string value, Color? color = null)
        {
            ClearLine(line);
            var lastTop = CursorTop();
            var lastLeft = CursorLeft();
            CursorTop(line);
            CursorLeft(0);
            WriteLine(value, color);
            CursorTop(lastTop);
            CursorLeft(lastLeft);
        }

        public static void SetColor(Color? color)
        {
            if (color == null)
            {
                color = GetColor();
            }

            System.Console.ForegroundColor = color.GetForeground();
            System.Console.BackgroundColor = color.GetBackground();
        }

        public static Color GetColor()
        {
            return new Color(System.Console.ForegroundColor, System.Console.BackgroundColor);
        }

        public static void ResetColor()
        {
            System.Console.ResetColor();
        }

        public static void Write(object value, Color? color = null)
        {
            var lastColor = GetColor();
            SetColor(color);
            System.Console.Write(value);
            SetColor(lastColor);
        }

        public static void WriteLine(object value, Color? color = null)
        {
            Write(value, color);
            System.Console.WriteLine();
        }

        public static void BreakLine()
        {
            WriteLine("");
        }

        public static string? ReadLine()
        {
            return System.Console.ReadLine();
        }

        public static string? ReadLine(string value, Color? color = null)
        {
            Write(value, color);
            return ReadLine();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return System.Console.ReadKey(intercept: intercept);
        }

        private static byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        private static byte[] HashSecureString(SecureString input, byte[] salt)
        {
            var bstr = Marshal.SecureStringToBSTR(input);
            var length = Marshal.ReadInt32(bstr, -4);
            var bytes = new byte[length];

            var bytesPin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(bstr, bytes, 0, length);
                Marshal.ZeroFreeBSTR(bstr);

                var argon2 = new Argon2id(bytes);
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8;
                argon2.Iterations = 4;
                argon2.MemorySize = 1024 * 1024;
                return argon2.GetBytes(16);
            }
            finally
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = 0;
                }

                bytesPin.Free();
            }
        }

        public static SecureString ReadPassword(Color? color = null)
        {
            var pass = new SecureString();
            ConsoleKey key;

            do
            {
                var keyInfo = ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Write("\b \b");
                    pass.RemoveAt(pass.Length - 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Write("*", color);
                    pass.AppendChar(keyInfo.KeyChar);
                }
            } while (key != ConsoleKey.Enter);

            pass.MakeReadOnly();
            BreakLine();

            return pass;
        }

        public static SecureString ReadPassword(string value, Color? color = null, Color? passColor = null)
        {
            Write(value, color);
            return ReadPassword(passColor);
        }

        public static bool BoolQuestion(string value, bool def = true, Color? color = null)
        {
            var currentLine = CursorTop();
            Write(value, color);
            var selectedOption = def;
            if (selectedOption)
            {
                Write(" (", new Color(ConsoleColor.DarkGray));
                Write("Y", new Color(ConsoleColor.Green));
                Write("/n) ", new Color(ConsoleColor.DarkGray));
            }
            else
            {
                Write(" (y/", new Color(ConsoleColor.DarkGray));
                Write("N", new Color(ConsoleColor.Red));
                Write(") ", new Color(ConsoleColor.DarkGray));
            }

            ConsoleKey key;

            do
            {
                var keyInfo = ReadKey(intercept: true);
                key = keyInfo.Key;
                var lastOption = selectedOption;

                if (key == ConsoleKey.LeftArrow)
                {
                    selectedOption = true;
                }
                else if (key == ConsoleKey.RightArrow)
                {
                    selectedOption = false;
                }

                if (lastOption != selectedOption)
                {
                    CursorTop(currentLine);
                    CursorLeft(0);

                    Write(value, color);
                    if (selectedOption)
                    {
                        Write(" (", new Color(ConsoleColor.DarkGray));
                        Write("Y", new Color(ConsoleColor.Green));
                        Write("/n) ", new Color(ConsoleColor.DarkGray));
                    }
                    else
                    {
                        Write(" (y/", new Color(ConsoleColor.DarkGray));
                        Write("N", new Color(ConsoleColor.Red));
                        Write(") ", new Color(ConsoleColor.DarkGray));
                    }
                }
            } while (key != ConsoleKey.Enter);

            BreakLine();
            return selectedOption;
        }

        public static int SelectOption(string[] options, Color? color = null, Color? selectedColor = null)
        {
            if (selectedColor == null)
            {
                selectedColor = new Color(ConsoleColor.DarkYellow);
            }

            var startLine = CursorTop();
            var selected = 0;
            ConsoleKey key;

            WriteOptions(options, color, startLine, selected, selectedColor);

            do
            {
                var keyInfo = ReadKey(intercept: true);
                key = keyInfo.Key;
                var lastSelected = selected;

                if (key == ConsoleKey.UpArrow && selected > 0)
                {
                    selected -= 1;
                }
                else if (key == ConsoleKey.DownArrow && selected < options.Length - 1)
                {
                    selected += 1;
                }

                if (lastSelected != selected)
                {
                    WriteOptions(options, color, startLine, selected, selectedColor);
                }
            } while (key != ConsoleKey.Enter);

            return selected;
        }

        private static void WriteOptions(string[] options, Color? color, int startLine, int selected,
            Color selectedColor)
        {
            CursorTop(startLine);
            CursorLeft(0);

            for (var i = 0; i < options.Length; i++)
            {
                ClearLine(startLine + i);
                CursorTop(startLine + i);
                if (i == selected)
                {
                    WriteLine("-> " + options[i], selectedColor);
                }
                else
                {
                    WriteLine("> " + options[i], color);
                }
            }
        }

        public static string[]? Form(string[] options, string[]? values = null, Color? color = null,
            Color? selectedColor = null,
            Color? editColor = null)
        {
            if (selectedColor == null)
            {
                selectedColor = new Color(ConsoleColor.DarkGreen);
            }

            if (editColor == null)
            {
                editColor = new Color(ConsoleColor.DarkYellow);
            }

            var startLine = CursorTop();
            var selected = 0;

            if (values == null)
            {
                values = new string[options.Length];
                for (int i = 0; i < options.Length; i++)
                {
                    values[i] = "";
                }
            }

            if (values.Length != options.Length)
            {
                return null;
            }

            var lastValues = new string[values.Length];

            var startValues = new string[values.Length];
            values.CopyTo(startValues, 0);

            ConsoleKey key;
            WriteFormOptions(options, values, startLine, selected, color, selectedColor, startValues);

            do
            {
                var keyInfo = ReadKey(intercept: true);
                key = keyInfo.Key;
                var lastSelected = selected;
                values.CopyTo(lastValues, 0);

                if (key == ConsoleKey.UpArrow && selected > 0)
                {
                    selected -= 1;
                }
                else if (key == ConsoleKey.DownArrow && selected < options.Length)
                {
                    selected += 1;
                }
                else if (key == ConsoleKey.Enter && selected < options.Length)
                {
                    ConsoleKey inEditKey;
                    WriteFormOptions(options, values, startLine, selected, color, editColor, startValues, true);

                    do
                    {
                        keyInfo = ReadKey(intercept: true);
                        inEditKey = keyInfo.Key;
                        values.CopyTo(lastValues, 0);
                        CursorTop(selected + startLine);
                        CursorLeft(BufferWidth() - 1);

                        if (inEditKey == ConsoleKey.Backspace)
                        {
                            if (values[selected].Length > 0)
                            {
                                values[selected] = values[selected].Remove(values[selected].Length - 1);
                            }
                        }
                        else if (
                            inEditKey != ConsoleKey.LeftArrow &&
                            inEditKey != ConsoleKey.RightArrow &&
                            inEditKey != ConsoleKey.UpArrow &&
                            inEditKey != ConsoleKey.DownArrow
                        )
                        {
                            values[selected] += keyInfo.KeyChar;
                        }

                        values[selected] = RemoveEscapeCharacter(values[selected]);

                        if (lastSelected != selected || lastValues != values)
                        {
                            WriteFormOptions(options, values, startLine, selected, color, editColor, startValues, true);
                        }
                    } while (inEditKey != ConsoleKey.Enter);

                    WriteFormOptions(options, values, startLine, selected, color, selectedColor, startValues);
                }

                if (lastSelected != selected || lastValues != values)
                {
                    WriteFormOptions(options, values, startLine, selected, color, selectedColor, startValues);
                }
            } while (key != ConsoleKey.Enter || selected != options.Length);

            BreakLine();
            return values;
        }

        private static void WriteFormOptions(string[] options, string[] values, int startLine, int selectedLine,
            Color? color,
            Color selectedColor, string[] startValues, bool editMode = false)
        {
            CursorTop(startLine);
            CursorLeft(0);

            var maxLen = 0;

            for (var i = 0; i < options.Length; i++)
            {
                WriteLine(startLine + i);
                if (options[i].Length > maxLen)
                {
                    maxLen = options[i].Length;
                }
            }

            for (var i = 0; i < options.Length; i++)
            {
                ClearLine(startLine + i);
                CursorTop(startLine + i);
                if (i == selectedLine)
                {
                    Write(options[i] + ":", selectedColor);
                    CursorLeft(maxLen + 2);
                    if (editMode)
                    {
                        Write("-", selectedColor);
                    }

                    if (values[i] == "")
                    {
                        Write("_", new Color(ConsoleColor.DarkGray));
                    }

                    WriteLine(values[i], selectedColor);
                }
                else
                {
                    Write(options[i] + ":", color);
                    CursorLeft(maxLen + 2);

                    if (values[i] == "")
                    {
                        Write("_", new Color(ConsoleColor.DarkGray));
                    }

                    WriteLine(values[i], color);
                }
            }

            if (startValues.Equals(values))
            {
                ClearLine(startLine + options.Length);
                CursorTop(startLine + options.Length);
                if (options.Length == selectedLine)
                {
                    WriteLine("<- Exit", new Color(ConsoleColor.DarkRed));
                }
                else
                {
                    WriteLine("<- Exit", new Color(ConsoleColor.DarkGray));
                }
            }
            else
            {
                ClearLine(startLine + options.Length);
                CursorTop(startLine + options.Length);
                if (options.Length == selectedLine)
                {
                    WriteLine("<- Save and Exit", new Color(ConsoleColor.DarkRed));
                }
                else
                {
                    WriteLine("<- Save and Exit", new Color(ConsoleColor.DarkGray));
                }
            }
        }

        private static string RemoveEscapeCharacter(string value)
        {
            value = value.Replace("\a", "");
            value = value.Replace("\b", "");
            value = value.Replace("\t", "");
            value = value.Replace("\r", "");
            value = value.Replace("\v", "");
            value = value.Replace("\f", "");
            value = value.Replace("\n", "");

            return value;
        }

        public static void AwaitAnyKey(bool showMessage = true, Color? color = null)
        {
            if (color == null)
            {
                color = new Color(ConsoleColor.DarkGray);
            }

            if (showMessage)
            {
                Write(" (press any key to continue)", color);
            }

            ReadKey();
            BreakLine();
        }

        public static void AwaitEnter(bool showMessage = true, Color? color = null)
        {
            if (color == null)
            {
                color = new Color(ConsoleColor.DarkGray);
            }

            if (showMessage)
            {
                Write(" (press any enter to continue)", color);
            }

            ConsoleKey key;

            do
            {
                key = ReadKey().Key;
            } while (key != ConsoleKey.Enter);

            BreakLine();
        }
    }
}