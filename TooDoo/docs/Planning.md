# Project Plan

## TODO

### 1. Task operations (`TaskService`)

- [ ] Edit task (title, due date, project)
- [ ] Toggle completion status
- [ ] Delete task
- [ ] Reschedule one task / all overdue tasks to today

### 2. Input

- [ ] Natural-language due dates in `Validate` ("today", "tomorrow", "next week", day names)
- [ ] Reuse validation helpers for all prompts

### 3. Persistence

- [ ] Implement `FileService` (save to JSON)
- [ ] Load tasks at startup
- [ ] Handle missing and corrupt files
- [ ] Save on exit
- [ ] Decide on incremental saving after add/edit/complete/delete

### 4. Console workflows

- [ ] (2) Add new task: title, due date, optional project (show existing projects), add another or return
- [ ] (3) Edit tasks: `u`, `r`, `m`, `x` commands with confirmation on delete
- [ ] (1) Show tasks: sorting by date (`d`) and project (`p`), `m <TaskID>` toggle
- [ ] Task counts above the main menu
- [ ] (0) Save and quit

### 5. Display

- [X] Due-date coloring (white / green / yellow / red; completed never overdue)
- [ ] Keep empty-state message

### 6. Cleanup and tests

- [ ] Remove hard-coded sample tasks from `Program`
- [ ] Tests: ID generation
- [ ] Tests: validation
- [ ] Tests: task operations
- [ ] Tests: persistence

## Current Implementation

Tasks are represented by `TaskItem`:

- **ID:** 4 random characters from 0-9 and A-Z; collisions are checked against in-memory tasks.
- **Title:** required string.
- **Due date:** required `DateOnly`.
- **Status:** `TaskStatus.NotDone` by default, or `TaskStatus.Done`.
- **Project:** optional; blank values are normalized to `null`.

`TaskService` can add tasks and return the current task list.

The console menu has options to show tasks, add tasks, edit tasks, and save and quit:

- Showing tasks lists ID, title, due date, project, and status, and handles the empty state.
- Add and edit flows are placeholders.
- Quitting does not yet save tasks.

`Validate` provides required-string, integer, and date validation helpers.

`Program` currently creates sample tasks at startup.

## Remaining Requirements

### Tasks

Keep the current task fields and conventions:

- **ID:** 4 random uppercase alphanumeric characters; regenerate on collision.
- **Title:** required string.
- **Due date:** `DateOnly`.
- **Status:** `TaskStatus` (`NotDone` by default, `Done`).
- **Project:** optional.

Implement task operations:

- Create tasks through the console.
- Edit title, due date, and project.
- Toggle completion status.
- Delete tasks with confirmation.

### Viewing

- Show task counts in the main menu.
- Add sorting by due date and project.
- Add due-date coloring:
  - **White:** upcoming.
  - **Green:** due today.
  - **Yellow:** due yesterday.
  - **Red:** more than one day overdue.
  - Completed tasks are never colored as overdue.
- Keep the existing empty-state message.

### Persistence

- Implement `FileService` to save tasks to JSON and load them at startup.
- Save on exit and handle missing or corrupt files.
- Consider incremental saving after add, edit, complete, or delete.

### Input

- Use the existing validation helpers for required strings, integers, and dates.
- Add natural-language due-date input:
  - "today" means today.
  - "tomorrow" means tomorrow.
  - "next week" means the first Monday after today.
  - A day name means the first occurrence of that day after today.

## Menu Flow

### Main Menu

```
(1) Show tasks
(2) Add new task
(3) Edit tasks
(0) Save and quit
```

Show task counts above the menu once task management is implemented.

### (1) Show Tasks

- List tasks with ID, title, due date, project, and status.
- Support sorting by date (`d`) and project (`p`).
- Support toggling completion (`m <TaskID>`).
- Empty input returns to the menu.

### (2) Add New Task

- Prompt for title and due date.
- Prompt for an optional project; show existing project names where practical.
- Allow adding another task or returning to the menu.

### (3) Edit Tasks

Show tasks with their IDs, then accept a command and task ID:

- `u <TaskID>`: update task details, prefilled with current values. Enter keeps a value; `-` clears the optional project.
- `r -a` or `r <TaskID>`: reschedule all overdue tasks or one task to today.
- `m <TaskID>`: toggle completion.
- `x <TaskID>`: delete after confirmation.
- Empty input returns to the menu.

### (0) Save and Quit

- Save tasks before exiting.

## Classes and Responsibilities

| Class | Responsibility |
|---|---|
| `App` | Main menu and console workflows. |
| `UI` | Render menus, headings, text, and lists. |
| `UserInput` | Prompt for user input. |
| `Validate` | Validate required strings, integers, and dates; extend for natural-language dates if needed. |
| `TaskItem` / `TaskStatus` | Store and validate task data. |
| `TaskService` | Manage the in-memory task list and task operations. |
| `FileService` | Load and save task data as JSON. |

## Development Cleanup

- Remove the hard-coded sample tasks from `Program` once normal task entry and persistence are in place.
- Add tests for ID generation, validation, task operations, and persistence.