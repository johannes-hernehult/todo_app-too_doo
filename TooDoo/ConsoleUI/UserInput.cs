namespace TooDoo.ConsoleUI;

public class UserInput
{
    public string ReadNormalizedString(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
    }
}