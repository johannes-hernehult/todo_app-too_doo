using System.Globalization;
using TooDoo.Services;
using TooDoo.ConsoleUI;
using TooDoo.Models;
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
                    AddTask();
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
        bool showDoneTasks = false;
        var orderedTasks = taskService.GetTasks()
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

    private void AddTask()
    {
        List<TaskItem> sessionTasks = [];

        while (true)
        {
            Console.Clear();
            UI.RenderHeading("Add new task");

            if (!TryPromptTaskDetails(out string title, out DateOnly dueDate, out string project))
            {
                return;
            }

            TaskItem task = taskService.AddTask(title, dueDate, project);
            sessionTasks.Add(task);

            Console.Clear();
            UI.RenderHeading("Session Tasks");
            UI.RenderTasks(sessionTasks, TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);

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
            Console.WriteLine(TaskItem.RenderProjectNames(taskService.GetTasks()));
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