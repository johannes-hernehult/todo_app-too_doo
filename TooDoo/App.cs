using TooDoo.Services;

namespace TooDoo;

public class App(TaskManager taskManager, UserInput userInput)
{
    public void Run()
    {
        while (true)
        {
            string choice = ShowMainMenu();

            switch (choice)
            {
                case "1":
                    ShowTasks();
                    break;
                case "2":
                    Console.WriteLine("Add task flow goes here.");
                    Console.ReadKey();
                    break;
                case "3":
                    Console.WriteLine("Edit task flow goes here.");
                    Console.ReadKey();
                    break;
                case "0":
                    // Save tasks here when persistence is implemented.
                    return;
                default:
                    Console.WriteLine("Choose an option from 1 to 3 (0 to exit).");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private string ShowMainMenu()
    {
        IReadOnlyList<string> menuItems = new List<string>()
        {
            "(1) Show tasks",
            "(2) Add new task",
            "(3) Edit tasks",
            "(4) Save and quit",
        };


        Console.Clear();
        UI.RenderMenu("Main Menu", menuItems);

        return userInput.ReadNormalizedString("Pick an option: ");
    }

    private void ShowTasks()
    {
        Console.Clear();
        Console.WriteLine("Tasks");
        userInput.ReadNormalizedString("Press Enter to return to the menu: ");
    }
}