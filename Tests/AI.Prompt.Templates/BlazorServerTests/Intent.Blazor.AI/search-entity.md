### Styling Rules
- Use existing utility classes from shared/global styles first.
- Component style files should remain minimal - only add truly component-specific styles.
- If you need a new utility class or pattern that does not exist, you may add it to shared/global styles.
- NEVER modify existing shared/global styles or theme values - only add new ones if needed.

### 1. Criteria must come ONLY from the backend search service
- The search form **must only expose filters that are supported by the backend search request model or existing backing search mechanism**.
- Inspect the existing `.razor.cs` search model and backing methods:
	- Identify the primary search method (for example `LoadServerData`, `LoadEntities`, `SearchEntities`).
	- Inspect the request DTO/query object passed to services.
	- These properties are the **single source of truth** for search fields.

- (IMPORTANT) Never invent filters:
	- Do not add filters unless they exist in the backing request/search model.
	- Do not modify service signatures or DTOs to make UI filters work.

### 2. Paging & Sorting Rules
- Do **not** create standalone form controls for paging or sorting parameters.
- If paging/sorting values exist (`pageNo`, `pageSize`, `orderBy`, etc.):
	- Use them in table/server-data flow.
	- Do **not** expose them as normal filter inputs.
- If you implement `LoadServerData`, it must have exactly 2 parameters:
	- `(TableState state, CancellationToken cancellationToken)`

### 2A. Choose the correct example pattern
- There are **2 distinct search/list examples** in the `SearchEntity` folder and you must choose the correct one based on the backing class:
	- **`SearchEntityTemplate`**: use this pattern when the component supports filtering, paging, or sorting via methods like `LoadServerData(TableState state, CancellationToken cancellationToken)`, request models with `pageNo/pageSize/orderBy`, or explicit search/filter properties.
	- **`GridEntityTemplate`**: use this pattern when the component does **not** support filtering/paging/sorting and instead loads a plain collection directly (for example in `OnInitializedAsync()` via methods like `LoadAllEntities()` or similar).
- Do **not** mix the two patterns.
- If the backing class does not expose search/filter properties or server-data paging/sorting methods, do **not** generate search fields, `ServerData`, `MudTableSortLabel`, pager content, or reload-through-table patterns.
- If the backing class does expose server-data paging/sorting or real search/filter properties, follow the filtered/searchable example and do **not** fall back to the simple grid pattern.

### 3. Mapping DTO properties to MudBlazor UI controls
Choose the correct control type based on the property in the DTO/search model (excluding paging/sorting):

- **string / string?**
	- If named like `search`, `searchTerm`, `keyword` -> Use a single search text field.
	- Otherwise -> Use `MudTextField`.

- **bool / bool?**
	- Use `MudSelect` with:
		- All (null/empty)
		- Yes / No (or Active / Inactive depending on naming)

- **enum or lookup values**
	- Use `MudSelect`.
	- For enum options, use the enum's underlying numeric value for each option `Value` (cast to int), not enum-name strings.
	- Do NOT use string literals like `Value="AddressType.Delivery"`.
	- Preferred pattern:
		- `<MudSelect T="int" @bind-Value="...">`
		- `<MudSelectItem T="int" Value="@((int)AddressType.Delivery)">Delivery</MudSelectItem>`
	- Populate options only from real lookup services that exist.
	- Do not create fake lookup data.

- **number / number?**
	- Use `MudNumericField`.

- **dates**
	- Use `MudDatePicker`.

- For MudBlazor generic components (for example `MudSelect`, `MudRadioGroup`, `MudSwitch`, `MudChipSet`), declare `T` explicitly.
- Always add placeholders to `MudSelect` controls.
- If you use `ValueChanged`, use it together with `Value`, not `@bind-Value`.
- (CRITICAL) MudBlazor enum properties like `AlignItems`, `Justify`, `Direction`, `Variant`, `Color`, `Size` must use explicit enum values: e.g., `AlignItems="AlignItems.Center"`, not `AlignItems="Center"`.

### 4. Search button behavior
- Do not auto-query on every keystroke unless an existing method already does so and must be preserved.
- Provide a **Search** button that:
	- reads current form values,
	- calls the existing component method for data loading,
	- does not modify backend-calling methods.
