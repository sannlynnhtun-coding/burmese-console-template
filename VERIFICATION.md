# Verification

- Built the template package from this standalone repository successfully.
- Installed `SLH.BurmeseConsole.Templates` version 1.2.3 from artifacts/packages.
- Generated `VerifyBurmese` with `-f net10.0`; exactly one root .csproj was generated.
- Generated project build: zero warnings, zero errors.
- Generated application's --self-test: PASS for inline Burmese text, protected prompt,
  Backspace, font increase/decrease/numpad/bounds, and Console.ReadLine echo.
- Earlier source verification also covered clipboard and Windows Alt+Space then C.
- Visual Studio template metadata includes the C# console icon and Framework selection.
- Linux/macOS native execution and IME have not been tested on this Windows machine.
- GitHub repository: https://github.com/sannlynnhtun-coding/burmese-console-template (public).


