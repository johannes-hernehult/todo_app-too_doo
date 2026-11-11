# TooDoo

A clean, console-based task management application written in C# and .NET. TooDoo enables users to track, organize, and persistent-store daily tasks through an intuitive terminal interface.

---

## 📌 Features & Purpose

* **Task Lifecycle Management:** Create tasks with titles, due dates, and optional project categories.
* **Status Tracking:** Mark tasks as completed or incomplete, edit existing details, or remove tasks.
* **Sorting & Views:** View tasks sorted by due date or project.
* **Fault-Tolerant Persistence:** Task data is held in memory during execution and automatically saved to a local JSON file. If file writing fails, active session data is preserved while notifying the user.
* **Automatic Backups:** Corrupted or unreadable data files are safely backed up with timestamps during startup, allowing the app to launch with a fresh state without data destruction.

---

## 🏗️ Architecture & High-Level Data Flow

TooDoo follows a clear separation of concerns across **Models**, **Services**, and **Console Views**:

```text
[ Terminal Views ] <---> [ TaskService ] <---> [ TaskRepository ] <---> [ tasks.json ]

```

1. **Initialization:** `Program.cs` resolves the local file storage path via `StorageLocation` and initializes `TaskRepository` and `TaskService`.
2. **Data Loading:** `TaskService` loads saved tasks asynchronously. `TaskRepository` parses the JSON file and converts entries into strongly-typed `TaskItem` objects.
3. **Menu Routing:** `App.cs` executes the interactive main menu loop and delegates operations to target views (`ShowTasksView`, `AddTaskView`, `EditTasksView`).
4. **Operations & State:** Views gather user input and invoke domain operations on `TaskService`. The service updates its in-memory task collection and triggers asynchronous persistence via `TaskRepository`.
5. **Error Recovery:** If a file write fails, task modifications remain in memory and the UI alerts the user (`SaveFailed` event). Subsequent operations or exiting the application will attempt a save retry.

---

## 📁 Repository Structure & Application Components

### 🟢 Application Entry & Lifecycle

| Component | File | Description |
| --- | --- | --- |
| **Entry Point** | `Program.cs` | Resolves storage path (`GetDataFilePath`), handles startup data loading (`TryLoadTasksAsync`), backs up corrupted task files (`TryStartWithEmptyList`), and boots `App.cs`. |
| **App Orchestrator** | `App.cs` | Manages the main menu workflow (`RunAsync`), displays live task counters (`BuildTaskSummary`), and handles save failure notifications (`ReportSaveFailure`). |

### 📦 Domain Models

| Component | File | Description |
| --- | --- | --- |
| **`TaskItem`** | `TaskItem.cs` | Core domain entity representing a task. Enforces business rules (non-blank titles, required due dates, text normalization) and handles state changes (`Update`, `ToggleDone`). |
| **`TaskDetails`** | `TaskDetails.cs` | Immutable record carrying user input (`Title`, `DueDate`, `Project`) for creating and editing tasks. |
| **`TaskSortOrder`** | `TaskSortOrder.cs` | Enum defining task sorting strategies (`DueDate`, `Project`). |

### ⚙️ Services & Data Persistence

| Component | File | Description |
| --- | --- | --- |
| **`TaskService`** | `TaskService.cs` | Core business layer holding the active task list in memory. Manages task filtering, unique short-ID generation, project name querying, CRUD operations, and dispatches `SaveFailed` events on I/O errors. |
| **`TaskRepository`** | `TaskRepository.cs` | Data access layer reading and writing tasks to JSON. Uses temporary file writes prior to replacement for atomic saves, handles legacy file format compatibility, and creates timestamps for unreadable files (`BackUpUnreadableFile`). |
| **`StorageLocation`** | `StorageLocation.cs` | Utility class determining the cross-platform system file path (`GetDataFilePath`) where JSON task data is stored. |

### 💻 User Interface & Console Views

#### UI Helpers & Formatting

* **`ConsoleMenu.cs`**: Renders menu choices and captures interactive user selections.
* **`Prompts.cs`**: Handles command-line input collection and pause-to-continue interactions.
* **`Colors.cs`**: Manages terminal color output for visual hierarchy and status indicators.
* **`ConsoleFormats.cs`**: Formatting utilities for headers, dates, and status text.
* **`TaskTableRenderer.cs`**: Renders task lists into structured ASCII terminal tables.

#### Interactive Views

* **`AddTaskView.cs`**: Handles user interactions for prompting task details and adding new tasks.
* **`EditTasksView.cs`**: Coordinates task selection for updating fields, toggling completion, or deleting items.
* **`ShowTasksView.cs`**: Displays filtered and sorted task lists.
* **`TaskListView.cs`**: Supporting UI component for task list navigation and selection.
* **`TaskDetailsPrompt.cs`**: Standardized console input prompter for collecting title, due date, and project information.

---

* **`EditTasksView.cs`**: Coordinates task selection for updating fields, toggling completion, or deleting items.
* **`ShowTasksView.cs`**: Displays filtered and sorted task lists.
* **`TaskListView.cs`**: Supporting UI component for task list navigation and selection.
* **`TaskDetailsPrompt.cs`**: Standardized console input prompter for collecting title, due date, and project information.

### Command Overview

Commands are entered in the task-list views. Replace `<id>` with the task's displayed ID.

| View | Commands and interaction |
| --- | --- |
| **Add task** (`AddTaskView`) | Follow the prompts to enter the title, due date, and optional project. After adding a task, enter `a` to add another or `q` to return to the main menu. |
| **Show tasks** (`ShowTasksView`) | `toggle <id>` — mark a task done or not done; `todo` — show incomplete tasks; `done` — show completed tasks; `sort` — switch between due-date and project order; `q` — return to the main menu. |
| **Edit tasks** (`EditTasksView`) | `edit <id>` — edit a task's details; `delete <id>` — delete a task after confirmation; `toggle <id>` — mark a task done or not done; `todo` — show incomplete tasks; `done` — show completed tasks; `q` — return to the main menu. |

---

## 📐 Layer Responsibilities Summary

| Layer | Primary Responsibilities | Components |
| --- | --- | --- |
| **Models** | Entity validation, normalization, immutable transfer objects | `TaskItem`, `TaskDetails`, `TaskSortOrder` |
| **Services** | Business logic, task sorting, ID generation, JSON file persistence | `TaskService`, `TaskRepository`, `StorageLocation` |
| **Console UI** | Terminal rendering, menu navigation, input prompts, table formatting | `App`, `ConsoleMenu`, Views, Renderers |
