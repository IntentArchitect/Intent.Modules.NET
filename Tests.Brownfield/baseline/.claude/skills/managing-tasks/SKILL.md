---
description: How to create and manage Intent Architect's runnable tasks — the `tasks.json` files behind the Tasks toolbar and the `run_task` tool. Covers the two scopes (the solution's tasks.json at the workspace root, and an application's own), the full schema, compound and background/watch tasks, error parsers and AI auto-fix, and how to verify a task actually runs. Load before adding, changing, or removing a task, or when asked to "set up a build/test/watch task".
short-description: Create and manage the tasks.json files behind the Tasks toolbar and run_task.
intent-updates: automatic
requiredTools:
  - run_task
  - read_file
  - write_file
  - patch_file
group: Documents, preview and tasks
---

# Managing Tasks

A **task** is a named command the user can run from the Tasks control in the toolbar and you can run with `run_task`. Tasks are declared in `tasks.json` files — plain configuration, nothing generated, nothing in the designer model. Editing one is an ordinary file edit with `read_file` / `write_file` / `patch_file`.

Do this work yourself; there is no model change and nothing for the Software Factory to generate.

## Two scopes, two files

| Scope           | File                                        | Holds                                                                                                |
| --------------- | ------------------------------------------- | ---------------------------------------------------------------------------------------------------- |
| **Solution**    | `<workspaceRoot>/tasks.json`                | Tasks that belong to the repository as a whole — a full build, the test suite, a dev-server watcher. |
| **Application** | `<application's tasksDirectory>/tasks.json` | Tasks that belong to one application — build just it, run just its tests.                            |

Both appear together in the solution shell's Tasks strip (solution tasks first, then the active application's, with a divider between). In the **Agents shell** only the solution scope exists — a conversation there is pointed at a folder, not an application — so a task you want available to every agent conversation in a repository belongs in the solution file.

**Get both paths from the workspace context, never by guessing:**

- The workspace root is the `workspaceRoot` on any `availableTasks` entry whose `scope` is `solution`. When there are none yet, it is the repository root the conversation runs in — the `codebaseRoots` entry with `isWorkspaceFolder: true`, or the folder the shell tools resolve `.` to. Confirm with the shell (`ls <root>`) before writing.
- An application's directory is that application's `tasksDirectory` in `applications`. It is **not** its `outputLocation` — those differ in most solutions, and writing to the output location produces a file nothing reads.

**Pick the scope deliberately, and say which you chose.** A command that runs at the repository root (`dotnet build MySolution.sln`, `npm test` in a single-package repo) is a solution task. A command scoped to one application's folder is an application task. When it is genuinely ambiguous, ask — the wrong file is invisible until someone wonders why the button isn't where they expected.

## The schema

`tasks.json` is a single object with one `tasks` map. Keys are task names; each name must be unique **within its file** (the same name in both scopes is fine — they stay separate buttons). `//` and `/* */` comments and trailing commas are accepted.

```json
{
    "tasks": {
        "build": {
            "label": "Build",
            "icon": "fa-cogs",
            "command": "dotnet build",
            "workingDirectory": ".",
            "aiAutoFix": { "enabled": false, "errorParser": "dotnet" }
        }
    }
}
```

