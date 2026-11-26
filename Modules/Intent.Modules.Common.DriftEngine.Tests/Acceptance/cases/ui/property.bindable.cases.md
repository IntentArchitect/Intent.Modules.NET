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

## react-controlled-props

React binds both ways through a controlled prop: the value, and a callback that hands back a new one, `value` with `onChange`, or a name with `on{Name}Change`. The stock React reader reads a Property as bindable when the component takes both. `Quantity` has `onQuantityChange`; `Max` has no callback, so it isn't bindable, against the model.

```json case
{ "phase": "later", "kinds": ["property.bindable"], "requirements": ["R2", "R3", "R28", "R38"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "quantity-input", "type": "Component", "name": "QuantityInput",
            "children": [
              { "id": "quantity-input-quantity", "type": "Property", "name": "Quantity", "typeRef": { "type": "int" }, "stereotypes": { "Bindable": {} } },
              { "id": "quantity-input-max", "type": "Property", "name": "Max", "typeRef": { "type": "int" }, "stereotypes": { "Bindable": {} } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```tsx file src/QuantityInput.tsx
interface QuantityInputProps {
    quantity: number;
    max: number;
    onQuantityChange(quantity: number): void;
}

export function QuantityInput({ quantity, max, onQuantityChange }: QuantityInputProps) {
    return <input type="number" max={max} value={quantity} onChange={(e) => onQuantityChange(Number(e.target.value))} />;
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "quantity-input-quantity", "outcome": "verified", "actual": "true", "at": "src/QuantityInput.tsx:2" },
    { "kind": "property.bindable", "element": "quantity-input-max", "outcome": "contradicted", "expected": "true", "actual": "false", "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/property.bindable", "severity": "error", "subject": "quantity-input-max" }
  ]
}
```

## maui-bindable-property

A .NET MAUI component's property is bindable (`intent.maui`) when a static `{Name}Property` `BindableProperty` backs it, the only way a parent binds it: eShop's ClientApp badge binds its `Text`; its `Caption` is a plain property. MAUI is in the code through the page's XAML.

```json case
{ "phase": "later", "kinds": ["property.bindable"], "requirements": ["R2", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "badge", "type": "Component", "name": "BadgeView", "children": [
            { "id": "badge-text", "type": "Property", "name": "Text", "typeRef": { "type": "string" }, "stereotypes": { "[Bindable]": { } } },
            { "id": "badge-caption", "type": "Property", "name": "Caption", "typeRef": { "type": "string" } }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```xml file src/Shop/Shop.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <UseMaui>true</UseMaui>
  </PropertyGroup>
</Project>
```

```xml file src/Shop/Views/CatalogView.xaml
<ContentPage x:Class="Shop.Views.CatalogView"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:views="clr-namespace:Shop.Views">
    <views:BadgeView Text="{Binding Count}" />
</ContentPage>
```

```csharp file src/Shop/Views/CatalogView.xaml.cs
namespace Shop.Views;

public partial class CatalogView : ContentPage
{
}
```

```csharp file src/Shop/Views/BadgeView.cs
namespace Shop.Views;

public class BadgeView : Grid
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(BadgeView), "0");

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Caption { get; set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "badge-text", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop/Views/BadgeView.cs:5" },
    { "kind": "property.bindable", "element": "badge-caption", "outcome": "verified", "expected": "false", "actual": "false" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-bindable-property-removed

A planted drift: the badge's `Text` is a plain property. One finding: no `BindableProperty` backs it, so a parent can't bind it.

```json case
{ "phase": "later", "base": "maui-bindable-property", "kinds": ["property.bindable"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/Views/BadgeView.cs
namespace Shop.Views;

public class BadgeView : Grid
{
    public string Text { get; set; } = "0";

    public string Caption { get; set; } = "";
}
```

```json expect
{
  "claims": [
    { "kind": "property.bindable", "element": "badge-text", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/property.bindable", "severity": "error", "subject": "badge-text", "messageContains": "No `TextProperty` BindableProperty backs it, so a parent can't bind it." }
  ],
  "noOtherFindings": true
}
```
