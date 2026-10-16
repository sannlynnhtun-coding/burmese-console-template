# Burmese Console App (Pyidaungsu)

One project. Open Program.cs, write normal Console.WriteLine / Console.ReadLine
code, and press F5. No Viewer project or startup-project selection is needed.
The included ConsoleHost.cs supplies the inline Burmese display in the same project.
Keep the BurmeseConsoleHost.Start(args) call before your console code.

Windows, Linux and macOS require the matching .NET runtime and installed Pyidaungsu.
Avalonia 11.3.22 supplies cross-platform shaping. .NET 6–10 and 11 Preview are selectable.

    dotnet run
    dotnet run -- --self-test

Type immediately after the prompt. Enter submits the line. Backspace edits only
that input. Select text and Ctrl+C / Ctrl+V (Cmd on macOS) or right-click Copy/Paste.
There is no footer field. Copy still works after the program finishes.

This is a line-oriented console host, not a complete ANSI terminal emulator.
Console.ReadKey, cursor positioning, and full-screen TUIs are not supported.
Linux needs Avalonia's X11/fontconfig dependencies; see Avalonia platform docs.
Native Linux/macOS rendering and IME must be tested on their respective systems.

Publish framework-dependent, for example:
    dotnet publish -c Release -r linux-x64 --self-contained false
    dotnet publish -c Release -r osx-arm64 --self-contained false
Distribute the complete publish folder; run dotnet BurmeseApp.dll on that OS.

မြန်မာလို Hello World နမူနာကို Program.cs မှာ ထည့်ထားပါတယ်။
Project တစ်ခုတည်းကို F5 နှိပ်ရုံနဲ့ မြန်မာစာ console ပွင့်ပါတယ်။

Windows window shortcuts: Alt+Space opens the native system menu even while input
has focus. Release Alt, then press C to Close; Escape dismisses the menu.

Font size: Ctrl+Plus (or Ctrl+Shift+= / Ctrl+=) increases font size;
Ctrl+Minus decreases it. Ctrl+numpad Plus/Minus also work.
Size changes by 2 within 10–72, without modifying console text or the input line.
