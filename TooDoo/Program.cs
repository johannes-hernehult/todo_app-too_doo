using TooDoo;
using TooDoo.Services;

Console.WriteLine("TooDoo - Your personal todo console app");

var taskManager = new TaskService();

var today = DateOnly.FromDateTime(DateTime.Today);


taskManager.AddTask("Due next week", today.AddDays(7), "Testing");
taskManager.AddTask("Due today", today, "Testing");
taskManager.AddTask("Due yesterday", today.AddDays(-1), "Testing");
taskManager.AddTask("Due one week ago", today.AddDays(-7), "Testing");

var app = new App(taskManager);
app.Run();