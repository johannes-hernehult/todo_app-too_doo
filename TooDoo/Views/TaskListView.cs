using System.Diagnostics.CodeAnalysis;
using TooDoo.ConsoleUI;
using TooDoo.Models;
using TooDoo.Services;

namespace TooDoo.Views;

/// <summary>
/// A screen that lists tasks and accepts typed commands. Handles the commands every list
/// shares (toggle, todo, done, q); subclasses add their own.
/// </summary>
public abstract class TaskListView(TaskService taskService)
{
    protected TaskService Tasks { get; } = taskService;

    /// <summary>True while the completed list is shown instead of the to-do list.</summary>
    protected bool ShowCompleted { get; private set; }

    protected abstract string Heading { get; }
    protected abstract string CommandLegend { get; }
    protected abstract TaskSortOrder SortOrder { get; }

    /// <returns>True if the command belongs to this view, even if it then failed.</returns>
    protected abstract Task<bool> TryHandleCommandAsync(
        string command,
        string[] arguments,
        IReadOnlyList<TaskItem> visibleTasks);

    public async Task ShowAsync()
    {
        while (true)
        {
            IReadOnlyList<TaskItem> visibleTasks = RenderScreen();

            string input = Prompts.ReadLine("Command: ");

            if (Prompts.IsCancel(input))
            {
                return;
            }

            if (input.Length > 0)
            {
                await HandleCommandAsync(input, visibleTasks);
            }
        }
    }

    /// <summary>Finds the task named by a "command /<taskID/>" input, or explains why not.</summary>
    protected static bool TryFindTask(
        string command,
        string[] arguments,
        IReadOnlyList<TaskItem> visibleTasks,
        [NotNullWhen(true)] out TaskItem? task)
    {
        task = null;

        if (arguments.Length != 1)
        {
            Prompts.Pause($"Usage: {command} <taskID>");
            return false;
        }

        string id = arguments[0];
        task = visibleTasks.FirstOrDefault(visibleTask =>
            visibleTask.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

        if (task is null)
        {
            Prompts.Pause($"No task with ID '{id}' in this list.");
        }

        return task is not null;
    }

    private IReadOnlyList<TaskItem> RenderScreen()
    {
        Console.Clear();

        IReadOnlyList<TaskItem> tasks = Tasks.GetTasks(ShowCompleted, SortOrder);
        string emptyMessage = ShowCompleted ? "No completed tasks." : "No tasks to do.";

        ConsoleMenu.WriteHeading(Heading);
        TaskTableRenderer.Render(tasks, TaskColumns.All, emptyMessage);

        Console.WriteLine();
        Console.WriteLine(Colors.Paint($"Commands: {CommandLegend}", Colors.Muted));

        return tasks;
    }

    private async Task HandleCommandAsync(string input, IReadOnlyList<TaskItem> visibleTasks)
    {
        string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string command = words[0].ToLowerInvariant();
        string[] arguments = words[1..];

        switch (command)
        {
            case "todo":
                ShowCompleted = false;
                return;

            case "done":
                ShowCompleted = true;
                return;

            case "toggle":
                if (TryFindTask(command, arguments, visibleTasks, out TaskItem? task))
                {
                    await Tasks.ToggleDoneAsync(task.Id);
                }

                return;
        }

        if (!await TryHandleCommandAsync(command, arguments, visibleTasks))
        {
            Prompts.Pause($"Unknown command '{command}'.");
        }
    }
}
