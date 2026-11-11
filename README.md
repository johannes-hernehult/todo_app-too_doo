# TooDoo

A lightweight, resilient console task manager built with C# and .NET. **TooDoo** helps you organize tasks by title, due date, and project category directly from your terminal—featuring crash protection, ANSI visual styling, and fault-tolerant JSON persistence.

---

## 📌 Key Features

- **Task Management:** Create, view, update, toggle, and delete tasks with validated fields (title, due date, and optional project category).
- **Flexible Views & Sorting:** Separate views for active vs. completed tasks, with dynamic sorting by **Due Date** or **Project**.
- **Atomic File Persistence:** Write-to-temp operations (`.tmp`) ensure your data file is never corrupted or left in a partial state if power cuts or an app crash occurs mid-save.
- **Automatic Corruption Recovery:** Damaged JSON data files are automatically backed up with a timestamp (`.corrupt-yyyyMMdd-HHmmss-fff`) during startup, allowing TooDoo to safely launch with a fresh state without losing the original file.
- **Backward-Compatible Schema:** Transparently reads legacy JSON data formats storing numeric status codes while migrating cleanly to boolean flags.
- **Readable Short IDs:** Generates 4-character cryptographic IDs using a custom alphabet that excludes easily confused characters (`0`/`O`, `1`/`I`).
- **ANSI Terminal Interface:** Styled console UI featuring color-coded due date indicators (overdue, due today, due yesterday) and alternate row shading.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later installed.

### Quick Start

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/TooDoo.git
   cd TooDoo
   ```

2. **Run the application:**
   ```bash
   dotnet run --project TooDoo
   ```

---

## 🏗️ Architecture & Data Flow

TooDoo follows a strict separation of concerns between domain entities, persistent repositories, and interactive views:

```text
[ Console Views ] <---> [ TaskService ] <---> [ TaskRepository ] <---> [ tasks.json ]
```

1. **Initialization:** `Program.cs` resolves the system-specific data folder via `StorageLocation` and initializes the repository and service layers.
2. **Data Loading:** `TaskService` loads saved JSON tasks through `TaskRepository`, which deserializes disk data into strongly-typed `TaskItem` domain objects.
3. **Menu Routing:** `App.cs` runs the main interactive loop, routing user actions to focused views (`ShowTasksView`, `AddTaskView`, `EditTasksView`).
4. **State & Persistence:** Views execute domain operations on `TaskService`. The service updates its in-memory task collection and triggers asynchronous disk writes.
5. **Fault Tolerant I/O:** If a disk write fails, changes remain safe in memory, a `SaveFailed` event alerts the user, and the app retries saving on subsequent changes or exit.

---

## 💻 Command & Navigation Reference

### Main Menu Options

| Option | Action |
| :--- | :--- |
| `(1) Show tasks` | Open active or completed task list view with sorting options. |
| `(2) Add new task` | Step-by-step creation prompt with project auto-suggestions. |
| `(3) Edit tasks` | Command-driven management view to edit, toggle, or delete tasks. |
| `(q) Quit` | Trigger final save check and exit application. |

### Interactive View Commands

Commands are entered directly into list views. Replace `<id>` with the displayed 4-character task ID.

| Command | Description | Available In |
| :--- | :--- | :--- |
| `toggle <id>` | Flip completion status (`Done` $\leftrightarrow$ `To-Do`). | Show tasks, Edit tasks |
| `edit <id>` | Prompt to update title, due date, or project (Press `Enter` to keep existing value, `q` to cancel, `-` to clear project). | Edit tasks |
| `delete <id>` | Permanently remove a task after user confirmation. | Edit tasks |
| `todo` | Switch to the active/incomplete tasks view. | Show tasks, Edit tasks |
| `done` | Switch to the completed tasks view. | Show tasks, Edit tasks |
| `sort` | Toggle list sorting between **Due Date** and **Project**. | Show tasks |
| `q` | Return to main menu. | All views |

---

## 📂 Repository Structure & Component Map

```text
TooDoo/
├── ConsoleUI/            # CLI utilities, menu logic, and table rendering
├── Models/               # Core domain entities, transfer records, enums
├── Services/             # Business logic, unique ID generation, file I/O
├── Views/                # Screen views, base list controllers, prompt handlers
├── App.cs                # Main menu loop & save failure listener
└── Program.cs            # App bootstrap & startup crash recovery
```

### 🟢 Application Bootstrap

| File | Responsibilities |
| :--- | :--- |
| `Program.cs` | Resolves data path, loads initial tasks, handles corrupt file backups, and starts `App.cs`. |
| `App.cs` | Runs main menu loop, builds live task counters, and displays non-intrusive save error alerts. |

### 📦 Models & Domain Rules

| File | Responsibilities |
| :--- | :--- |
| `TaskItem.cs` | Central domain model. Enforces non-empty titles, valid dates, and handles field updates. |
| `TaskDetails.cs` | Immutable record used for cleanly transferring user inputs from UI prompts to services. |
| `TaskSortOrder.cs` | Enum defining sorting strategies (`DueDate`, `Project`). |

### ⚙️ Services & Data Access

| File | Responsibilities |
| :--- | :--- |
| `TaskService.cs` | Owns the in-memory task collection, generates unambiguous IDs, handles filtering, and fires I/O error events. |
| `TaskRepository.cs` | Manages JSON serialization/deserialization, atomic `.tmp` file swapping, and corrupted file renaming. |
| `StorageLocation.cs` | Resolves system application data folder paths across OS platforms (Windows `%APPDATA%`, Linux `~/.config`). |

### 💻 Console UI & Views

| File | Responsibilities |
| :--- | :--- |
| `TaskListView.cs` | Abstract base class using the **Template Method pattern** to handle shared list commands (`todo`, `done`, `toggle`, `q`). |
| `ShowTasksView.cs` | Specialized list view handling list display and project/date sorting. |
| `EditTasksView.cs` | Specialized list view handling task editing and deletion confirmation. |
| `AddTaskView.cs` | Interactive creation flow supporting session history and multi-task entry loops. |
| `TaskDetailsPrompt.cs` | Reusable console prompter with default-keeping and project suggestions. |
| `TaskTableRenderer.cs` | Formats tasks into clean, aligned ASCII tables with dynamic ANSI color highlights. |
| `Colors.cs` / `Prompts.cs` | ANSI styling rules, background colors, error helpers, and input prompt utilities. |

---

## 🛠️ Design Patterns & Technical Highlights

1. **Template Method Pattern (`TaskListView.cs`):** Avoids DRY violations by putting standard list navigation (`todo`, `done`, `toggle`, `q`) in a base class while letting child views declare custom commands (`edit`, `delete`, `sort`).
2. **Atomic Disk Operations (`TaskRepository.cs`):** Writes task lists to a unique `.tmp` file prior to replacing the main `tasks.json`. If a system crash occurs mid-write, original disk data is preserved.
3. **Data DTO Isolation (`StoredTask`):** Keeps disk JSON structures decoupled from domain entities (`TaskItem`), enabling smooth backward compatibility for older JSON data formats without polluting business logic.
4. **Domain Guarding:** Inputs are sanitized and verified at the entity boundary (`TaskItem`), guaranteeing tasks can never exist in an invalid state.

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for details.
