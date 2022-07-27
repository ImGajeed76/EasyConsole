# EasyConsole

### Project on NuGet:
[ImGajeedsEasyConsole](https://www.nuget.org/packages/ImGajeedsEasyConsole)

### ToDo:

```
- implement argon2 hashing for SecureString (Does not work yet)
- implement asciit text
```

## Examples:

#### "Hello World" in color:

```csharp
Console.WriteLine("Hello World", new Color(ConsoleColor.Green));
```

#### Password input:

```csharp
var password = Console.ReadPassword("Password: ");
```

#### Yes / No Question:

```csharp
var answer = Console.BoolQuestion("Would you like to continue?");
Console.WriteLine(answer);
```

#### Object Selection:

```csharp
var options = new Options(new[]
{
    "Option 1",
    "Option 2",
    "Option 3"
});
var index = Console.SelectOption(options);
Console.WriteLine(index);
```

#### Form:

```csharp
var lines = new[]
{
    "Firstname",
    "Lastname"
};
var values = Console.Form(lines);
     
Console.WriteLine(values[0]);
Console.WriteLine(values[1]);
```

#### Set Console Color:

```csharp
Console.SetColor(new Color(ConsoleColor.Blue));
Console.WriteLine("This text should be blue");
Console.WriteLine("And this one too");
```

## Documentation:

#### Color:

In the Color class you can define a fore- and or background color.  
(Pretty self explaining)

```csharp
var color = new Color(ConsoleColor.DarkBlue, ConsoleColor.DarkGray);
```

Note: You can use this Color in pretty much every other Console function.

#### Write:

With the Write function you can write text to the console and color it. This command will not make a new line in the
console!

```csharp
Console.Write("Text", color);
```

#### WriteLine:

This function is exactly the same as Write, except it does make a new line after it.

```csharp
Console.Write("Text with new line", color)
```

#### ReadLine:

This function is like the one in System.Console. But here you can ask for something too.

```csharp
var answer = Console.ReadLine();

//or

var answer = Console.ReadLine("Username: ", color);
```

#### ReadKey:

This one is completly like the one in System.Console.

```csharp
var key = Console.ReadKey();
```

#### ReadPassword:

With this function you can ask for a Password that is returned as SecureString.

```csharp
var pass = Console.ReadPassword();

//or

var pass = Console.ReadPassword("Password: ", color, passColor);
```

#### Yes / No Question (BoolQuestion):

With this function you can ask a yes/no question with a default value.  
(yes = true, no = false)

```csharp
var answer = Console.BoolQuestion("Would you like to continue?", def, color);
```

#### SelectOption:

With this function you can select from a string[] of options. Returned will be the index of the array. To select a
option, move with the arrow keys and press enter.

```csharp
var options = new[]
{
    "Option 1",
    "Option 2",
    "Option 3"
};
var index = Console.SelectOption(options);
Console.WriteLine(index);
```

#### Form:

With this function you can create a Form where users can input variables for multiple lines. To edit a field, select it
with your arrow keys and and press enter. Now you can write.

```csharp
var lines = new[]
{
    "Firstname",
    "Lastname"
};
var values = Console.Form(lines);
     
Console.WriteLine(values[0]);
Console.WriteLine(values[1]);
```

#### AwaitAnyKey:

This function waits for any key to press. You can enable or disable the message in the params.

```csharp
Console.AwaitAnyKey();
```

#### AwaitEnter:

This function is like the AwaitAnyKey function, but it only waits for the enter key.

```csharp
Console.AwaitEnter();
```

#### Clear:

With this function you can clear the whole console.

```csharp
Console.WriteLine("Hello");
Console.WriteLine("World");
Console.Clear();
```

#### ClearLine:

With this function you can clear a specific line.

```csharp
Console.WriteLine("Hello");
Console.WriteLine("World");
Console.ClearLine(0);
```

#### OverwriteLine:

With this function you can overwrite any line in the console.

```csharp
Console.WriteLine("Hello");
Console.WriteLine("World");
Console.OverwriteLine(0, "Overwritten");
```

#### ReadEmail:

Read an validate a email.

```csharp
var email = Console.ReadEmail();
```

#### IsValidEmail:

Checks if the email is plausable.

```csharp
var isValid = Console.IsValidEmail(email);
```

#### OptVerification:

Sends a email verification code to a specific email and checks it in the console. For this you need a google email (fromEmail), where you have to create a appPassword for.

```csharp
var isValid = Console.OptVerification(email, fromEmail, appPassword);
```

#### GetColor:

With this function you can get the current console color.

```csharp
var color = Console.GetColor();
```

#### SetColor:

With this function you can set the current console color.

```csharp
Console.SetColor(color);
```

#### CursorTop:

Get and set the CursorTop.

```csharp
Console.CursorTop(0);
var top = Console.CursorTop();
```

#### CursorLeft:

Like the CursorTop you can get and set its position.

```csharp
Console.CursorLeft(0);
var left = Console.CursorLeft();
```

#### BufferWidth:

Width this function you can get the BufferWidth.

```csharp
var width = Console.BufferWidth();
```

#### BufferWidth:

Width this function you can get the BufferHeight.

```csharp
var height = Console.BufferHeight();
```

## License

This project has a MIT License: [license](https://github.com/ImGajeed76/EasyConsole/blob/master/ImGajeedsEasyConsole/license.txt)

## Contact

If you have any question please write to me on discord because im not that often on github.

```
ImGajeed76#5617
```