- Pressing Enter in the main search field should trigger the same search behavior.
- If you add key handlers like `OnKeyPress`, use Razor lambdas directly (for example `@(async e => { ... })`) without escaped quotes.
- For the `SearchEntityTemplate` pattern, keep the action row visually consistent with the sample template:
	- Use a plain `MudItem xs="12"` action row inside the existing `MudGrid`.
	- Render the Search/Add buttons as sibling `MudButton` elements directly in that row.
	- Do **not** wrap those action buttons in a `MudStack`, `MudGrid`, or other extra layout container unless the sample already does so.
	- Keep the layout similar to the sample template rather than introducing a new button-group pattern.

### 5. Using existing component methods
- (IMPORTANT) If the component already includes methods like:
	- `LoadServerData(...)`
	- `SearchEntities(...)`
	- `RefreshTable(...)`
	- `LoadEntities(...)`

	You **must call those methods** - do not duplicate logic and do not rewrite backend calls.

### 6. Table output rules
- Columns must represent only fields that exist on the returned DTO/view model.
- Never invent table columns.
- Use `MudTable` (or existing equivalent table pattern already used by the component).
- **Filtered/searchable pattern**:
	- Use `ServerData="LoadServerData"` when that method exists.
	- Use `MudTableSortLabel` only when the page supports sorting through the backing search mechanism.
	- Use `MudTablePager` only when the page supports paging through the backing search mechanism.
- **Simple grid pattern**:
	- Use `Items="..."` bound to the existing loaded collection.
	- Do **not** use `ServerData`, `MudTableSortLabel`, or pager content.
	- If a load method like `LoadAllEntities()` exists, the Refresh button should call that method directly.
- When checking which buttons to render, inspect all methods available on the backing component class (public, protected, internal, private), not just public methods.
- (CRITICAL) Evaluate each row action button independently - only render it if the exact corresponding method exists in `.razor.cs`:
	- Add a **View** row button only if a method named like `View...`, `Navigate...View...`, `NavigateTo...ViewPage...`, or equivalent exists and it accepts an id-like row argument (for example `Guid`, `int`, or `string`).
	- Add an **Edit** row button only if a method named like `Edit...`, `Navigate...Edit...`, `NavigateTo...EditPage...`, or equivalent exists and it accepts an id-like row argument (for example `Guid`, `int`, or `string`).
	- Add a **Delete** row button only if a method named like `Delete...`, `OnDelete...`, or equivalent exists.
	- If the row DTO has an id field (for example `Id` or `<Entity>NameId`) and a matching existing Edit method is present, the Edit row button is required and must be rendered.
	- If the method is absent, **do not add the button at all** - never invent or stub a method to justify a button.

### 7. General constraints
- Do not change, add, rename, or remove DTO properties.
- Do not modify backend-calling methods - only call them.
- Do not generate UI fields for properties that do not exist in the search model/DTO.
- Only add search criteria if they are supported by the backing page search mechanism.
- (CRITICAL) Add an Add Entity button only if the backing class already defines a navigation/action method for it; never invent one.
- (CRITICAL) NEVER add navigation or CRUD action methods to the `.razor.cs` file. Methods such as `AddEntity()`, `EditEntity(id)`, `ViewEntity(id)`, or `DeleteEntity(id)` must already be present in the backing class. If they are absent, do NOT add them to `.razor.cs` and do NOT add the corresponding buttons to the `.razor` file.
- If there are forms, ensure they are valid when running searches/actions.
- (CRITICAL) Button placement depends on whether the page uses a filter form:
	- **Page WITH filter fields** (paging or filtering enabled): place the Add button inline with the filter controls inside `MudCardContent`, but only if its corresponding method exists in `.razor.cs`.
	- **Page WITHOUT filter fields**: place any action buttons (e.g. Add) inside `<CardHeaderContent>`, never in the card body.

### 8. CardHeaderContent layout for list pages
- For list pages without search/filter forms (the `GridEntityTemplate` pattern), include both:
	- **Add** action button (only if a navigation method like `NavigateTo...Add...` exists in the component)
	- **Refresh** action button (only if a load method like `LoadEntities()`, `RefreshTable()`, or similar exists in the component)
- For list pages with search/filter forms (the `SearchEntityTemplate` pattern), keep the action buttons in the card body with the filter controls instead of moving them into `CardHeaderContent`.
- For list pages with search/filter forms, preserve the sample action-row structure from `SearchEntityTemplate.razor` and do **not** replace it with a `MudStack`-based button group.
- Never render action buttons in the center or omit the Refresh button when a load method exists.
