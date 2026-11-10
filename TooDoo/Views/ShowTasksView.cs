using TooDoo.Models;
using TooDoo.Services;

namespace TooDoo.Views;

public class ShowTasksView(TaskService taskService) : TaskListView(taskService)
{
    private TaskSortOrder _sortOrder = TaskSortOrder.DueDate;

    protected override TaskSortOrder SortOrder => _sortOrder;

    protected override string Heading =>
        $"{(ShowCompleted ? "Completed tasks" : "Tasks to do")} | Sorted by {DescribeSortOrder()}";

    protected override string CommandLegend =>
        "toggle <id> (done/not done) | todo, done (switch list) | sort (change order) | q (back)";

    protected override Task<bool> TryHandleCommandAsync(
        string command,
        string[] arguments,
        IReadOnlyList<TaskItem> visibleTasks)
    {
        if (command != "sort")
        {
            return Task.FromResult(false);
        }

        _sortOrder = _sortOrder == TaskSortOrder.DueDate
            ? TaskSortOrder.Project
            : TaskSortOrder.DueDate;

        return Task.FromResult(true);
    }

    private string DescribeSortOrder()
    {
        return _sortOrder == TaskSortOrder.DueDate ? "due date" : "project";
    }
}
