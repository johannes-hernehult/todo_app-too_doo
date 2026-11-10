using TooDoo.Services;
using TooDoo.ConsoleUI;
using TooDoo.Views;

namespace TooDoo;

public class App(TaskService taskService)
{
    private readonly List<string> _menuItems =
    [
        "(1) Show tasks",
        "(2) Add new task",
        "(3) Edit tasks",
        "(q) Quit"
    ];

    public void Run()
    {
        while (true)
        {
            string choice = UI.RenderMenu("TooDoo", _menuItems);

            switch (choice)
            {
                case "1":
                    new ViewTasksView(taskService).Show();
                    break;
                case "2":
                    new AddTaskView(taskService).Show();
                    break;
                case "3":
                    new EditTaskView(taskService).Show();
                    break;
                case "q":
                    // Save tasks here when persistence is implemented.
                    Console.Clear();
                    Console.WriteLine("Ok, bye!");

                    return;
                default:
                    Console.WriteLine("Choose 1, 2, 3, or q to quit.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}