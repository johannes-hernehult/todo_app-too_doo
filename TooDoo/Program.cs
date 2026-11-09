using TooDoo;
using TooDoo.Services;
using TaskStatus = TooDoo.Models.TaskStatus;

Console.WriteLine("TooDoo - Your personal todo console app");

var taskManager = new TaskService();

var today = DateOnly.FromDateTime(DateTime.Today);


taskManager.AddTask("Audit test coverage, with long ass title to showcase how it handles this..", today.AddDays(-1), "Testing", TaskStatus.Done);
taskManager.AddTask("Build regression suite", today, "Testing");
taskManager.AddTask("Document test setup", today.AddDays(7), "Testing");

taskManager.AddTask("Implement landing page", today.AddDays(-5), "Website");
taskManager.AddTask("Optimize page loading", today.AddDays(-1), "Website");
taskManager.AddTask("Review accessibility", today.AddDays(-2), "Website", TaskStatus.Done);

taskManager.AddTask("Clean out workspace", today.AddDays(-1), "Personal", TaskStatus.Done);
taskManager.AddTask("Plan weekly meals", today.AddDays(5), "Personal");
taskManager.AddTask("Schedule dentist visit", today.AddDays(14), "Personal");

var app = new App(taskManager);
app.Run();