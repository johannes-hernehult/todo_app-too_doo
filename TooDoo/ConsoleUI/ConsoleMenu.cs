namespace TooDoo.ConsoleUI;

public static class ConsoleMenu
{
    /// <param name="summary">Optional line shown between the heading and the items.</param>
    /// <returns>The lowercase choice the user typed.</returns>
    public static string Show(string title, IReadOnlyList<string> items, string? summary = null)
    {
        Console.Clear();
        WriteHeading(title);

        if (summary is not null)
        {
            Console.WriteLine(summary);
            Console.WriteLine();
        }

        foreach (string item in items)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine();
        return Prompts.ReadLine("Choose an option: ").ToLowerInvariant();
    }

    public static void WriteHeading(string title)
    {
        string line = new('-', GetLineWidth());
        string heading = string.Join(Environment.NewLine, line, title, line);

        Console.WriteLine(Colors.Paint(heading, Colors.Heading));
    }

    private static int GetLineWidth()
    {
        const int fallbackWidth = 80;

        return Console.IsOutputRedirected
            ? fallbackWidth
            : Math.Max(1, Console.WindowWidth - 1);
    }
}