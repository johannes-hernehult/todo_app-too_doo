using System.Globalization;
using TooDoo.ConsoleUI;
using TooDoo.Models;
using TooDoo.Services;

namespace TooDoo.Views;

public class AddTaskView
{
    private readonly TaskService _taskService;

    public AddTaskView(TaskService taskService)
    {
        _taskService = taskService;
    }

    public void Show()
    {
        List<TaskItem> sessionTasks = [];

        while (true)
        {
            Console.Clear();
            UI.RenderHeading("Add new task");

            // Move your existing TryPromptTaskDetails method into this class.
            if (!TryPromptTaskDetails(
                    out string title,
                    out DateOnly dueDate,
                    out string project))
            {
                return;
            }

            TaskItem task = _taskService.AddTask(title, dueDate, project);
            sessionTasks.Add(task);

            Console.Clear();
            UI.RenderHeading("Session Tasks");
            UI.RenderTasks(
                sessionTasks,
                TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);

            // Move your existing ShouldAddAnotherTask method into this class.
            if (!ShouldAddAnotherTask())
            {
                return;
            }
        }
    }

    private bool TryPromptTaskDetails(out string title, out DateOnly dueDate, out string project)
    {
        title = string.Empty;
        dueDate = default;
        project = string.Empty;

        string? enteredTitle = UserInput.PromptValidated("Title: ", DataType.String);
        if (enteredTitle is null)
        {
            return false;
        }

        Console.Write("Existing projects: ");
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        try
        {
            Console.WriteLine(TaskItem.RenderProjectNames(_taskService.GetTasks()));
        }
        finally
        {
            Console.ResetColor();
        }

        string? enteredProject = UserInput.PromptValidated("Project: ", DataType.String);
        if (enteredProject is null)
        {
            return false;
        }

        while (true)
        {
            string? dateText = UserInput.PromptValidated("Due Date (yyyy-MM-dd): ", DataType.DateOnly);
            if (dateText is null)
            {
                return false;
            }

            if (DateOnly.TryParseExact(
                    dateText,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dueDate))
            {
                title = enteredTitle;
                project = enteredProject;
                return true;
            }

            Console.WriteLine("Enter a date in yyyy-MM-dd format.");
        }
    }

    private static bool ShouldAddAnotherTask()
    {
        string input = UserInput.Prompt("Press 'Enter' to add another task ('q' finish): ");
        return !string.Equals(input.Trim(), "q", StringComparison.OrdinalIgnoreCase);
    }
}