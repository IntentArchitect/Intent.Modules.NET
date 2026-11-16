# property.bindable

Whether a component's Property supports two-way binding (`@bind-Quantity`): the Bindable stereotype on a User Interface designer Property, against a `[Parameter] EventCallback<T>` named after the property with `Changed` appended. The kind is `stated: always`: a component with no such callback states that the Property isn't bindable, and the check runs both ways.

The stock User Interface pack gives the kind and its vocabulary, and the Blazor reader reads the component, whose `.razor` and `.razor.cs` files are one unit (R37).

## verified

A quantity input whose `Quantity` parameter has a `QuantityChanged` callback, and the model's `Quantity` Property is Bindable.

```json case
{ "phase": "later", "kinds": ["property.bindable"], "traces": ["ESC-FE-20"], "requirements": ["R2", "R28", "R37"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog", "type": "Folder", "name": "Catalog",
            "children": [
              { "id": "quantity-input", "type": "Component", "name": "QuantityInput",
                "children": [
                  { "id": "quantity-input-quantity", "type": "Property", "name": "Quantity",
                    "typeRef": { "type": "int" },
                    "stereotypes": { "Bindable": {} } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```razor file src/WebAppComponents/Catalog/QuantityInput.razor
<input type="number" min="1" value="@Quantity" @onchange="OnChangeAsync" />

@code {
    [Parameter]
    public int Quantity { get; set; }

    [Parameter]
    public EventCallback<int> QuantityChanged { get; set; }

    private Task OnChangeAsync(ChangeEventArgs e)
        => QuantityChanged.InvokeAsync(int.Parse((string)e.Value!));
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "quantity-input-quantity", "outcome": "verified",
      "expected": "true", "actual": "true", "at": "src/WebAppComponents/Catalog/QuantityInput.razor:7" }
  ]
}
```

## contradicted-not-modelled

The component has the `QuantityChanged` callback, and the model's `Quantity` Property isn't Bindable.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.bindable"], "traces": ["ESC-FE-20"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog", "type": "Folder", "name": "Catalog",
            "children": [
              { "id": "quantity-input", "type": "Component", "name": "QuantityInput",
                "children": [
                  { "id": "quantity-input-quantity", "type": "Property", "name": "Quantity",
                    "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "quantity-input-quantity", "outcome": "contradicted",
      "expected": "false", "actual": "true", "at": "src/WebAppComponents/Catalog/QuantityInput.razor:7" }
  ],
  "findings": [
    { "rule": "claim/property.bindable", "severity": "error", "subject": "quantity-input-quantity" }
  ]
}
```

## contradicted-callback-missing

The model's `Quantity` is Bindable, and the component has no `QuantityChanged` callback: it raises a differently named one, so `@bind-Quantity` won't compile against it. From silence, `Quantity` isn't bindable.

```json case
{ "phase": "later", "base": "verified", "kinds": ["property.bindable"], "cases": ["engine/silence"], "traces": ["ESC-FE-20"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```razor file src/WebAppComponents/Catalog/QuantityInput.razor
<input type="number" min="1" value="@Quantity" @onchange="OnChangeAsync" />

@code {
    [Parameter]
    public int Quantity { get; set; }

    [Parameter]
    public EventCallback<int> OnQuantityChange { get; set; }

    private Task OnChangeAsync(ChangeEventArgs e)
        => OnQuantityChange.InvokeAsync(int.Parse((string)e.Value!));
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "quantity-input-quantity", "outcome": "contradicted",
      "expected": "true", "actual": "false", "fromSilence": true }
  ]
}
```
