### Version 1.0.0

- New Feature: Keeps the User Interface designer true to an existing UI in any technology: the `reverse-engineer-ui` skill models every page, dialog, component and layout and what each calls, navigates to, opens and renders, and the drift check, with tested Blazor, Angular and React recipe guides, fails the Software Factory run when the model and the code disagree.
- Improvement: The UI drift check reports through Intent.Common.DriftReporting: its findings are the UI Area of the application's one drift report (`.cache/drift/<application>/DriftReport.mdx`), contributed by the `UiDriftReporter` factory extension rather than a report template.
