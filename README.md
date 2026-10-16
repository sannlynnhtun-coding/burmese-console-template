# Burmese Console Template

A Visual Studio and `dotnet new` template that creates **one C# project** with
Burmese Hello World in `Program.cs`. No separate Viewer project or startup selection.

The included `ConsoleHost.cs` uses Avalonia to render Pyidaungsu and edits input
on the current prompt line. The same assembly runs the user's normal
`Console.WriteLine` / `Console.ReadLine` code through UTF-8 pipes.
This is a console-style desktop host, not a dependency-free terminal application.

## Download and install (no cloning or building)

Install the .NET SDK (recommended: .NET 10), Visual Studio with .NET desktop
support, and **Pyidaungsu Regular**. The font is not bundled with the template.

1. Open [GitHub Releases](https://github.com/sannlynnhtun-coding/burmese-console-template/releases/latest).
2. Download **SLH.BurmeseConsole.Templates.1.2.3.nupkg** under Assets (not Source code).
3. Open a terminal in the download folder and run:

```powershell
dotnet new install .\SLH.BurmeseConsole.Templates.1.2.3.nupkg
```

Or download and install directly in PowerShell:

```powershell
Invoke-WebRequest -Uri "https://github.com/sannlynnhtun-coding/burmese-console-template/releases/download/v1.2.3/SLH.BurmeseConsole.Templates.1.2.3.nupkg" -OutFile "SLH.BurmeseConsole.Templates.1.2.3.nupkg"
dotnet new install .\SLH.BurmeseConsole.Templates.1.2.3.nupkg
```

On Linux/macOS, download the same asset and use:

```sh
curl -fL -o SLH.BurmeseConsole.Templates.1.2.3.nupkg https://github.com/sannlynnhtun-coding/burmese-console-template/releases/download/v1.2.3/SLH.BurmeseConsole.Templates.1.2.3.nupkg
dotnet new install ./SLH.BurmeseConsole.Templates.1.2.3.nupkg
```

### Use in Visual Studio

1. Open Visual Studio → **Create a new project**.
2. Search **Burmese Console App (Pyidaungsu)**.
3. Enter your project name and location, then choose **Framework**.
4. Create the project and press **F5**. There is only one project.

If the template is missing, restart Visual Studio. Framework choices are .NET
6, 7, 8, 9, 10 and 11 Preview. The SDK/runtime for your chosen framework is required.
.NET 6/7 are retained for compatibility; .NET 11 requires a preview SDK.

### Replace an earlier installation

Uninstall the previous version before installing the downloaded package:

```powershell
dotnet new uninstall SLH.BurmeseConsole.Templates
dotnet new install .\SLH.BurmeseConsole.Templates.1.2.3.nupkg
```

No NuGet feed setup, Git clone, or source build is needed. A SHA-256 checksum file
is included in the release assets for checking the downloaded package.

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

## Build from source (optional)

```powershell
git clone https://github.com/sannlynnhtun-coding/burmese-console-template.git
cd burmese-console-template
.\scripts\Install.ps1
# Build a package only: .\scripts\Build.ps1
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

GitHub Releases မှာ `.nupkg` ကို download လုပ်ပြီး `dotnet new install <ဖိုင်လမ်းကြောင်း>` နဲ့ install လုပ်ပါ။
Repo clone လုပ်ဖို့၊ build လုပ်ဖို့ မလိုပါဘူး။ Visual Studio ရဲ့ Create a new project မှာ
**Burmese Console App (Pyidaungsu)** ကို ရှာပါ။ Framework ရွေးပြီး project ဖန်တီးပါ။
`Program.cs` မှာ code ရေးပြီး F5 နှိပ်ပါ။ Input ကို prompt နောက်မှာပဲ ရိုက်ပါ။
Viewer project သီးခြားရွေးစရာ မလိုပါဘူး။

## Source layout

- `templates/BurmeseConsole/`: generated project source and template metadata.
- `SLH.BurmeseConsole.Templates.csproj`: NuGet template package project.
- `scripts/`: local build, install and uninstall commands.

See `templates/BurmeseConsole/THIRD-PARTY-NOTICES.txt` for icon/dependency provenance.


Uninstall at any time: `dotnet new uninstall SLH.BurmeseConsole.Templates`.

