using TooDoo.Services;
using TooDoo.ConsoleUI;
using TooDoo.Views;

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
                    new ViewTasksView(taskService).Show();
                    break;
                case "2":
                    new AddTaskView(taskService).Show();
                    break;
                case "3":
                    new EditTaskView(taskService).Show();
                    break;
                case "0":
                    // Save tasks here when persistence is implemented.
                    Console.Clear();
                    Console.WriteLine("Ok, bye!");

                    return;
                default:
                    Console.WriteLine("Choose an option from 1 to 3 (0 to exit).");
                    Console.ReadKey();
                    break;
            }
        }
    }
}