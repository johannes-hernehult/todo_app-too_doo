using TooDoo.ConsoleUI;
using TooDoo.Services;
using TooDoo.Views;

namespace TooDoo;

public class App(TaskService taskService)
{
    private static readonly string[] MenuItems =
    [
        "(1) Show tasks",
        "(2) Add new task",
        "(3) Edit tasks",
        "(q) Quit"
    ];

    public async Task RunAsync()
    {
        taskService.SaveFailed += ReportSaveFailure;

        while (true)
        {
            switch (ConsoleMenu.Show("TooDoo", MenuItems, BuildTaskSummary()))
            {
                case "1":
                    await new ShowTasksView(taskService).ShowAsync();
                    break;
                case "2":
                    await new AddTaskView(taskService).ShowAsync();
                    break;
                case "3":
                    await new EditTasksView(taskService).ShowAsync();
                    break;
                case "q":
                    await taskService.SaveIfNeededAsync();
                    Console.WriteLine("Ok, bye!");
                    return;
                default:
                    Prompts.Pause("Invalid choice. Enter 1, 2, 3, or q.");
                    break;
            }
        }
    }

    private string BuildTaskSummary()
    {
        int total = taskService.TotalCount;

        if (total == 0)
        {
            return "You have no tasks yet.";
        }

        string noun = total == 1 ? "task" : "tasks";
        return $"You have {total} {noun} in total, {taskService.DoneCount} done.";
    }

    private static void ReportSaveFailure(string reason)
    {
        Prompts.Pause(
            $"Could not save your tasks: {reason}{Environment.NewLine}" +
            "Your changes are kept for this session. TooDoo will try again after the next change.");
    }
}