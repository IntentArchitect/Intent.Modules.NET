# Handover: the Software Factory decides deletes from a per-machine file

**For:** the Intent Architect platform team
**From:** the drift engine work (Intent.Common.Drift 2.0), 2026-11-24
**Source checked:** `D:\Dev\Intent`, branch `release/5.4.x` at `48e6002bed`. Paths below are relative to `Intent.IntentArchitect/`.

## Summary

The Software Factory works out which files to delete, and which files were renamed, from `.intent/output-log.xml`. That file is git-ignored, so every machine has its own copy, and a fresh clone or a CI agent has none. The same commit can therefore give different Software Factory results on different machines:

- a file that every machine should keep can be proposed for deletion on some machines;
- a file that should be deleted is never proposed for deletion on a machine with no log.

Until October 2025 the record was `<application>.application.output.log`, which is committed. It was moved out of git on purpose, to stop the merge conflicts it caused, and is now legacy: the Software Factory only keeps it up to date where it already exists. So any application created since then has no committed record of what the Software Factory generated that the delete logic reads.

The committed `<application>.application.managed-files.xml`, added in August 2026, already holds the same set of paths, so that Changes Review gives the same answer on every machine. The delete logic doesn't read it.

## The four files

| File | In git | Written | Read when deciding deletes and renames |
| --- | --- | --- | --- |
| `.intent/output-log.xml` | No: Intent's generated `.gitignore` excludes `**/.intent/*` | Every run, before and after changes are applied | **Yes** |
| `<application>.application.output.log` (legacy) | Yes: the `.gitignore` explicitly keeps `!*.application.output.log` | Only if it already exists | **Yes** |
| `<application>.application.output.config.xml` | Yes | When a file is ignored or generated once-off | **Yes**, for ignored and once-off state only |
| `<application>.application.managed-files.xml` | Yes, with `merge=union` | Every run, from the output log's paths | **No**: read by Changes Review and the File Classification pills |

Where this is in the code:

- `OutputLogPersistable.GetPath` puts the log at `.intent/output-log.xml` (`IntentArchitect.ElectronClient/Intent.IArchitect.Agent.Persistence/Output/OutputLogPersistable.cs:60-63`).
- `GitIgnoreHelper` ignores `**/.intent/*` and keeps `*.application.output.log` (`IntentArchitect.ElectronClient/Intent.IArchitect.Agent/Utils/GitIgnoreHelper.cs:16-29`).
- `OutputLog.SaveCore` always writes `.intent/output-log.xml`, writes the legacy log only `if (FileSystem.Exists(...))`, and writes the config file when it has entries (`IntentArchitect.SoftwareFactory/Intent.SoftwareFactory/OutputLog.cs:373-437`).
- `ManagedFilesPersistable`'s own doc comment calls it "a mirror of the git-ignored `.intent/output-log.xml` path set", there "so the deterministic/custom split reads the same in a fresh clone, in CI, and for a teammate who has never generated the application". It says `output-log.xml` "stays a local runtime artifact" (`IntentArchitect.ElectronClient/Intent.IArchitect.Agent.Persistence/Output/ManagedFilesPersistable.cs:12-24`).

## How a delete is decided today

1. `OutputLog.Load` builds the "previous run" set from the legacy log, then `.intent/output-log.xml`, then `output.config.xml` (`OutputLog.cs:59-105`).
2. `GetFilesNoLongerBeingGenerated` returns every previous entry this run didn't generate (`OutputLog.cs:177-181`).
3. `OutputManager` proposes a delete for each of those that is still on disk and whose overwrite behaviour allows deleting (`IntentArchitect.SoftwareFactory/Intent.SoftwareFactory/Engine/Output/OutputManager.cs:310-317`).
4. A rename is detected only when the previous run has an entry for the same correlation id (`Engine/ChangeManagement/Change.cs:33-66`).

So the "previous run" is whatever this machine's own log says. Where that log is missing or differs from a teammate's, so does the result.

## How it got this way

