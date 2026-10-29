namespace TooDoo;

public static class UI
{
    public static void RenderMenu(string title, IReadOnlyList<string> items)
    {
        Console.Clear();
        Console.WriteLine(title);

        foreach (string item in items)
        {
            Console.WriteLine(item);
        }
    }
}