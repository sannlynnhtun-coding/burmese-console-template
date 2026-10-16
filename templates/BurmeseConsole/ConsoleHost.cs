using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;

internal static class BurmeseConsoleHost
{
    internal static bool SelfTest { get; private set; }
    internal static bool ClipboardTest { get; private set; }

    // The same executable hosts the display and runs the user's Console code.
    public static bool Start(string[] args)
    {
        if (args.Contains("--burmese-console-worker")) return false;
        ClipboardTest = args.Contains("--clipboard-self-test");
        SelfTest = args.Contains("--self-test") || ClipboardTest;
        AppBuilder.Configure<ViewerApplication>().UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
        return true;
    }
}

public sealed class ViewerApplication : Application
{
    public override void Initialize()
    {
        RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
        Styles.Add(new FluentTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new ConsoleViewer();
        base.OnFrameworkInitializationCompleted();
    }
}

public sealed class ConsoleViewer : Window
{
    private readonly TextBox console = new()
    {
        AcceptsReturn = true,
        IsReadOnly = true,
        TextWrapping = TextWrapping.Wrap,
        BorderThickness = new Thickness(0),
        FontFamily = new FontFamily("Pyidaungsu"),
        FontSize = 26,
        Foreground = Brushes.White,
        Background = Brushes.Black,
        Padding = new Thickness(20)
    };
    private Process? child;
    private string history = "";
    private string validText = "";
    private bool updating;
    private bool closing;
    private string CurrentInput => console.Text is { } text && text.StartsWith(history, StringComparison.Ordinal) ? text[history.Length..] : "";

    public ConsoleViewer()
    {
        Title = "Burmese Console — Pyidaungsu";
        Width = 1150;
        Height = 760;
        Background = Brushes.Black;
        Content = console;
        ConfigureClipboard();
        if (OperatingSystem.IsWindows()) Win32Properties.AddWndProcHookCallback(this, HandleWindowsMenu);
        console.TextChanged += (_, _) =>
        {
            if (updating) return;
            string text = console.Text ?? "";
            if (text.StartsWith(history, StringComparison.Ordinal)) validText = text;
            else SetText(validText, validText.Length, validText.Length);
        };
        AddHandler(InputElement.TextInputEvent, (_, _) => PrepareForEdit(), RoutingStrategies.Tunnel);
        AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        Opened += async (_, _) => await RunAsync();
        Closed += (_, _) =>
        {
            closing = true;
            if (child is { HasExited: false }) child.Kill(entireProcessTree: true);
        };
    }

    private void SetText(string text, int start, int end)
    {
        updating = true;
        try
        {
            console.Text = validText = text;
            console.SelectionStart = start;
            console.SelectionEnd = end;
        }
        finally { updating = false; }
    }

    private void MoveCaret(int position)
    {
        console.SelectionStart = position;
        console.SelectionEnd = position;
        console.CaretIndex = position;
    }