- **2021-11:** `.gitignore` generation keeps `*.application.output.log` in git, so the output log was committed.
- **2025-10-01**, commit `5cbda81292`: a deliberate change, "to eliminate the cause of many SCM (e.g. Git) file conflicts". `<application>.application.output.log` is replaced by two files:
  - a committed file of ignored changes, "so that the Software Factory CLI running on CI servers and other users will also know which Software Factory files to ignore";
  - `.intent/output-log`, which tracks, among other things, "whether a file from a previous run now needs to be deleted or renamed". The release note says it "should not be committed to source control".

  This is the point where delete and rename decisions became per machine.
- **2025-10-13**, commit `adaeeeb9a2`: the old committed file is read as `LegacyOutputLogPersistable` and kept up to date only where it still exists.
- **2025-10-20**, commit `55f81df7db`: the ignored file becomes `output.config.xml` and also records once-off state. Those two states are shared through git; the list of generated files isn't.
- **2026-08:** `managed-files.xml` is added as a committed copy of the path set, written as one line per file with `merge=union`, so it doesn't cause the conflicts the old log did. Commit `549279e9e6` moves the File Classification pills "from committed managed-files, not local output-log". The same per-machine problem was fixed for Changes Review, but not for deletes.

## What goes wrong

### 1. A file every machine should keep is proposed for deletion on other machines (seen)

When a file stops being generated but should stay as hand-written code, only the machine that made that change can drop it from its log. Typical cases: uninstalling a module but keeping what it generated, or moving a file from generated to hand-written. Every other machine's log still lists the file, so its next run proposes deleting it, marked destructive.

We hit this with Intent.Common.Drift 2.0. Updating it removes the Guardrails Contracts modules and keeps the 18 contract templates' files as hand-written code. To stop the Software Factory deleting them, its migration removes their entries from the log, but it can only reach the log on the machine where it runs.

Tested on a copy of a real client application, with a teammate's clone made before the update, and with only the log pruning described below:

- **The teammate's first run** proposed deleting all 36 contract files.
- **Their next run, with nothing applied,** was clean, because Common.Drift removes the entries during the run.
- **After applying the run's changes through an AI agent** (`apply_staged_file_changes`), the 36 entries were back in the log, and every later run proposed the deletes again.

The stand-in templates described below now prevent all of these for Common.Drift's files, but only for them.

Today's only shared way to keep such a file is **Ignore**, recorded in `output.config.xml`. The delete then shows as an ignored change on every machine, on every run, permanently.

### 2. A file that should be deleted is never proposed for deletion on a machine with no log (from the code, not run)

On a fresh clone, a new teammate's machine or a CI agent, `.intent/output-log.xml` doesn't exist. For an application with no legacy log, the "previous run" set is then only `output.config.xml`'s entries, so `GetFilesNoLongerBeingGenerated` returns almost nothing.

This has two consequences:

- **No deletes there.** A generated file whose element or template was removed, but whose delete was never applied and committed, is never offered for deletion on that machine.
- **CI can't catch it.** On CI, `ensure-no-outstanding-changes` can't report an unapplied delete, while a developer's machine with a log would show it.

### 3. Renames look like new files on a machine with no log (from the code, not run)

`Change.CreateFromOutputFile` treats a file as renamed only when the previous run has an entry for its correlation id. Without the log, a renamed output is created at its new path, and the old file isn't removed or offered for deletion.

### 4. Machines drift apart over time (seen)

Each machine's log reflects what that machine applied. Unticked deletes, partial applies and module code that edits the log all leave machines with different logs for the same commit.

One case we observed: `apply_staged_file_changes` saves the log from memory, and no Software Factory hook runs after it. Anything a factory extension removed from the log during the run is written back.

## Workaround in Intent.Common.Drift 2.0

A module can't simply remove the entries from the log before the run decides its deletes. `SoftwareFactoryProcessOrchestrator` takes `IOutputLog` in its constructor (`SoftwareFactoryProcessOrchestrator.cs:82`), and `OutputLog`'s constructor reads the file, so the log is read before the first factory extension step, Start (line 290). An edit to the file at Start or a Metadata Load step is lost twice over:

- the deletes are worked out from the copy already in memory;
- `SaveEntriesForExistingFiles` (line 404) writes that copy back over the file before After Template Execution (line 422).

The SDK's view of the log, `IExecutionOutputLog`, is read-only. So Common.Drift does two things instead:

