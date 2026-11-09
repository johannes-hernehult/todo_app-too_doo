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

        while (true)
        {
            Console.Clear();
            var orderedTasks = taskService.GetTasks()
                .OrderBy(task => task.DueDate)
                .ThenBy(task => task.Project)
                .ThenBy(task => task.Title)
                .ToList();

            var visibleTasks = orderedTasks
                .Where(task => (task.Status == TaskStatus.Done) == showDoneTasks)
                .ToList();

            UI.RenderHeading(showDoneTasks ? "Completed tasks" : "Tasks to do");
            UI.RenderTasks(visibleTasks,
                TaskColumns.Id | TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);

            Console.WriteLine();
            Console.Write(showDoneTasks
                ? "Showing completed tasks. Press 't' to show tasks to do, or Enter to return: "
                : "Showing tasks to do. Press 't' to show completed tasks, or Enter to return: ");

            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            if (key == ConsoleKey.Enter)
            {
                return;
            }

            if (key == ConsoleKey.T)
            {
                showDoneTasks = !showDoneTasks;
            }
        }
    }

    private void AddTask()
    {
        List<TaskItem> sessionTasks = [];
        while (true)
        {
            Console.Clear();

            UI.RenderHeading("Add new task");
            string? title = UserInput.PromptValidated("Title: ", DataType.String);
            if (title is null) break;

            Console.Write("Existing projects: ");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{TaskItem.RenderProjectNames(taskService.GetTasks())}");
            Console.ResetColor();
            string? project = UserInput.PromptValidated("Project: ", DataType.String);
            if (project is null) break;

            string? dueDateString = UserInput.PromptValidated("Due Date (yyyy-MM-dd): ", DataType.DateOnly);
            if (dueDateString is null) break;

            DateOnly dueDate = DateOnly.ParseExact(dueDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            TaskItem task = taskService.AddTask(title, dueDate, project);
            sessionTasks.Add(task);

            Console.Clear();
            UI.RenderHeading("Session Tasks");
            UI.RenderTasks(sessionTasks, TaskColumns.Title | TaskColumns.DueDate | TaskColumns.Project);


            string input = UserInput.Prompt("Press 'Enter' to add another task ('q' finish): ");
            if (input.ToLowerInvariant() is "q") break;
        }
    }
}