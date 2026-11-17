---
description: Every Software Factory run checks this application for drift between the model and the code. Read the drift report whenever a run reports drift.
alwaysApply: true
contentHash: 8F0D10E253B3A8B89616CA93A99432421BE89BA89BC373E5B536525BB9426B48
---
# Drift report

Modules installed in this application check it for drift on every Software Factory run: where the model and the code disagree, or where the code breaks a rule the model states.

## After a Software Factory run

- Every drift **error** fails the run and is one of the run's errors. It starts with its Area in square brackets, e.g. `[Domain]`. An error about a model element is on that element, so Intent offers to fix it with AI.
- Every Area with findings ends with one line, `Drift in <Area>: N errors, M warnings. Report: <path>`. It is an error when the Area has errors, otherwise a warning.
- A drift **warning** never fails the run. It is only a warning line in the run's output, which the results an agent gets may leave out. **The report has every finding in full, warnings included:** `intent/.cache/drift/eShop/DriftReport.mdx`, in the `.cache` folder beside the solution's `.isln` file, which git ignores. A run with findings lists the report in its changes as a new file; a run with none writes no report. A run with errors applies none of its changes, so apply the report with `apply_staged_file_changes` and its path to read it from disk.
- The CLI (`ensure-no-outstanding-changes`) never writes the report: read the errors in its log.

## Reading the report

The report opens with a summary of the errors and warnings in each **Area**. An Area is one check, or several checks that share a name, such as Architecture or Domain. Each Area has a table per **category**:

- **Model / Code**: the model and the code disagree.
- **Code / Code**: code breaks a rule about other code, such as a layer importing one it may not depend on.
- **Code**: the code alone has a problem, such as a file no convention covers or a check's own configuration being wrong.

A row's **Subject** is what the finding is about (a model element or a file), **Against** is what it was compared with, and **Finding** says what is wrong. `✓ Clean` means the category was checked and nothing was found. An Area can end with notes and tables, such as the layers a check read.

Never edit the report: every run rewrites it. Resolve each error, then run the Software Factory again until it reports no drift.
