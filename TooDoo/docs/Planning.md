# Project Plan

## Requirements

### Must have

#### Tasks
- Fields:
    - TaskID: auto-generated, 5 random digits (regenerate on collision)
    - Title: string
    - Due date: DateTime
    - Status: `TaskStatus` enum (`NOT_DONE` default, `DONE`)
    - Project: optional
- Operations: create, edit, complete, delete

#### Viewing
- Sort by date or by project
- Overdue coloring (based on due date):
    - White: upcoming
    - Green: due today
    - Yellow: due yesterday
    - Red: more than 1 day overdue
    - Done tasks are never colored as overdue
- Handle empty state (no tasks)

#### Persistence
- Save to a JSON file on exit, restore on start
- Handle missing and corrupt files

#### Input validation
- Strings, ints, dates

### Nice to have
- Incremental saving after every add, edit, complete, delete
- Natural-language date input:
    - `today` = today
    - `tomorrow` = today + 1 day
    - `next week` = first Monday after today
    - Day name = first occurrence of that day after today

## Menu flow

Main menu shows task counts:

```
You have X tasks to do and Y tasks done!
Pick an option:
(1) Show tasks
(2) Add new task
(3) Edit tasks
(4) Save and quit
```

**(1) Show tasks**
- `d`: sort by date
- `p`: sort by project
- `e <TaskID>`: edit that task, then return to the list with the same sorting
- Empty input: back to menu

**(2) Add new task**
- Prompt for title, due date, project (optional)
- Offer to add another, showing an overview of tasks added this session
- Back to menu

**(3) Edit tasks**
- Shows all tasks with IDs, then accepts `<command> <TaskID>`:
    - `u <TaskID>`: update, reusing the add flow with current values prefilled
        - Enter keeps the current value
        - `-` clears the project
    - `m <TaskID>`: mark as done
    - `x <TaskID>`: delete (asks for confirmation)

**(4) Save and quit**

## Classes

| Class | Responsibility |
|---|---|
| `Task` | Data model |
| `TaskStatus` | Enum: `NOT_DONE`, `DONE` |
| `TaskManager` | Task logic: add, edit, complete, delete, sort |
| `FileStorage` | Save and load JSON |
| `App` | Application flow: menus, prompts, and coordinating task operations |
| `UI` | Reusable display helpers: render text, task lists, counts, and tables |
| `UserInput` | Prompts that loop until valid |
| `Validate` | Pure checks (valid date, valid int, etc.) |

```mermaid
classDiagram
  class Task {
    +int Id
    +string Title
    +DateOnly DueDate
    +TaskStatus Status
    +string? Project
  }

  class TaskStatus {
    <<enumeration>>
    NotDone
    Done
  }

  class TaskSort {
    <<enumeration>>
    Date
    Project
  }

  class TaskManager {
    -List~Task~ _tasks
    +GetAll() IReadOnlyList~Task~
    +GetSorted(sort: TaskSort) IReadOnlyList~Task~
    +Add(title: string, dueDate: DateOnly, project: string?) Task
    +Edit(id: int, title: string, dueDate: DateOnly, project: string?) bool
    +MarkDone(id: int) bool
    +Delete(id: int) bool
    +GetCounts() (int todo, int done)
  }

  class FileStorage {
    +Load() List~Task~
    +Save(tasks: IReadOnlyList~Task~) void
  }

  class App {
    -TaskManager _taskManager
    -FileStorage _storage
    -UserInput _input
    -UI _ui
    +Run() void
    -RunMainMenu() void
    -ShowTasks() void
    -AddTask() void
    -EditTasks() void
    -SaveAndQuit() void
  }

  class UserInput {
    +ReadString(prompt: string, allowEmpty: bool) string?
    +ReadDate(prompt: string) DateOnly
    +ReadCommand() string
  }

  class UI {
    +RenderMainMenu(todo: int, done: int) void
    +RenderTasks(tasks: IReadOnlyList~Task~) void
    +RenderMessage(message: string) void
  }

  Task --> TaskStatus
  TaskManager --> TaskSort
  TaskManager "1" *-- "0..*" Task : manages
  App --> TaskManager
  App --> FileStorage
  App --> UserInput
  App --> UI
````
