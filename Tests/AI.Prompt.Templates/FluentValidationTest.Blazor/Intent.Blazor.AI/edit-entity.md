### Source of truth
- In `OnInitializedAsync()` / `OnParametersSetAsync()`, load the data the component needs using the provided services and existing backing methods.
- Do not modify the shape of the model.
- Do not invent new fields.
- Favour reusing existing backing methods for fetching data.

### Form controls
- Same field mappings as the Add screen.
- Prepopulate values using existing bindings on `model.PropertyName`.
- For MudBlazor generic controls (for example `MudSelect`, `MudRadioGroup`, `MudSwitch`, `MudChipSet`), declare `T` explicitly.
- For `MudSelect`, always add placeholders.
- If you use `ValueChanged`, use it with `Value` (NOT `@bind-Value`).
- If you add Dense, Variant, or Margin settings on form controls, do it consistently across the form.

### Validation
- Form must use valid Blazor form patterns (`EditForm`) with existing validation conventions.
- Required fields should have:
	- validation wiring through model annotations and validation components
	- validation messages shown when invalid
	- Save disabled when invalid
- If using MudBlazor validation delegates, ensure the lambda/method signature matches `Func<T, IEnumerable<string>>` and nullability is correct for the type.
- If multiple forms are used to isolate validations, they must not be nested.

### Save behavior
- The Save button must call `Save()` / `SaveAsync()`.
- Save must:
	- validate the form,
	- call the existing service method (for example `UpdateEntity()` / `UpdateEntityAsync()`),
	- **NOT** modify that existing service-calling method,
	- on success, navigate using an existing navigation method such as `NavigateToEntitySearch()`.

### Conditional sections
- If the model uses boolean toggles (for example `HasLoyalty`):
	- Only render conditional blocks when flagged.
	- Maintain the exact logic from the `.razor.cs` file for showing/hiding these blocks.
- When generating MudBlazor components inside conditional Razor blocks, always set generic `T` explicitly.

### Child collections
- When arrays/collections exist (`Addresses`, `Phones`, etc.):
	- Render list items in MudBlazor UI blocks.
	- Include add/remove buttons **only if corresponding backing methods exist**:
		- `AddAddress()` / `AddAddressAsync()` -> show Add button
		- `RemoveAddress(i)` / `RemoveAddressAsync(i)` -> show Remove button
- If indexed binding or index-based callbacks are required (for example `Model.Addresses[index]` or `RemoveAddress(index)`), use a `for` loop and declare `var index = i;` inside each iteration.
- Never use `i` directly inside loop markup, bindings, `@key`, or callbacks; always use the local `index` variable.

### Styling Rules
- Use existing utility classes from shared/global styles first.
- Component style files should remain minimal - only add truly component-specific styles.
- If you need a new utility class or pattern that does not exist, you may add it to shared/global styles.
- NEVER modify existing shared/global styles or theme values - only add new ones if needed.
