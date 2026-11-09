namespace TooDoo.ConsoleUI;

public class UserInput
{
    public string Prompt(string prompt)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(prompt);
        Console.ResetColor();
        return (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
    }
}