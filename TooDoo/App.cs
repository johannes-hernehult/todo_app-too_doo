using TooDoo.Services;
using TooDoo.ConsoleUI;

namespace TooDoo;

public class App(TaskService taskService)
{
    private List<string> _menuItems = new List<string>
    {
        "(1) Show tasks",
        "(2) Add new task",
        "(3) Edit tasks",
        "(0) Save and quit",
    };

    public void Run()
    {
        while (true)
        {
            string choice = UI.RenderMenu("TooDoo", _menuItems);

            switch (choice)
            {
                case "1":
                    ViewTasks();
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


    private void ViewTasks()
    {
        Console.Clear();
        Console.WriteLine("Tasks");
        Console.WriteLine();

        var tasks = taskService.GetTasks();

        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks yet.");
        }
        else
        {
            foreach (var task in tasks)
            {
                Console.WriteLine($"{task.Id} | {task.Title}");
                Console.WriteLine(
                    $"  Due: {task.DueDate:yyyy-MM-dd} | Project: {task.Project ?? "None"} | Status: {task.Status}");
            }
        }

        Console.WriteLine();
        UserInput.Prompt("Press Enter to return to the menu: ");
    }
}