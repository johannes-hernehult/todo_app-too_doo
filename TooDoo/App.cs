using TooDoo.Services;
using TooDoo.ConsoleUI;
using TaskStatus = TooDoo.Models.TaskStatus;

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


    private void ViewTasks()
    {
        Console.Clear();
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly startDate = today.AddDays(-2);
        var tasks = taskService.GetTasks();

        var orderedTasks = tasks.OrderBy(task => task.DueDate).ThenBy(task => task.Project).ThenBy(task => task.Title)
            .ToList();

        var notDoneTasks = orderedTasks.Where(task => task.Status == TaskStatus.NotDone).ToList();
        var doneTasksPastThreeDays = orderedTasks
            .Where(task =>
                task.Status == TaskStatus.Done &&
                task.DueDate >= startDate &&
                task.DueDate <= today)
            .ToList();

        Console.WriteLine("Tasks to do");
        UI.RenderTasks(notDoneTasks, TaskColumns.Id | TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);

        Console.WriteLine();
        Console.WriteLine("Done tasks");
        //UI.RenderTasks(doneTasks, TaskColumns.Id | TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);
        UI.RenderTasks(doneTasksPastThreeDays);

        Console.WriteLine();
        UserInput.Prompt("Press Enter to return to the menu: ");
    }
}