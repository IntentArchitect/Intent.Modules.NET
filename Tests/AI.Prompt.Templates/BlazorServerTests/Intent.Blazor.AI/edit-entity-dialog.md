### Dialog Rules

- This component is a MudBlazor dialog, not a page. Use the standard MudBlazor dialog pattern.
- In `.razor.cs`, use `IMudDialogInstance` (NOT `MudDialogInstance`).
- In `.razor`, use these tags: `<TitleContent>`, `<DialogContent>`, `<DialogActions>`.
- Do NOT use old dialog tags like `<MudDialogTitle>`, `<MudDialogContent>`, `<MudDialogActions>`.
- Use MudBlazor dialog close/cancel APIs from `IMudDialogInstance`:
	- close success with `MudDialog.Close(DialogResult.Ok(true))`
	- cancel with `MudDialog.Cancel()`
- MudBlazor dialog result property name is `Canceled` (NOT `Cancelled`).

- If input data is needed, receive it via `[Parameter]` properties or cascading dialog parameters following existing project conventions.

- Implement two top-level methods for the template to use:
	- `Save()` / `SaveAsync()`:
		- calls the existing service method (for edit/update flow, e.g. `UpdateEntity()` / `UpdateEntityAsync()` logic)
		- on success, closes the dialog with success result
		- on error, sets an error string/state but does not close the dialog

	- `Cancel()`:
		- closes/cancels the dialog
		- do not reset the model or call any services here

- (IMPORTANT) In the `.razor`, bind the action buttons to `Save()`/`SaveAsync()` and `Cancel()`, not to raw service methods.

- (IMPORTANT) Never treat `Cancel()` as a "reset the form" method. It must only close/cancel the dialog.
- (IMPORTANT) After a successful save, always close the dialog with a success result so the caller can react (e.g. refresh the list).

- Do not call service methods directly from the template.
	- Always use a save method that:
		- validates (if needed),
		- calls the service, and
		- closes the dialog on success.

- If a method like `UpdateEntity()` or `UpdateEntityAsync()` already exists and calls the backend, either:
	- call it from inside `Save()` / `SaveAsync()`, **or**
	- inline its logic into `Save()` / `SaveAsync()`, but do not change its behavior to stop calling the service.


### Form & Validation in Dialogs

- Use Blazor form validation with `EditForm` and existing project validation conventions.
- Wrap dialog content and actions in a single form flow so validation and submit behavior are consistent.
- Prefer `OnValidSubmit` for save, or explicit save handler that checks validity before calling services.
- For each required field:
	- Ensure model validation attributes and validation components are correctly wired.
	- Ensure validation messages are shown when invalid and touched/submitted.

- In `Save()` / `SaveAsync()`:
	- If the form is invalid, ensure validation UI is displayed and **return without calling any service**.
	- Only call the backend service if the form is valid.
- The Save button must be disabled if the form is invalid or a save is in progress.

- When the user clicks Save:
	- Run `Save()` / `SaveAsync()`.
	- If the form is invalid:
		- Do not call any service.
		- Ensure validation messages are visible.
	- If the form is valid:
		- Call the existing update service method.
		- On success, close the dialog with a success result so the caller can refresh data.
		- On error, set a `serviceErrors.*` message/state and keep the dialog open.
- Never close the dialog on failure.
- The Cancel button must call `Cancel()` and `Cancel()` must only close/cancel the dialog, with no additional logic.

### Styling Rules
- Use existing utility classes from shared/global styles first.
- Component style files should remain minimal - only add truly component-specific styles.
- If you need a new utility class or pattern that does not exist, you may add it to shared/global styles.
- NEVER modify existing shared/global styles or theme values - only add new ones if needed.
