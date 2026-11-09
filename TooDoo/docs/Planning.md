# Project Plan

## Requirements

### Must have

#### Tasks

- Fields:
    - TaskID: auto-generated, 5 random digits & a-Z (regenerate on collision)
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
- `m <task ID>`: toggle DONE / NOT_DONE
- Empty input: back to menu

**(2) Add new task**

- Prompt for title, due date, project (optional, list ot all existing projects)
- Prompt to add another todo, showing an overview of tasks added this session
- or to go Back to menu

**(3) Edit tasks**

- Shows all tasks with IDs, then accepts `<command> <TaskID>`:
    - `u <TaskID>`: update, reusing the add flow with current values prefilled
        - Enter keeps the current value
        - `-` clears the project as it is optional
    - `r -a` / `r <TaskID>`: Reschedule all late task to today or just one by ID
    - `m <taskID>`: toggle DONE / NOT_DONE
    - `x <TaskID>`: delete (asks for confirmation)
    - Empty input: back to menu

**(4) Save and quit**

- Initially only save here
- Add incremental saving when its done and if there is time

## Classes & Methods

- UI
    - RenderMenu
    - RenderHeading
    - RenderText
    - RenderList
    -
- UserInput
    - Prompt
        - Move to UI ? Call it RenderPrompt ??
- Validate
    - Validate string input
    - Validate int input
    - Validate DateOnly input
    -
- Task
    - Id
    - Title
    - Status (enum)
    - DueDate
    - Project?
    -
- TaskStatus (enum)
  - DONE
  - NOT_DONE
- FileService
  - Save
  - Load
- TaskService
  - private List<Task> _tasks
  - Create
  - Read
  - Update
  - Delete

```mermaid
classDiagram
    
````
