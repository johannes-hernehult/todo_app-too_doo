using TooDoo.ConsoleUI;
using TooDoo.Models;
using TooDoo.Services;
using TaskStatus = TooDoo.Models.TaskStatus;

namespace TooDoo.Views;

public class ViewTasksView
{
    private readonly TaskService _taskService;

    public ViewTasksView(TaskService taskService)
    {
        _taskService = taskService;
    }

    public void Show()
    {
        bool showDoneTasks = false;
        var orderedTasks = _taskService.GetTasks()
            .OrderBy(task => task.DueDate)
            .ThenBy(task => task.Project)
            .ThenBy(task => task.Title)
            .ToList();

        while (true)
        {
            Console.Clear();

            var visibleTasks = orderedTasks
                .Where(task => showDoneTasks
                    ? task.Status == TaskStatus.Done
                    : task.Status != TaskStatus.Done)
                .ToList();

            UI.RenderHeading(showDoneTasks ? "Completed tasks" : "Tasks to do");
            UI.RenderTasks(visibleTasks,
                TaskColumns.Id | TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);

            Console.WriteLine();
            Console.Write(
                "Type 'toggle <taskID>' to change a task's status - 'view' to switch lists - Enter to return: ");

            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            ProcessTaskCommand(input, visibleTasks, ref showDoneTasks);
        }
    }

    private void ProcessTaskCommand(string input, List<TaskItem> visibleTasks, ref bool showDoneTasks)
    {
        string[] command = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (command.Length == 1 && command[0].Equals("view", StringComparison.OrdinalIgnoreCase))
        {
            showDoneTasks = !showDoneTasks;
            return;
        }

        if (command.Length == 2 && command[0].Equals("toggle", StringComparison.OrdinalIgnoreCase))
        {
            TaskItem? task = visibleTasks.FirstOrDefault(task =>
                task.Id.Equals(command[1], StringComparison.OrdinalIgnoreCase));

            if (task is not null)
            {
                TaskStatus newStatus = task.Status == TaskStatus.Done
                    ? TaskStatus.NotDone
                    : TaskStatus.Done;
                task.UpdateStatus(newStatus);
                return;
            }

            Console.WriteLine("That task ID is not in the current list.");
        }
        else
        {
            Console.WriteLine("Enter 'toggle <taskID>' or 'view'; press Enter to return.");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey(intercept: true);
    }
}