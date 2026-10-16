using System.Text;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (BurmeseConsoleHost.Start(args)) return;

        Console.InputEncoding = new UTF8Encoding(false);
        Console.OutputEncoding = new UTF8Encoding(false);

        Console.WriteLine("မင်္ဂလာပါ ကမ္ဘာကြီး။");
        Console.Write("သင့်အမည်ကို မြန်မာလို ရိုက်ထည့်ပါ: ");
        string? name = Console.ReadLine();
        Console.WriteLine($"မင်္ဂလာပါ၊ {name}။");
    }
}