    private void PrepareForEdit()
    {
        if (console.IsReadOnly) return;
        int start = Math.Min(console.SelectionStart, console.SelectionEnd);
        int end = Math.Max(console.SelectionStart, console.SelectionEnd);
        if (start >= history.Length) return;
        if (end > history.Length)
        {
            console.SelectionStart = history.Length;
            console.SelectionEnd = end;
        }
        else MoveCaret(console.Text?.Length ?? 0);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr DefWindowProcW(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

    private static IntPtr HandleWindowsMenu(IntPtr window, uint message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        bool altSpace = message is 0x0104 or 0x0106 && wParam.ToInt64() == 0x20;
        bool systemMenu = message == 0x0112 && (wParam.ToInt64() & 0xFFF0) == 0xF100 && lParam.ToInt64() == 0x20;
        if (!altSpace && !systemMenu) return IntPtr.Zero;
        handled = true;
        return DefWindowProcW(window, message, wParam, lParam);
    }

    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {

        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            !e.KeyModifiers.HasFlag(KeyModifiers.Alt) &&
            e.Key is Key.OemPlus or Key.Add or Key.OemMinus or Key.Subtract)
        {
            double step = e.Key is Key.OemPlus or Key.Add ? 2 : -2;
            console.FontSize = Math.Clamp(console.FontSize + step, 10, 72);
            e.Handled = true;
            return;
        }
        bool command = e.KeyModifiers.HasFlag(KeyModifiers.Control) ||
            (OperatingSystem.IsMacOS() && e.KeyModifiers.HasFlag(KeyModifiers.Meta));
        if (command && e.Key == Key.C)
        {
            console.Copy();
            e.Handled = true;
        }
        else if ((command && e.Key == Key.V) || (e.Key == Key.Insert && e.KeyModifiers == KeyModifiers.Shift))
        {
            e.Handled = true;
            await PasteAsync();
        }
        else if (command && e.Key == Key.A)
        {
            console.SelectAll();
            e.Handled = true;
        }
        else if (command && e.Key == Key.X)
        {
            e.Handled = true;
            if (!console.IsReadOnly)
            {
                PrepareForEdit();
                console.Cut();
            }
        }
        else if (command && e.Key is Key.Z or Key.Y)
            e.Handled = true; // Previous console output is immutable.
        else if (e.Key == Key.Enter)
        {
            e.Handled = true;
            await SubmitAsync();
        }
        else if (!console.IsReadOnly && e.Key is Key.Back or Key.Delete)
        {
            int start = Math.Min(console.SelectionStart, console.SelectionEnd);
            int end = Math.Max(console.SelectionStart, console.SelectionEnd);
            if (end <= history.Length && (e.Key == Key.Back || start < history.Length))
                e.Handled = true;
            else PrepareForEdit();
        }
        else if (!console.IsReadOnly && e.Key == Key.Home && !command)
        {
            MoveCaret(history.Length);
            e.Handled = true;
        }
    }

    private void ConfigureClipboard()
    {
        var copy = new MenuItem { Header = "Copy" };
        copy.Click += (_, _) => console.Copy();
        var paste = new MenuItem { Header = "Paste" };
        paste.Click += async (_, _) => await PasteAsync();
        var selectAll = new MenuItem { Header = "Select all" };
        selectAll.Click += (_, _) => console.SelectAll();
        console.ContextMenu = new ContextMenu { ItemsSource = new object[] { copy, paste, selectAll } };
    }

    private async Task PasteAsync()
    {
        if (console.IsReadOnly || Clipboard is not { } clipboard) return;
        string? text = await clipboard.TryGetTextAsync();
        if (text is null || console.IsReadOnly) return;
        console.Focus();
        PrepareForEdit();
        // Pasting never submits a command; Enter submits the current line.
        console.SelectedText = text.Replace("\r\n", "\n").Replace('\r', '\n').Replace('\n', ' ');
    }

    private void Append(string text)
    {
        string pending = CurrentInput;
        int oldBoundary = history.Length;
        int start = console.SelectionStart;
        int end = console.SelectionEnd;
        history += text;
        if (start == end) start = end = history.Length + pending.Length;
        else
        {
            if (start > oldBoundary) start += text.Length;
            if (end > oldBoundary) end += text.Length;
        }
        SetText(history + pending, start, end);
    }

    private async Task SubmitAsync()
    {
        if (console.IsReadOnly || child is null || child.HasExited) return;
        string text = CurrentInput;
        history = (console.Text ?? "") + Environment.NewLine;
        SetText(history, history.Length, history.Length);
        try
        {
            await child.StandardInput.WriteLineAsync(text);
            await child.StandardInput.FlushAsync();
        }
        catch (IOException ex)
        {
            console.IsReadOnly = true;
            Append("\nConsole input closed: " + ex.Message);
        }
    }

    private async Task PumpAsync(StreamReader reader)
    {
        var buffer = new char[1024];
        int count;
        while ((count = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            string text = new(buffer, 0, count);
            await Dispatcher.UIThread.InvokeAsync(() => Append(text));
        }
    }

    private void Type(string text)
    {
        foreach (char c in text)
            console.RaiseEvent(new TextInputEventArgs { RoutedEvent = InputElement.TextInputEvent, Text = c.ToString() });
    }

    private void KeyPress(Key key, KeyModifiers modifiers = KeyModifiers.None) =>
        console.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key, KeyModifiers = modifiers });