1. **Stand-in templates for this run.** A template registration registers, for each contract file the machine's log still lists and that is on disk, a template at the same path, under the entry's own correlation id, with overwrite disabled. The Software Factory counts the file as generated this run, so `GetFilesNoLongerBeingGenerated` (`OutputLog.cs:177-181`, which matches by correlation id or path) doesn't return it, and no delete is proposed. The file is already there and can't be overwritten, so the change is locked to what is on disk (`Change.cs:45-79`) and nothing is written. The stand-ins are deliberately not once-off: once-off is recorded in the committed `output.config.xml` (`OutputLog.cs:451-454`), and `Load` reads every entry there back as ignored (`OutputLog.cs:89-96`). On the test copy, once-off stand-ins created that file where the application had none.
2. **Pruning for later runs.** A factory extension removes the contract entries from the log, `output.config.xml` and `managed-files.xml` at After Template Execution, and again at After Commit Changes (line 599), after the Software Factory's own saves. `managed-files.xml` needs it because it is rebuilt from the run's outputs (lines 1675-1689), and the stand-ins are outputs. Once the log is pruned, no stand-in registers.

Tested on the same copy, with the teammate's unpruned log:

- **The first run** proposed no contract delete and changed no committed file.
- **The next run** was clean.
- **After an agent apply,** the 36 entries were back in the log, and the contract files were back in `managed-files.xml`. The run after that still proposed no delete, and removed both again.

Its limits:

- **An agent's apply still writes the entries back,** as described under case 4, so until that machine's next run, its `managed-files.xml` lists the contract files.
- **It only covers this one module.** Any module that releases files would need its own copy of both parts.
- **It doesn't touch cases 2 to 4** in general.

It is a stopgap. The fix belongs in the platform.

## Options

**A. Decide deletes from the committed inventory (recommended).** Propose a delete only for a file that `managed-files.xml` lists and this run doesn't generate. Keep `.intent/output-log.xml` for what it adds on one machine: correlation ids for rename detection, and model correlations for Open in IDE and View Code.

- **Same result everywhere.** Every machine and CI agent gets the same answer for the same commit.
- **Releasing a file works for everyone.** A commit that releases a file also removes it from the inventory, so no other machine proposes deleting it.
- **CI sees unapplied deletes.** The inventory still lists the file, so CI would report the delete as outstanding.
- **Gaps to close:**
  - The inventory has paths and template ids but no correlation ids, so rename detection still needs the local log, or the inventory needs correlation ids added.
  - Applications with no inventory yet need a fallback: today's behaviour, until the first run writes one.

**B. An SDK call that lets a module forget a generated file before deletes are worked out.** This would fix module-driven cases such as Common.Drift's, but nothing else. Common.Drift shows a module can already get the same result with stand-in templates and pruning, so this would mostly make that one call instead of two mechanisms. It also costs an Intent release and a higher client minimum for every module that uses it.

**C. Commit the output log again.** This would bring back the merge conflicts the October 2025 change removed. Not recommended. Option A gets the same shared answer from a file already designed not to conflict.

## Questions for the team

1. The October 2025 change moved delete and rename decisions to each machine's own log, to stop merge conflicts. Was the cost described above, deletes that differ between machines and between developers and CI, weighed at the time?
2. Should `managed-files.xml` be what deletes are decided from (option A)?
3. Is `.intent/output-log.xml` meant to be phased out? The code doesn't suggest it: it's written on every run, and `managed-files.xml` describes it as staying a local runtime artifact.

## Reproducing case 1

1. Take an application with one generated file, `F`, committed. Clone it twice, as machine A and machine B, and run the Software Factory on both, so both logs list `F`.
2. On machine A, make the Software Factory stop generating `F` while keeping the file. For example, remove the Template Output that places it, and remove `F`'s entry from A's `.intent/output-log.xml` the way a module can. Then run the Software Factory, which proposes no delete, and commit.
3. On machine B, pull and run the Software Factory. It proposes deleting `F`, marked destructive.

To reproduce case 2, delete `.intent/output-log.xml` on a machine where a generated file's element has been removed but the delete was never applied. The run proposes no delete for that file.