| Field                  | Meaning                                                                                                                                                                                |
| ---------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `label`                | What the button says. Keep it 1–2 words — the strip is horizontal and must not wrap.                                                                                                   |
| `command`              | The command line. Required, **except** on a compound task (one with `dependsOn`).                                                                                                      |
| `icon`                 | A Font Awesome 4 class, e.g. `fa-cogs`, `fa-flask`, `fa-play`, `fa-eye`. Defaults to `fa-terminal` (`fa-tasks` for a compound). Only the idle icon — status replaces it while running. |
| `workingDirectory`     | Relative to **this file's own folder**: the workspace root for a solution task, the application's folder for an application task. Defaults to `.`.                                     |
| `hidden`               | `true` keeps the task off the toolbar while leaving it runnable as a compound's child and by `run_task`.                                                                               |
| `aiAutoFix`            | `{ "enabled": true, "errorParser": "dotnet" \| "generic" }` — on failure, starts a conversation to fix it. See the limits below.                                                       |
| `dependsOn`            | Child task names — makes this a **compound** task.                                                                                                                                     |
| `dependsOrder`         | `"parallel"` (default) or `"sequence"`.                                                                                                                                                |
| `isBackground`         | `true` for a command that never exits (a watcher).                                                                                                                                     |
| `readyPattern`         | Regex over a background task's output; the first match means "up and watching".                                                                                                        |
| `errorPattern`         | Regex marking a background task's build as failed.                                                                                                                                     |
| `beginsPattern`        | Regex marking the start of a background task's (re)build cycle.                                                                                                                        |
| `producesTestCoverage` | `true` marks this as the task the Changes Review runs to measure test coverage. See below.                                                                                             |

### The test-coverage task

The Changes Review shows how much of a change its tests reach, read from a Cobertura report under a `TestResults` folder. It needs to be told which task writes one — a plain `dotnet test` writes none — so exactly one **solution** task opts in:

```json
"test:coverage": {
    "label": "Test",
    "icon": "fa-flask",
    "command": "dotnet test --collect:\"XPlat Code Coverage\"",
    "producesTestCoverage": true
}
```

- **Solution scope only.** Coverage is measured per repository, so the flag is read from the workspace root's `tasks.json`; setting it on an application task does nothing.
- The command must actually leave a report: `--collect:"XPlat Code Coverage"` for .NET, `--coverage` with the cobertura reporter for a jest suite. A compound works — the review watches its children's exits too.
- With no flagged task the review offers to start a conversation to add one, so a wrong command here is worse than none.

**It must be self-sufficient in a clean checkout, and it must use only RELATIVE paths.** To measure a commit nobody has checked out — a pull request's head — the review runs this task in an isolated worktree of that commit, which has never been built in. Two rules follow, and neither is enforced for you:

- **Assume no prior build.** A bare `dotnet test` or `npx jest` that relies on a warm tree fails there. Make it a compound that `dependsOn` the restore/install (and build, if the suite needs one) rather than assuming they have happened.
- **Never name an absolute path, and never pass the solution/project explicitly.** `dotnet test C:\repo\Foo.sln` ignores its working directory outright: it would measure the MAIN tree, write its report under the main repo, contaminate the working-tree measurement, _and_ be filed as the pull request's coverage. Likewise an absolute `workingDirectory` wins over the override. Write the command as if the repository root is the only thing you know — `dotnet test` with no path argument, `npx jest --coverage`, a `workingDirectory` that is relative or absent.

The run fails loudly if no report is found under the worktree afterwards, so the dangerous version of this mistake is caught — but a task that merely can't cold-start just wastes a checkout and a build.

### Compound tasks

`dependsOn` launches several tasks in one container tab with a sub-tab per child.

```json
"watch all": {
    "label": "Watch all",
    "dependsOn": ["watch web", "watch api"],
    "dependsOrder": "parallel"
}
```

