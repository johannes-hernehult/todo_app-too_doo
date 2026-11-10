# TooDoo

A simple console-based task manager for keeping track of tasks, due dates, and projects. TooDoo lets you add and edit tasks, mark them complete, and view them sorted by due date or project.

## Tech Stack

* **C# 14**
* **.NET 10**
* Console application
* JSON file storage using `System.Text.Json`

## Features

* **View tasks**, including completed and incomplete tasks
* **Add tasks** with a title, due date, and optional project
* **Edit and delete** tasks
* **Mark tasks** complete or incomplete
* **Sort tasks** by due date or project
* **Assign short, readable IDs** to tasks
* **Save task data** as JSON between runs
* **Migrate existing task data** from the older project-local location
* **Preserve damaged data** by moving an unreadable file aside before starting with an empty list

## Folder Structure

```text
TooDoo/
├── App.cs
├── Program.cs
├── TooDoo.csproj
├── ConsoleUI/
│   ├── Colors.cs
│   ├── ConsoleFormats.cs
│   ├── ConsoleMenu.cs
│   ├── Prompts.cs
│   └── TaskTableRenderer.cs
├── Data/
│   └── tasks.json
├── Models/
│   ├── TaskDetails.cs
│   ├── TaskItem.cs
│   └── TaskSortOrder.cs
├── Services/
│   ├── StorageLocation.cs
│   ├── TaskRepository.cs
│   └── TaskService.cs
└── Views/
    ├── AddTaskView.cs
    ├── EditTasksView.cs
    ├── ShowTasksView.cs
    ├── TaskDetailsPrompt.cs
    └── TaskListView.cs
```

## How to Run

### Requirements
* **.NET 10 SDK**

### Execution Commands

From the repository root, run:

```bash
dotnet run --project "TooDoo/TooDoo.csproj"
```

If the repository root is already the `TooDoo` project directory, run:

```bash
dotnet run
```

> **Note:** On first launch, TooDoo starts with an empty task list if no saved data exists. Tasks are stored as JSON in the user's application-data directory.

## Screenshots

*(Add your screenshots here)*
