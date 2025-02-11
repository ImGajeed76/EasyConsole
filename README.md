# 🎮 EasyConsole

Make your .NET console applications beautiful and interactive with EasyConsole! This library provides an enhanced set of console UI components and utilities.

[![NuGet](https://img.shields.io/nuget/v/ImGajeedsEasyConsole.svg)](https://www.nuget.org/packages/ImGajeedsEasyConsole)

## ✨ Features

- 🎨 Colorful console output
- 🔐 Secure password input
- ✅ Interactive yes/no prompts
- 📝 Multi-field forms
- 📋 Option selection menus
- 📧 Email validation and verification
- 🎯 Cursor control and positioning

## 🚀 Getting Started

### Installation

```bash
dotnet add package ImGajeedsEasyConsole
```

### Quick Start

```csharp
using ImGajeedsEasyConsole.Components;

EConsole.WriteLine("Hello World", new Color(ConsoleColor.Green));
```

## 📚 Examples

### 🎨 Colored Text

```csharp
// Single line with color
EConsole.WriteLine("Success!", new Color(ConsoleColor.Green));

// Set color for multiple lines
EConsole.SetColor(new Color(ConsoleColor.Blue));
EConsole.WriteLine("This text is blue");
EConsole.WriteLine("This one too!");
```

### 🔐 Password Input

```csharp
var password = EConsole.ReadPassword("Enter password: ");
```

### ✅ Yes/No Questions

```csharp
var answer = EConsole.BoolQuestion("Would you like to continue?");
```

### 📋 Option Selection

```csharp
var options = new[] {
    "Start Game",
    "Settings",
    "Exit"
};
var selectedIndex = EConsole.SelectOption(options);
```

### 📝 Interactive Forms

```csharp
var fields = new[] {
    "Username",
    "Email",
    "Location"
};
var values = EConsole.Form(fields);
```

## 📖 API Reference

### Color Management 🎨

#### `Color` Class
```csharp
// Set foreground color only
var color = new Color(ConsoleColor.Blue);

// Set both foreground and background
var color = new Color(ConsoleColor.White, ConsoleColor.DarkBlue);
```

#### Color Control
```csharp
// Get current colors
var currentColor = EConsole.GetColor();

// Set new colors
EConsole.SetColor(new Color(ConsoleColor.Green));

// Reset to default
EConsole.ResetColor();
```

### Text Output 📝

#### Write Methods
```csharp
// Write without line break
EConsole.Write("Loading...", new Color(ConsoleColor.Yellow));

// Write with line break
EConsole.WriteLine("Complete!", new Color(ConsoleColor.Green));
```

### User Input ⌨️

#### Basic Input
```csharp
// Simple input
var name = EConsole.ReadLine("Enter your name: ");

// Single key input
var key = EConsole.ReadKey();
```

#### Special Input Types
```csharp
// Secure password input
var password = EConsole.ReadPassword("Password: ");

// Email input with validation
var email = EConsole.ReadEmail("Email: ");
```

### Screen Control 🖥️

#### Cursor Management
```csharp
// Get/Set cursor position
EConsole.CursorTop(0);
var left = EConsole.CursorLeft();

// Clear specific line
EConsole.ClearLine(5);

// Clear entire screen
EConsole.Clear();

// Overwrite line contents
EConsole.OverwriteLine(3, "New content");
```

### Interactive Components 🎯

#### Form Input
```csharp
var fields = new[] {"Name", "Email", "Phone"};
var values = EConsole.Form(fields);
```

#### Option Selection
```csharp
var options = new[] {"Option A", "Option B", "Option C"};
var selected = EConsole.SelectOption(options);
```

### Email Features 📧

#### Email Validation
```csharp
var isValid = EConsole.IsValidEmail("user@example.com");
```

#### OTP Verification
```csharp
var verified = EConsole.OptVerification(
    toEmail: "user@example.com",
    fromEmail: "your-app@gmail.com",
    appPassword: "your-google-app-password"
);
```

## 🤝 Contributing

Contributions are welcome! Feel free to submit issues and pull requests.

## 📄 License

This project is licensed under the MIT License - see the [license](https://github.com/ImGajeed76/EasyConsole/blob/master/ImGajeedsEasyConsole/license.txt) file for details.

## 📞 Contact

Have questions? Contact me on Discord: `@ImGajeed76`

---
Made with ❤️ by ImGajeed