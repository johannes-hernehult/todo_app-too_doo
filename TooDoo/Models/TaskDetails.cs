namespace TooDoo.Models;

/// <summary>The user-editable fields of a task.</summary>
public sealed record TaskDetails(string Title, DateOnly DueDate, string? Project);
