using TooDoo.ConsoleUI;
using TooDoo.Models;
using TooDoo.Services;
using TaskStatus = TooDoo.Models.TaskStatus;

namespace TooDoo.Views;

public class EditTaskView
{
    private readonly TaskService _taskService;

    public EditTaskView(TaskService taskService)
    {
        _taskService = taskService;
    }

    public void Show()
    {
        bool showDoneTasks = false;

        while (true)
        {
            Console.Clear();

            List<TaskItem> visibleTasks = _taskService.GetTasks()
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Project)
                .ThenBy(task => task.Title)
                .Where(task => showDoneTasks
                    ? task.Status == TaskStatus.Done
                    : task.Status != TaskStatus.Done)
                .ToList();

            UI.RenderHeading(showDoneTasks ? "Edit completed tasks" : "Edit tasks to do");
            UI.RenderTasks(
                visibleTasks,
                TaskColumns.Id | TaskColumns.Title | TaskColumns.DueDate |
                TaskColumns.Project | TaskColumns.Status);

            Console.WriteLine();
            Console.Write(
                "Use 'edit <taskID>', 'delete <taskID>', 'toggle <taskID>', 'view', or 'q' to return: ");

            string? input = Console.ReadLine();
            if (string.Equals(input?.Trim(), "q", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            ProcessCommand(input, visibleTasks, ref showDoneTasks);
        }
    }

    private void ProcessCommand(
        string input,
        List<TaskItem> visibleTasks,
        ref bool showDoneTasks)
    {
        string[] command = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (command.Length == 1 && command[0].Equals("view", StringComparison.OrdinalIgnoreCase))
        {
            showDoneTasks = !showDoneTasks;
            return;
        }

        if (command.Length == 2 &&
            command[0].Equals("toggle", StringComparison.OrdinalIgnoreCase))
        {
            TaskItem? task = FindVisibleTask(command[1], visibleTasks);
            if (task is null)
            {
                ShowMessage("That task ID is not in the current list.");
                return;
            }

            task.UpdateStatus(task.Status == TaskStatus.Done
                ? TaskStatus.NotDone
                : TaskStatus.Done);
            return;
        }

        if (command.Length == 2 &&
            command[0].Equals("edit", StringComparison.OrdinalIgnoreCase))
        {
            TaskItem? task = FindVisibleTask(command[1], visibleTasks);
            if (task is null)
            {
                ShowMessage("That task ID is not in the current list.");
                return;
            }

            EditTask(task);
            return;
        }

        if (command.Length == 2 &&
            command[0].Equals("delete", StringComparison.OrdinalIgnoreCase))
        {
            TaskItem? task = FindVisibleTask(command[1], visibleTasks);
            if (task is null)
            {
                ShowMessage("That task ID is not in the current list.");
                return;
            }

            string confirmation = UserInput.Prompt(
                $"Delete '{task.Title}'? Type 'y' to confirm: ");

            if (confirmation.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                _taskService.DeleteTask(task.Id);
            }

            return;
        }

        ShowMessage("Enter 'edit <taskID>', 'delete <taskID>', 'toggle <taskID>', or 'view'.");
    }

    private static TaskItem? FindVisibleTask(string id, List<TaskItem> visibleTasks)
    {
        return visibleTasks.FirstOrDefault(task =>
            task.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    private static void EditTask(TaskItem task)
    {
        if (!TryPromptTaskDetails(
                task,
                out string title,
                out DateOnly dueDate,
                out string? project))
        {
            return;
        }

        if (!string.Equals(title, task.Title, StringComparison.Ordinal))
        {
            task.UpdateTitle(title);
        }

        if (dueDate != task.DueDate)
        {
            task.UpdateDueDate(dueDate);
        }

        if (!string.Equals(project, task.Project, StringComparison.Ordinal))
        {
            if (project is null)
            {
                task.ClearProject();
            }
            else
            {
                task.UpdateProject(project);
            }
        }
    }

    private static bool TryPromptTaskDetails(
        TaskItem task,
        out string title,
        out DateOnly dueDate,
        out string? project)
    {
        title = task.Title;
        dueDate = task.DueDate;
        project = task.Project;

        while (true)
        {
            string? enteredTitle = PromptOrCancel(
                $"Title (Enter keeps '{task.Title}'; 'q' cancels): ");

            if (enteredTitle is null)
            {
                return false;
            }

            if (enteredTitle.Length == 0)
            {
                break;
            }

            string? error = Validate.GetError(DataType.String, enteredTitle);
            if (error is null)
            {
                title = enteredTitle;
                break;
            }

            Console.WriteLine($"ERROR: {error}");
        }

        while (true)
        {
            string? enteredDate = PromptOrCancel(
                $"Due date (yyyy-MM-dd; Enter keeps '{task.DueDate:yyyy-MM-dd}'): ");

            if (enteredDate is null)
            {
                return false;
            }

            if (enteredDate.Length == 0)
            {
                break;
            }

            if (DateOnly.TryParseExact(
                    enteredDate,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateOnly parsedDate))
            {
                dueDate = parsedDate;
                break;
            }

            Console.WriteLine("Enter a date in yyyy-MM-dd format.");
        }

        string? enteredProject = PromptOrCancel(
            $"Project (Enter keeps '{task.Project ?? "-"}'; '-' clears it): ");

        if (enteredProject is null)
        {
            return false;
        }

        if (enteredProject == "-")
        {
            project = null;
        }
        else if (enteredProject.Length > 0)
        {
            project = enteredProject;
        }

        return true;
    }

    private static string? PromptOrCancel(string prompt)
    {
        string input = UserInput.Prompt(prompt);
        return input.Equals("q", StringComparison.OrdinalIgnoreCase) ? null : input;
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine("Press any key to continue.");
        Console.ReadKey(intercept: true);
    }
}