# TooDoo

A console task manager written in C# and .NET. Track tasks by title, due date, and project, right from your terminal.

<img width="929" height="360" alt="image" src="https://github.com/user-attachments/assets/ab77c5da-d894-4b9f-bb29-edc2af473e98" />


## Features

- Add, edit, delete, and mark tasks done, with validated input
- Separate to-do and completed lists, sortable by due date or project
- Color-coded due dates (today, yesterday, overdue) and alternating row shading
- Saves automatically after every change
- Protects your data: a crash mid-save can't leave a half-written file, and a corrupt file is moved aside, never overwritten

## Getting started

**Requires:** [.NET 10 SDK](https://dotnet.microsoft.com/download) and a terminal with ANSI color support, at least 90 columns wide.

```bash
git clone https://github.com/johannes-hernehult/todo_app-too_doo.git
cd todo_app-too_doo
dotnet run --project TooDoo
```

Tasks are stored in `tasks.json` in your app data folder (`%APPDATA%\TooDoo` on Windows, `~/.config/TooDoo` on Linux).

## Usage

Pick an option from the main menu: show tasks, add a task, edit tasks, or quit. Inside a list, type a command:

| Command | What it does | Where |
| :--- | :--- | :--- |
| `toggle <id>` | Mark a task done or not done | Show, Edit |
| `edit <id>` | Change title, due date, or project (Enter keeps a value, `-` clears the project) | Edit |
| `delete <id>` | Delete a task after confirmation | Edit |
| `todo` / `done` | Switch between the to-do and completed lists | Show, Edit |
| `sort` | Switch between due date and project order | Show |
| `q` | Go back | All |

Every task has a short 4-character ID. The alphabet leaves out `0`, `O`, `1`, and `I`, so IDs are easy to read and type.

## Project structure

```text
TooDoo/
├── ConsoleUI/   Menu, prompts, colors, and the table renderer
├── Models/      TaskItem, TaskDetails, TaskSortOrder
├── Services/    TaskService (logic), TaskRepository (JSON file), StorageLocation
├── Views/       Screens: show, add, edit
├── App.cs       Main menu loop
└── Program.cs   Startup and corrupt-file recovery
```

Data flows in one direction:

```text
Views  →  TaskService  →  TaskRepository  →  tasks.json
```

Views never touch the file. `TaskService` owns the task list and saves after each change. `TaskRepository` is the only class that reads or writes JSON.

## Design notes

- **Safe saving.** `TaskRepository` writes to a temporary file and then swaps it in with `File.Move`. If the app crashes mid-write, the original file is untouched.
- **Corruption recovery.** If `tasks.json` can't be read at startup, it is renamed to `tasks.json.corrupt-<timestamp>` and TooDoo starts with an empty list. Your original data stays recoverable.
- **Template Method.** `TaskListView` handles the commands every list shares (`toggle`, `todo`, `done`, `q`). `ShowTasksView` and `EditTasksView` add only their own.
- **Table as data.** `TaskTableRenderer` describes each column as a header, a width, and a function. Adding a column is one line.
- **Validation at the model.** `TaskItem` checks titles and dates in its constructor and in `Update`, so a bad value never leaves a task half-updated.

## Limitations

- The whole file is rewritten on every change. That is fine for a personal list, but not for thousands of tasks.
- Running two copies at once is not supported; the last save wins.
- Saves are crash-safe but do not force data to disk, so a sudden power loss can still lose the latest change.

## About

An individual project. The first version was working but tangled: UI, logic, and storage were mixed together. It was then refactored into the layers above.
