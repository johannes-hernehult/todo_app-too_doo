namespace TooDoo.ConsoleUI;

public static class UserInput
{
    public static string Prompt(string prompt)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(prompt);
        Console.ResetColor();
        return (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
    }
}