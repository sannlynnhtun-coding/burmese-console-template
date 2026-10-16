# Burmese Console Template

A Visual Studio and `dotnet new` template that creates **one C# project** with
Burmese Hello World in `Program.cs`. No separate Viewer project or startup selection.

The included `ConsoleHost.cs` uses Avalonia to render Pyidaungsu and edits input
on the current prompt line. The same assembly runs the user's normal
`Console.WriteLine` / `Console.ReadLine` code through UTF-8 pipes.
This is a console-style desktop host, not a dependency-free terminal application.

## Install for Visual Studio

Requires the .NET 10 SDK to build the template package. On Windows:

```powershell
git clone https://github.com/sannlynnhtun-coding/burmese-console-template.git
cd burmese-console-template
.\scripts\Install.ps1
```

The script builds the local NuGet template package, replaces an older installation
of this template, and installs it. The install script does not publish packages or push changes.

1. Open Visual Studio → **Create a new project**.
2. Search **Burmese Console App (Pyidaungsu)**.
3. Enter your project name and location, then choose **Framework**.
4. Create the project and press **F5**.

If the template is missing, restart Visual Studio. Framework choices are .NET
6, 7, 8, 9, 10 and 11 Preview. The SDK/runtime for your chosen framework is required.
.NET 6/7 are retained for compatibility; .NET 11 requires a preview SDK.

## Command line

```powershell
dotnet new burmese-console -n MyBurmeseApp -f net10.0
cd MyBurmeseApp
dotnet run
```

Edit `Program.cs` and keep `BurmeseConsoleHost.Start(args)` before your console code.
The starter prints **မင်္ဂလာပါ ကမ္ဘာကြီး။** and reads your name on the same prompt line.

## Shortcuts

- Enter: submit the current input; Backspace: edit it without deleting prior output.
- Ctrl+C/V/A/X: copy/paste/select/cut (Cmd equivalents on macOS).
- Right-click: Copy / Paste / Select all.
- Ctrl+Plus / Ctrl+Minus: increase/decrease font size (10–72); numpad supported.
- Windows Alt+Space, release Alt, then C: close through the native system menu.

## Platform requirements

The project targets plain `netX.0` and uses Avalonia 11.3.22 for Windows/Linux/macOS.
Install **Pyidaungsu Regular** on every destination OS. Font files are not bundled.
Linux requires Avalonia's X11/fontconfig dependencies. See the generated README.
Windows input, clipboard, Backspace, zoom and native close shortcuts were tested.
Linux/macOS publish builds were checked previously; native rendering and IME on
those operating systems have not been verified in this Windows workspace.

The host supports plain text output and line input. `Console.ReadKey`, ANSI cursor
positioning and full-screen terminal programs are not supported.

## Build and uninstall

```powershell
.\scripts\Build.ps1
.\scripts\Uninstall.ps1
```

Package output: `artifacts/packages/SLH.BurmeseConsole.Templates.1.2.3.nupkg`.
On any OS you can instead run `dotnet pack -o artifacts/packages`, followed by
`dotnet new install <package-path>`.

## Verify a generated app

```powershell
dotnet run -- --self-test
```

The app tests inline Burmese input, protected prompt, Backspace, font-size keys,
and Console.ReadLine echo. The result is written to `viewer-test.txt` beside the
built executable. `--clipboard-self-test` additionally tests the system clipboard
and changes clipboard text; save your clipboard first.

## မြန်မာလို အသုံးပြုနည်း

`Install.ps1` ကို run ပြီး Visual Studio ရဲ့ Create a new project မှာ
**Burmese Console App (Pyidaungsu)** ကို ရှာပါ။ Framework ရွေးပြီး project ဖန်တီးပါ။
`Program.cs` မှာ code ရေးပြီး F5 နှိပ်ပါ။ Input ကို prompt နောက်မှာပဲ ရိုက်ပါ။
Viewer project သီးခြားရွေးစရာ မလိုပါဘူး။

## Source layout

- `templates/BurmeseConsole/`: generated project source and template metadata.
- `SLH.BurmeseConsole.Templates.csproj`: NuGet template package project.
- `scripts/`: local build, install and uninstall commands.

See `templates/BurmeseConsole/THIRD-PARTY-NOTICES.txt` for icon/dependency provenance.

