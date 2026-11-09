namespace TooDoo.ConsoleUI;

public static class UserInput
{
    public static string Prompt(string prompt)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(prompt);
        Console.ResetColor();
        return (Console.ReadLine() ?? string.Empty).Trim();
    }

    public static string? PromptValidated(string prompt, DataType dataType)
    {
        while (true)
        {
            string value = Prompt(prompt);

            if (value.ToLowerInvariant() is "q")
            {
                return null;
            }

            string? error = Validate.GetError(dataType, value);
            if (error is null)
            {
                return value;
            }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"ERROR: {error}");
            Console.ResetColor();
        }
    }
}