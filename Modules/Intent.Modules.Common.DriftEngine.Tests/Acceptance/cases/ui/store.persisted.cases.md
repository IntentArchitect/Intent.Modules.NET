# store.persisted

A Store whose state is saved and restored across restarts (local storage, a settings file, a user-preferences API) carries the User Interface designer's `Persisted` stereotype (Intent.Modelers.UI 2.0.0). `intent.ui` claims it as `store.persisted`, a `bool` kind, `stated: whenSet`, waiting on the Store's `type.exists`. Silence is `false`, so the opt-in `unmodelled.value` probe reports a Store that saves its state while the model leaves Persisted off.

The kind names no framework. .NET MAUI (`intent.maui`) reads it from the Store's class in a MAUI app: it reads or writes the device's `Preferences` or `SecureStorage` (their `Default` instances too), or an `IPreferences` or `ISecureStorage` it is given, anywhere in its code, its property accessors included. A Store of another kind of app saves its state in other ways, which no stack reads yet.

## maui-preferences-in-accessors

eShop's ClientApp keeps its settings in `Preferences`, read and written in its properties' accessors. MAUI is in the code through the app's `MauiApp` builder; the project has no XAML.

```json case
{ "phase": 6, "kinds": ["store.persisted"], "requirements": ["R2", "R28"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "settings", "type": "Store", "name": "SettingsService", "stereotypes": { "Persisted": { } }, "children": [
            { "id": "use-mocks", "type": "Property", "name": "UseMocks", "typeRef": { "type": "bool" } }
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

```csharp file src/Shop/MauiProgram.cs
namespace Shop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp.CreateBuilder().Build();
}
```

```csharp file src/Shop/Services/SettingsService.cs
namespace Shop.Services;

public class SettingsService
{
    private const string IdUseMocks = "use_mocks";

