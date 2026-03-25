## ➕ ADD ENTITY SCREEN RULES (Blazor + MudBlazor + Best Practice)

### Form generation rules
- Build a Blazor MudBlazor form **based on the entity model defined in the `.razor.cs` file**.
- Bind every input to a property on `model`.
- Do not add properties that do not exist.
- Do not rename or remove properties.

### Form controls
- For each property:
	- Strings -> `MudTextField`
	- Booleans -> `MudSwitch` or `MudCheckBox`
	- Enums -> `MudSelect`
	- Lookups -> `MudSelect` with service-loaded options if such services exist
	- Arrays -> repeatable MudBlazor blocks

### Validation
- Use Blazor form validation with `EditForm` and data annotations, or existing validation patterns already present in the component.
- Required fields must have appropriate validation wiring through model annotations and validation components.
- Validation messages must be visible when inputs are invalid.
- The Save button must be disabled if:
	- the form is invalid, or
	- `isLoading` is true.

### Save behavior
- (IMPORTANT) The **Save** button must call an existing save action method (`Save()`, `SaveAsync()`, or equivalent existing method).
- The save action must:
	- perform validation before save,
	- call the existing service method *without modifying it*,
	- on success, navigate using an existing navigation method (e.g. `NavigateToEntitySearch()`).

### Forbidden actions
- DO NOT modify the existing backend-calling method (e.g. `CreateEntity()` / `CreateEntityAsync()`).
- DO NOT change the shape of the payload.
- DO NOT invent lookup services.
- DO NOT add logic that rewrites existing C# functionality.

### Child collections
- Render them in repeatable MudBlazor UI blocks.
- Include a delete button **only if the `.razor.cs` file already contains a method like `RemoveX()` / `RemoveXAsync()`**.
- Include an add button **only if `AddX()` / `AddXAsync()` exists**.
- If rendering child collections with a `for` loop, declare `var index = i;` inside each iteration and use `index` for all bindings, `@key`, and callbacks.
- Never bind or pass `i` directly inside the loop body.

### Styling Rules
- Use existing utility classes and shared styles first.
- Component style files should remain minimal - only add truly component-specific styles.
- If you need a new utility class or pattern that does not exist, you may add it to shared/global styles.
- NEVER modify existing styles in shared style/theme files - only add new ones if needed.