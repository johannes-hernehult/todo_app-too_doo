using TooDoo.Services;
using TooDoo.ConsoleUI;
using TooDoo.Views;

namespace TooDoo;

public class App(
    TaskService taskService,
    FileService fileService,
    string dataFilePath)
{
    private readonly List<string> _menuItems =
    [
        "(1) Show tasks",
        "(2) Add new task",
        "(3) Edit tasks",
        "(q) Quit"
    ];

    public async Task RunAsync()
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
                    try
                    {
                        await fileService.SaveAsync(dataFilePath, taskService.GetTasks());
                        Console.WriteLine("Tasks saved.");
                    }
                    catch (Exception ex) when (
                        ex is IOException or UnauthorizedAccessException)
                    {
                        Console.WriteLine($"Could not save tasks: {ex.Message}");
                    }

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