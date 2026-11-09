namespace TooDoo.ConsoleUI;

public static class UI
{
    public static string RenderMenu(string title, List<string> items)
    {
        
        Console.Clear();
        UI.RenderList(title, items);

        return UserInput.Prompt("Pick an option: ");
    }

    public static void RenderList(string title, IReadOnlyList<string> items)
    {
        Console.Clear();
        Console.WriteLine(title);

        foreach (string item in items)
        {
            Console.WriteLine(item);
        }
    }
}