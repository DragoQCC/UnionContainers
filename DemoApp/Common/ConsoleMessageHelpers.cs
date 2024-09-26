using System.Drawing;
using Pastel;

namespace DemoApp.Common;

public static class ConsoleMessageHelpers
{
    public static string Info() => $"{"[* Info]".Pastel(Color.SkyBlue)}";

    public static string Error() => $"{"[\u2717 Error]".Pastel(Color.Red)}";

    public static string Success() => $"{"[\u2713 Success]".Pastel(Color.LimeGreen)}";

    public static string Caution() => $"{"[\u26A0 Caution]".Pastel(Color.Gold)}";
}