- A compound resolves `dependsOn` **within its own file only**. A solution `watch all` cannot name an application's task, and vice versa — the child must live beside it. If you need both, define the child in each file or promote the whole group to the solution scope.
- **Nesting is supported**: a child may itself have its own `dependsOn`, to any depth. A dependency reached from more than one place (e.g. three watchers that all `dependsOn: ["install"]`) resolves to a single run — it starts once and every dependent waits on that same run, not three separate installs racing each other. A genuine cycle (`a` depends on `b` depends on `a`) is rejected with an error naming the cycle, rather than deadlocking.
- Every task reached via `dependsOn` (at any depth) needs a `command` — except the top-level compound itself, which may omit one to be a pure grouping alias (see `watch all` above).
- A compound may ALSO have a `command` of its own; it then runs **after** every `dependsOn` child has completed, and is skipped if any of them failed (VS Code's `dependsOn`-then-run semantics). This applies at every level, not just the top: a nested task with both `dependsOn` and a `command` runs after ITS OWN dependencies, then unblocks whatever depends on it in turn.
- Mark children `"hidden": true` when they exist only to be run by the compound (their own, or an ancestor's).

### Background / watch tasks

A watcher never exits, so it needs to say when it is _ready_ rather than _done_. Without a `readyPattern` a background task counts as ready the moment it spawns — which means a `sequence` advances past it before it has built anything, and `run_task` reports success for a build that hasn't happened.

```json
"watch web": {
    "label": "Watch web",
    "command": "npx tsc -w",
    "isBackground": true,
    "readyPattern": "Found \\d+ errors?\\. Watching for file changes",
    "errorPattern": "error TS",
    "beginsPattern": "Starting incremental compilation",
    "hidden": true
}
```

- `readyPattern` gates `sequence` advancement and is what `run_task` waits for.
- `errorPattern` surfaces errors as soon as they appear — including _before_ the task first becomes ready, so a watcher whose initial build fails shows as erroring rather than starting forever.
- `beginsPattern` scopes errors to one build cycle, which is what lets a watcher recover after a failed build. Without it a watcher that once failed cannot tell a later successful build from the old one and stays flagged. **Set it whenever you set `errorPattern`.**
- Patterns are JSON strings holding regexes, so backslashes are doubled (`\\d`, `\\.`). Take the pattern from the tool's **real output** — run the command once with the shell tools and copy a line from it. A guessed pattern that never matches is the single most common way a watch task ends up permanently "starting".

### Error parsers and AI auto-fix

`aiAutoFix.errorParser` picks how failure output is condensed for the tooltip and for a fix conversation: `"dotnet"` keeps MSBuild `error CODE:` diagnostics, `"generic"` applies a broad heuristic. The parser only ever _adds_ a hint — `run_task` still returns the full terminal tail either way, so a crash whose cause the parser drops is still visible to you.

Auto-fix does not apply to **AI-initiated** runs. When you call `run_task` the failure comes back to you in the tool result, so a fix conversation would just duplicate the loop you are already in.

An application task's fix conversation is for its own application. A **solution** task's has no application: it works in the repository the task ran in and re-runs the task by its workspace root. Each failure streak gets at most three fix conversations; a successful run starts a new streak.

So `aiAutoFix` is for a human clicking the button. Leave it `false` unless the user asks for it.

## Running and verifying

**Never claim a task works because the JSON parses.** Both shells watch these files and reload within ~200ms of a save, so verification is immediate:

1. `run_task` it. Pass `applicationId` for an application task; pass that task's `workspaceRoot` for a solution task (`availableTasks` reports `scope` and `workspaceRoot` per task — copy the value, don't construct a path).
2. Read the exit code **and** the output. A task can exit 0 having done nothing — a `workingDirectory` pointing at the wrong folder often does exactly that.
3. For a background task, check it reported ready rather than timed out, and that the `readyPattern` matched the line you expected. `run_task` defaults `stopWhenReady: true`, which gives you a one-shot verdict instead of leaving a watcher running; pass `false` only when the user wants it left up.

A task that has not been run once is a task you have not tested. Say so plainly if you could not run it (no shell available, a command needing credentials) rather than reporting it done.

## Editing safely

- **Patch, don't rewrite.** `patch_file` on the one task keeps the user's comments, ordering and formatting; a `write_file` of the whole document silently drops all three.
- **Read before you write.** The file may not exist — check first, and create it with the full `{ "tasks": { ... } }` wrapper rather than a bare map.
- **Renaming a task loses its state, not its history.** The toolbar keys per-task status by name, so a rename reads as "the old task vanished, a new one appeared". Any running session keeps its own tab either way.
- **Removing a task that is currently running** leaves its terminal tab alive until it exits. That is deliberate; don't kill it to tidy up unless asked.
- Keep the task set small and the labels short. This is a toolbar, not a script library — a repository with fifteen buttons has a strip nobody can read, and the toolbar collapses to a dropdown when it runs out of room.