    public bool UseMocks
    {
        get => Preferences.Get(IdUseMocks, true);
        set => Preferences.Set(IdUseMocks, value);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "settings", "outcome": "verified", "expected": "true", "actual": "true", "at": "src/Shop/Services/SettingsService.cs:9" }
  ],
  "others": "verified",
  "noOtherFindings": true
}
```

## maui-secure-storage-given

The Store is given MAUI's `ISecureStorage` and keeps its token there.

```json case
{ "phase": 6, "base": "maui-preferences-in-accessors", "kinds": ["store.persisted"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/Services/SettingsService.cs
namespace Shop.Services;

public class SettingsService(ISecureStorage storage)
{
    public bool UseMocks { get; set; }

    public Task SaveTokenAsync(string token) => storage.SetAsync("token", token);
}
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "settings", "outcome": "verified", "at": "src/Shop/Services/SettingsService.cs:7" }
  ],
  "noOtherFindings": true
}
```

## maui-store-saves-nothing

A planted drift: the Store keeps its setting in a field. One finding, Persisted's.

```json case
{ "phase": 6, "base": "maui-preferences-in-accessors", "kinds": ["store.persisted"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```csharp file src/Shop/Services/SettingsService.cs
namespace Shop.Services;

public class SettingsService
{
    private bool _useMocks = true;

    public bool UseMocks
    {
        get => _useMocks;
        set => _useMocks = value;
    }
}
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "settings", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/store.persisted", "severity": "error", "subject": "settings", "messageContains": "`SettingsService` persisted: model `true`, code `false`. `SettingsService` reads and writes neither `Preferences` nor `SecureStorage`" }
  ],
  "noOtherFindings": true
}
```

## maui-persisted-the-model-leaves-unset

With the opt-in rule for unmodelled values on, a Store the model doesn't mark Persisted that saves its state is one finding; one that doesn't states nothing.

```json case
{ "phase": 6, "base": "maui-preferences-in-accessors", "kinds": ["store.persisted"], "cases": ["optin/unmodelled.value"], "requirements": ["R49"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "ui", "name": "Shop", "type": "User Interface Package",
        "elements": [
          { "id": "settings", "type": "Store", "name": "SettingsService", "children": [
            { "id": "use-mocks", "type": "Property", "name": "UseMocks", "typeRef": { "type": "bool" } }
          ] },
          { "id": "basket", "type": "Store", "name": "BasketState" }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.value" } ] }
        } }]
    }
  }
}
```

```csharp file src/Shop/Services/BasketState.cs
namespace Shop.Services;

public class BasketState
{
    public int Count { get; set; }
}
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "settings", "absent": true }
  ],
  "findings": [
    { "rule": "optin/unmodelled.value", "severity": "error", "subject": "src/Shop/Services/SettingsService.cs:9", "messageContains": "`SettingsService` persisted: the code states `true`, which the model leaves unset" }
  ],
  "noOtherFindings": true
}
```

## pinia-web-storage

A Pinia store keeps its lines in `sessionStorage`, read and written in the module's own functions its setup calls.

```json case
{ "phase": 6, "kinds": ["store.persisted"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "stores", "type": "Folder", "name": "stores", "children": [
            { "id": "cart", "type": "Store", "name": "cart", "stereotypes": { "Persisted": { } } }
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

```typescript file src/stores/cart.ts
import { defineStore } from 'pinia';
import { ref, watch } from 'vue';

const STORAGE_KEY = 'cart';

function readStoredLines(): string[] {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : [];
}

function writeStoredLines(lines: string[]): void {
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(lines));
}

export const useCartStore = defineStore('cart', () => {
    const lines = ref(readStoredLines());
    watch(lines, (value) => writeStoredLines(value), { deep: true });
    return { lines };
});
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "cart", "outcome": "verified", "expected": "true", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/store.persisted", "count": 0 }
  ]
}
```

## pinia-persist-option

A Pinia store with pinia-plugin-persistedstate's `persist` option is persisted.

```json case
{ "phase": 6, "base": "pinia-web-storage", "kinds": ["store.persisted"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```typescript file src/stores/cart.ts
import { defineStore } from 'pinia';

export const useCartStore = defineStore('cart', {
    state: () => ({ lines: [] as string[] }),
    persist: true,
});
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "cart", "outcome": "verified", "at": "src/stores/cart.ts:5" }
  ]
}
```

## pinia-store-saves-nothing

A planted drift: the store keeps its lines in memory only. One finding, Persisted's.

```json case
{ "phase": 6, "base": "pinia-web-storage", "kinds": ["store.persisted"], "requirements": ["R3", "R14"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```typescript file src/stores/cart.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useCartStore = defineStore('cart', () => {
    const lines = ref<string[]>([]);
    return { lines };
});
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "cart", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/store.persisted", "severity": "error", "subject": "cart", "messageContains": "reads and writes neither `localStorage` nor `sessionStorage`" }
  ]
}
```

## not-a-maui-app

The same Store in an app with no MAUI in its code: the MAUI reader leaves it, and nothing else reads how such an app saves its state, so Persisted can't be verified.

```json case
{ "phase": 6, "base": "maui-preferences-in-accessors", "without": ["src/Shop/MauiProgram.cs"], "kinds": ["store.persisted"], "cases": ["engine/noRule"], "requirements": ["R3"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```xml file src/Shop/Shop.csproj
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "settings", "outcome": "cantVerify", "reason": "noRule" }
  ]
}
```

## mobx-class-store-web-storage

A store written as a class (MobX's) keeps its state across reloads when its own code reads or writes Web Storage: an adopted application's common store starts its token from `localStorage` and writes it back in a `reaction` its constructor sets up.

```json case
{ "phase": 6, "kinds": ["store.persisted"], "requirements": ["R2"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "client", "name": "client-app", "type": "User Interface Package",
        "elements": [
          { "id": "common-store", "type": "Store", "name": "CommonStore", "stereotypes": { "Persisted": { } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "client-app", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```ts file src/stores/commonStore.ts
import { observable, reaction } from 'mobx';

export default class CommonStore {
    @observable token: string | null = window.localStorage.getItem('jwt');

    constructor() {
        reaction(
            () => this.token,
            (token) => {
                if (token) {
                    window.localStorage.setItem('jwt', token);
                }
            }
        );
    }
}
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "common-store", "outcome": "verified", "expected": "true", "actual": "true" }
  ],
  "findings": [
    { "rule": "claim/store.persisted", "count": 0 }
  ]
}
```

## mobx-class-store-saves-nothing

The store keeps its token in memory only, so the model's Persisted is contradicted.

```json case
{ "phase": 6, "base": "mobx-class-store-web-storage", "kinds": ["store.persisted"], "requirements": ["R14"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```ts file src/stores/commonStore.ts
import { observable } from 'mobx';

export default class CommonStore {
    @observable token: string | null = null;
}
```

```json expect
{
  "claims": [
    { "kind": "store.persisted", "element": "common-store", "outcome": "contradicted", "expected": "true", "actual": "false" }
  ],
  "findings": [
    { "rule": "claim/store.persisted", "severity": "error", "subject": "common-store" }
  ]
}
```
