using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace ImGajeedsEasyConsole.Components
{
    public static class EConsole
    {
        public static int CursorTop()
        {
            return Console.CursorTop;
        }

        public static void CursorTop(int n)
        {
            Console.CursorTop = n;
        }

        public static int CursorLeft()
        {
            return Console.CursorLeft;
        }

        public static void CursorLeft(int value)
        {
            Console.CursorLeft = value;
        }

        public static int BufferWidth()
        {
            return Console.BufferWidth;
        }

        public static int BufferHeight()
        {
            return Console.BufferHeight;
        }

        public static void Clear()
        {
            Console.Clear();
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
            color ??= GetColor();

            Console.ForegroundColor = color.GetForeground();
            Console.BackgroundColor = color.GetBackground();
        }

        public static Color GetColor()
        {
            return new Color(Console.ForegroundColor, Console.BackgroundColor);
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void Write(object value, Color? color = null)
        {
            var lastColor = GetColor();
            SetColor(color);
            Console.Write(value);
            SetColor(lastColor);
        }

        public static void WriteLine(object value, Color? color = null)
        {
            Write(value, color);
            Console.WriteLine();
        }

        public static void BreakLine()
        {
            WriteLine("");
        }

        public static string? ReadLine()
        {
            return Console.ReadLine();
        }

        public static string? ReadLine(string value, Color? color = null)
        {
            Write(value, color);
            return ReadLine();
        }

        public static ConsoleKeyInfo ReadKey(bool intercept = false)
        {
            return Console.ReadKey(intercept: intercept);
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
            selectedColor ??= new Color(ConsoleColor.DarkYellow);

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
            selectedColor ??= new Color(ConsoleColor.DarkGreen);

            editColor ??= new Color(ConsoleColor.DarkYellow);

            var startLine = CursorTop();
            var selected = 0;

            if (values == null)
            {
                values = new string[options.Length];
                for (var i = 0; i < options.Length; i++)
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

        private static void WriteFormOptions(IReadOnlyList<string> options, IReadOnlyList<string> values, int startLine,
            int selectedLine,
            Color? color,
            Color selectedColor, IEnumerable startValues, bool editMode = false)
        {
            CursorTop(startLine);
            CursorLeft(0);

            var maxLen = 0;

            for (var i = 0; i < options.Count; i++)
            {
                WriteLine(startLine + i);
                if (options[i].Length > maxLen)
                {
                    maxLen = options[i].Length;
                }
            }

            for (var i = 0; i < options.Count; i++)
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
                ClearLine(startLine + options.Count);
                CursorTop(startLine + options.Count);
                WriteLine("<- Exit",
                    options.Count == selectedLine
                        ? new Color(ConsoleColor.DarkRed)
                        : new Color(ConsoleColor.DarkGray));
            }
            else
            {
                ClearLine(startLine + options.Count);
                CursorTop(startLine + options.Count);
                WriteLine("<- Save and Exit",
                    options.Count == selectedLine
                        ? new Color(ConsoleColor.DarkRed)
                        : new Color(ConsoleColor.DarkGray));
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
            color ??= new Color(ConsoleColor.DarkGray);

            if (showMessage)
            {
                Write(" (press any key to continue)", color);
            }

            ReadKey();
            BreakLine();
        }

        public static void AwaitEnter(bool showMessage = true, Color? color = null)
        {
            color ??= new Color(ConsoleColor.DarkGray);

            if (showMessage)
            {
                Write(" (press enter to continue)", color);
            }

            ConsoleKey key;

            do
            {
                key = ReadKey().Key;
            } while (key != ConsoleKey.Enter);

            BreakLine();
        }

        public static string? ReadEmail(string value = "", Color? color = null, Color? validColor = null,
            Color? invalidColor = null)
        {
            while (true)
            {
                color ??= GetColor();
                validColor ??= new Color(ConsoleColor.Green);
                invalidColor ??= new Color(ConsoleColor.DarkYellow);

                var email = "";
                var isValid = false;

                var currentLine = CursorTop();

                ClearLine(currentLine);
                CursorTop(currentLine);
                Write(value, color);
                Write(email, isValid ? validColor : invalidColor);

                ConsoleKey key;

                do
                {
                    var keyInfo = ReadKey(true);
                    key = keyInfo.Key;
                    var lastEmail = email;

                    if (key == ConsoleKey.Backspace)
                    {
                        if (email.Length > 0)
                        {
                            email = email.Remove(email.Length - 1);
                        }
                    }
                    else if (key != ConsoleKey.LeftArrow && key != ConsoleKey.RightArrow && key != ConsoleKey.UpArrow &&
                             key != ConsoleKey.DownArrow)
                    {
                        email += keyInfo.KeyChar;
                    }

                    email = RemoveEscapeCharacter(email);

                    try
                    {
                        var unused = new MailAddress(email);
                        isValid = true;
                    }
                    catch (Exception)
                    {
                        isValid = false;
                    }

                    if (lastEmail == email) continue;
                    ClearLine(currentLine);
                    CursorTop(currentLine);
                    CursorLeft(0);
                    Write(value, color);
                    Write(email, isValid ? validColor : invalidColor);
                } while (key != ConsoleKey.Enter);

                BreakLine();

                if (isValid) return email;
                var exit = BoolQuestion("Email is invalid. Do you want to exit?", color: color);
                if (exit)
                {
                    return null;
                }

                ClearLine(currentLine + 1);
                CursorTop(currentLine);
            }
        }
        
        public static bool IsValidEmail(string email)
        {
            try
            {
                var unused = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool OptVerification(string toEmail, string fromEmail, string appPassword,
            Color? color = null, string? customMessage = null)
        {
            if (!IsValidEmail(toEmail))
            {
                return false;
            }
            
            color ??= GetColor();

            // use ~verifyCode~ as placeholder for the verification code
            customMessage ??= "<p> This email is automatically generated. Please do not reply to this email. </p> <p> If you haven't requested this email, please ignore it. </p> <br> <p> Your verification code is: <p> <div style='background: ghostwhite; font-size: 20px; padding: 10px; border: 1px solid lightgray; margin: 10px;'>~verifyCode~</div>";

            var verificationCode = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());

            var message = new MailMessage
            (
                fromEmail,
                toEmail,
                "Verification Code",
                customMessage.Replace("~verifyCode~", verificationCode)
            );

            message.IsBodyHtml = true;

            var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, appPassword)
            };
            client.Send(message);

            var verificationCodeInput = ReadLine("Enter verification code: ", color);
            return verificationCode == verificationCodeInput;
        }
    }
}