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
        bool sortByProject = false;

        while (true)
        {
            Console.Clear();

            List<TaskItem> orderedTasks = GetOrderedTasks(sortByProject);
            List<TaskItem> visibleTasks = orderedTasks
                .Where(task => showDoneTasks
                    ? task.Status == TaskStatus.Done
                    : task.Status != TaskStatus.Done)
                .ToList();

            string orderName = sortByProject
                ? "Project"
                : "Due date";

            UI.RenderHeading(
                $"{(showDoneTasks ? "Completed tasks" : "Tasks to do")} | Order: {orderName}");
            UI.RenderTasks(
                visibleTasks,
                TaskColumns.Id | TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);

            Console.WriteLine();
            Console.Write(
                "Type 'toggle <taskID>', 'view' to switch view, 'o' to change order, or 'q' to return: ");

            string? input = Console.ReadLine();
            if (string.Equals(input?.Trim(), "q", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            ProcessTaskCommand(input, visibleTasks, ref showDoneTasks, ref sortByProject);
        }
    }

    private List<TaskItem> GetOrderedTasks(bool sortByProject)
    {
        IOrderedEnumerable<TaskItem> orderedTasks = sortByProject
            ? _taskService.GetTasks()
                .OrderBy(task => task.Project, StringComparer.OrdinalIgnoreCase)
                .ThenBy(task => task.DueDate)
                .ThenBy(task => task.Title, StringComparer.OrdinalIgnoreCase)
            : _taskService.GetTasks()
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Project, StringComparer.OrdinalIgnoreCase)
                .ThenBy(task => task.Title, StringComparer.OrdinalIgnoreCase);

        return orderedTasks.ToList();
    }

    private void ProcessTaskCommand(
        string input,
        List<TaskItem> visibleTasks,
        ref bool showDoneTasks,
        ref bool sortByProject)
    {
        string[] command = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (command.Length == 1 &&
            command[0].Equals("view", StringComparison.OrdinalIgnoreCase))
        {
            showDoneTasks = !showDoneTasks;
            return;
        }

        if (command.Length == 1 &&
            command[0].Equals("o", StringComparison.OrdinalIgnoreCase))
        {
            sortByProject = !sortByProject;
            return;
        }

        if (command.Length == 2 &&
            command[0].Equals("toggle", StringComparison.OrdinalIgnoreCase))
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
            Console.WriteLine("Enter 'toggle <taskID>', 'view', or 'o'.");
        }

        Console.WriteLine("Press any key to continue.");
        Console.ReadKey(intercept: true);
    }
}