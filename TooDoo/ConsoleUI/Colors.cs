namespace TooDoo.ConsoleUI;

/// <summary>ANSI escape sequences. All console styling goes through this class.</summary>
public static class Colors
{
    public const string Reset = "\u001b[0m";

    public const string Heading = "\u001b[38;2;215;170;70m";
    public const string Neutral = "\u001b[38;2;170;170;170m";
    public const string Muted = "\u001b[38;2;120;120;120m";
    public const string Accent = "\u001b[38;2;100;180;220m";
    public const string Error = "\u001b[38;2;220;90;90m";

    public const string DueToday = "\u001b[38;2;100;200;120m";
    public const string DueYesterday = "\u001b[38;2;230;145;70m";
    public const string Overdue = "\u001b[38;2;220;90;90m";

    public const string AlternateRowBackground = "\u001b[48;2;40;40;40m";

    public static string Paint(string text, string color) => $"{color}{text}{Reset}";
}