    private async Task CheckEditingAsync()
    {
        double originalSize = console.FontSize;
        string originalText = console.Text ?? "";
        KeyPress(Key.OemPlus, KeyModifiers.Control | KeyModifiers.Shift);
        if (console.FontSize != originalSize + 2) throw new InvalidOperationException("Font increase failed.");
        KeyPress(Key.OemMinus, KeyModifiers.Control);
        if (console.FontSize != originalSize) throw new InvalidOperationException("Font decrease failed.");
        KeyPress(Key.Add, KeyModifiers.Control);
        KeyPress(Key.Subtract, KeyModifiers.Control);
        if (console.FontSize != originalSize) throw new InvalidOperationException("Numpad zoom failed.");
        for (int i = 0; i < 40; i++) KeyPress(Key.Add, KeyModifiers.Control);
        if (console.FontSize != 72) throw new InvalidOperationException("Maximum font size failed.");
        for (int i = 0; i < 40; i++) KeyPress(Key.Subtract, KeyModifiers.Control);
        if (console.FontSize != 10) throw new InvalidOperationException("Minimum font size failed.");
        console.FontSize = originalSize;
        if (console.Text != originalText) throw new InvalidOperationException("Font shortcuts changed console text.");
        string prompt = history;
        MoveCaret(history.Length);
        KeyPress(Key.Back);
        if ((console.Text ?? "") != prompt) throw new InvalidOperationException("Backspace modified the prompt.");
        Type("မောင်အောင်ကျောX");
        KeyPress(Key.Back);
        Type("်");
        if (CurrentInput != "မောင်အောင်ကျော်") throw new InvalidOperationException($"Inline typing / Backspace failed: [{CurrentInput}]");
        console.SelectionStart = 0;
        console.SelectionEnd = history.Length;
        Type("X");
        if (history != prompt || !(console.Text?.StartsWith(prompt, StringComparison.Ordinal) ?? false))
            throw new InvalidOperationException("Typing modified console history.");
        KeyPress(Key.Back);
        if (CurrentInput != "မောင်အောင်ကျော်") throw new InvalidOperationException("History selection editing failed.");
        if (BurmeseConsoleHost.ClipboardTest)
        {
            var clipboard = Clipboard ?? throw new InvalidOperationException("Clipboard unavailable.");
            console.SelectionStart = 0;
            console.SelectionEnd = Math.Min(10, history.Length);
            string expected = console.SelectedText;
            KeyPress(Key.C, KeyModifiers.Control);
            await Task.Delay(200);
            if (await clipboard.TryGetTextAsync() != expected) throw new InvalidOperationException("Output copy failed.");
            console.SelectionStart = history.Length;
            console.SelectionEnd = console.Text!.Length;
            await clipboard.SetTextAsync("မောင်အောင်ကျော်");
            KeyPress(Key.V, KeyModifiers.Control);
            await Task.Delay(200);
            if (CurrentInput != "မောင်အောင်ကျော်") throw new InvalidOperationException("Inline paste failed.");
        }
        MoveCaret(console.Text!.Length);
        await SubmitAsync();
    }

    private async Task RunAsync()
    {
        try
        {
            string payloadDirectory = AppContext.BaseDirectory;
            var start = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardInputEncoding = new UTF8Encoding(false),
                StandardOutputEncoding = new UTF8Encoding(false, true),
                StandardErrorEncoding = new UTF8Encoding(false, true),
                WorkingDirectory = payloadDirectory
            };
            start.ArgumentList.Add(typeof(BurmeseConsoleHost).Assembly.Location);
            start.ArgumentList.Add("--burmese-console-worker");
            child = Process.Start(start) ?? throw new IOException("Could not start Console App.");
            console.IsReadOnly = false;
            console.Focus();
            var stdout = PumpAsync(child.StandardOutput);
            var stderr = PumpAsync(child.StandardError);
            if (BurmeseConsoleHost.SelfTest)
            {
                var deadline = DateTime.UtcNow.AddSeconds(15);
                while (!history.Contains("ရိုက်ထည့်ပါ:", StringComparison.Ordinal))
                {
                    if (DateTime.UtcNow > deadline) throw new TimeoutException("Console prompt not ready.");
                    await Task.Delay(50);
                }
                await CheckEditingAsync();
            }
            await child.WaitForExitAsync();
            await Task.WhenAll(stdout, stderr);
            if (closing) return;
            console.IsReadOnly = true;
            Title = $"Burmese Console — exited ({child.ExitCode})";
            if (BurmeseConsoleHost.SelfTest)
            {
                string saved = "မောင်အောင်ကျော်";
                bool passed = child.ExitCode == 0 && (console.Text?.Contains($"မင်္ဂလာပါ၊ {saved}။", StringComparison.Ordinal) ?? false);
                string report = $"Inline input + protected prompt + Backspace + clipboard (if requested) + Console.ReadLine echo: {(passed ? "PASS" : "FAIL")}\nReceived: {saved}\n{console.Text}";
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "viewer-test.txt"), report, new UTF8Encoding(false));
                Title = passed ? "Burmese Console — SELF-TEST PASS" : "Burmese Console — SELF-TEST FAIL";
            }
        }
        catch (Exception ex)
        {
            if (closing) return;
            console.IsReadOnly = true;
            Append("\nERROR: " + ex.Message);
            Title = "Burmese Console — failed";
            if (BurmeseConsoleHost.SelfTest)
                File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "viewer-test.txt"), "FAIL: " + ex, new UTF8Encoding(false));
        }
        finally
        {
            child?.Dispose();
            child = null;
        }
    }
}









