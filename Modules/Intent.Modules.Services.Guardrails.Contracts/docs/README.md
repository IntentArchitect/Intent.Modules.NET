# Intent.Services.Guardrails.Contracts

Guards the Services designer's model in existing C# code. Each modelled element is merged into its existing file, so every modelled type and member keeps existing. Methods are always generated with empty bodies, so merging never adds code to an implementation.

Install it into an application whose existing code has been modelled in the Services designer, by hand, with Intent's C# importer, or with the AI skill below. Set the Default Type Modifiers module setting to the code's convention, and apply the Type Configuration and Async Configuration stereotypes wherever the code differs from the defaults described below.

It needs Intent Architect 5.4.0-beta.6 or later. Where the application already has them, installing it also brings Intent.VisualStudio.Projects up to 4.1.12-pre.1, Intent.Modelers.Types.ServiceProxies up to 5.5.8-pre.1, Intent.Metadata.WebApi up to 4.7.22-pre.1, and Intent.Modelers.CodebaseStructure up to 1.3.0-pre.0, the versions the AI skill relies on. It depends on Intent.Modelers.Eventing, so the Services designer always offers integration events, integration commands and eventing DTOs.

## AI skill: reverse-engineer-services

The module generates `skills/reverse-engineer-services/SKILL.md` into the application's agents folder (the `AI.Context.Skills` output). An AI agent working in Intent Architect uses it to:

- model existing commands, queries, DTOs and services in the Services designer, mirroring the code's folders and record/class kinds;
- model integration events, integration commands, eventing DTOs and their handlers when the code has messaging, and link the code's own copy of another application's message to the message it copies (see Integration eventing);
- model the code's API clients as Service Proxies, with no mappings (see Service proxies);
- install the CQRS, WebApi, Security, DomainInteractions, EventInteractions and Service Proxies modules when the code calls for them;
- capture the HTTP surface and its security on commands, queries and publicly exposed service operations alike;
- model the intent of every command, query and service operation, public or internal: the entity actions it performs, the services, commands, queries and API client methods it invokes, and the integration events and commands it publishes or sends;
- run the Software Factory until it changes nothing but the Intent file headers and the code's formatting, then adopt the existing files;
- configure the drift check: write the recipes that read the code's HTTP and security facts, until the Software Factory reports no drift (see Drift check).

Run it after `reverse-engineer-domain` (from Intent.Domain.Guardrails.Contracts) when the code has a domain layer.

## Drift check

Once its recipe file exists (below), every Software Factory run compares the Services model with the code on disk, and writes the result to `.cache/guardrails/<application>/ServicesDriftReport.mdx`, in the folder that holds the solution's `.isln` (which every Intent solution's `.gitignore` already ignores). The report is only written when it needs attention, and the last one is deleted when the run starts, so a run that writes one always lists it in the Changes pane. A run with no drift writes no report and logs one line saying so. Intent applies none of a failing run's changes by itself, the report included; apply the ones you expect by hand.

Until the recipe file exists, the check is off: nothing is compared, the report says how to set it up, and the run only logs a warning. Installing the module therefore never blocks a run, including the one that generates the **configure-drift-check** skill that writes the file.

The report is an MDX document with up to two tables, each left out when empty, and no timestamps:

- **Model and code drift**: one row per difference, with the model element (live links to it and the elements it belongs to, such as a class and its attribute, or a Service and its operation), the code file (a link to the line that states it; when the code doesn't state it, to where it would: the nearest place the recipe reached, such as the entity's configuration, else the element's own file) and what the drift is. The model must never be wrong, so every row fails the run, and a CI run fails too. The Software Factory shows one error, which says how many drifts there are and names the report in the run's changes. It is on the first row's model element, so Intent Architect offers to fix it with AI; when no row is about a model element, it is a friendly error instead. On CI, where no report is written, the error lists every row. Intent Architect versions that don't yet show logged friendly and element errors show it as a plain error, prefixed with the exception type. A row says that an HTTP or security fact or a generated type differs between the model and the code (kind, modifiers, base types, a property's type, a method's signature); that a fact can't be verified (a recipe finds no code for it, can't read its value, or the code states it in a form the recipe marks as unreadable, such as a route from a constant); that code isn't modelled (what a recipe's `own` list finds, such as every controller action or every type following an element type's convention, or a member of a generated type, with no exception); that a modelled type or member isn't in the code; or that the recipe file is wrong.
- **Warnings**: a kind of fact the recipe file has no recipe for (for example one a module upgrade adds), an exception that matches nothing, or the check being off. They only warn.

For example:

| Model element | Code file | Drift |
| --- | --- | --- |
| its Service › `ClearCache` (links) | `Basislink.API/Controllers/Main/FeatureController.cs:76` (link) | `FeatureController.ClearCache` route: model `Cache/Clear/{CacheKey}/{cacheKey}`, code `Cache/Clear/{CacheKey}` |

- **Facts.** The model side is fixed: Intent's own stereotypes (`Http Service Settings`, `Http Settings`, `Parameter Settings`, `Secured`, `Unsecured`), read by name, so neither Intent.Metadata.WebApi nor Intent.Metadata.Security is a dependency. The facts are a Service's route; each endpoint's verb, route and effective security (its own `Secured` or `Unsecured`, else its Service's, folders' or package's), with roles and policy where modelled; and each parameter's binding. An endpoint is a Service operation, Command or Query with `Http Settings`.
- **Recipes.** The code side is described once per codebase, in `guardrails/services.drift.json` beside the application's `.application.config`: for each kind of fact, where it is in the code (a find path over classes, methods, parameters, statements, call chains or files) and how to read its value (regular expressions over the code, with defaults and inheritance, e.g. an action's security from its controller). The module knows no endpoint framework; the recipes do, so controllers, minimal APIs, FastEndpoints or anything else can be read. A recipe's `unstated` says, in the codebase's own terms, what the code saying nothing means (no `IsRequired()` in EF Core, which maps a string as nullable), and a drift row whose value comes from the recipe's `default` or `absent` quotes it. A read case marked `unreadable` reports a form no recipe can read (a route from a constant) as can't be verified, rather than letting the recipe's default stand in. Commit the recipe file. The generated **configure-drift-check** skill documents the format and every fact.
- **Structure.** Each type this module's templates generate is compared with its file on disk, but only on what its element can state, so nothing the designer can't express is ever drift or needs an exception. Every type is compared on its modifiers and the base types the model names. Services are also compared on their kind (class or record) and methods, and Service Proxies on their kind (class or interface) and methods. Commands, queries, integration events and integration commands are compared on their kind and properties, and DTOs and eventing DTOs also on their generic parameters; for all of these, parameters on the type are compared only as a positional record. Handlers are compared on their modifiers only. So a Service's, handler's or proxy's primary constructor holding its injected dependencies, a DTO's methods and a handler's `Handle` methods aren't drift. Spellings that mean the same type agree (`Nullable<int>` and `int?`, `System.String` and `string`, a qualified name and its last part). A member the model doesn't describe is reported only when it's public: whether a protected, internal or private member belongs in the model is a judgement call, so leaving one out is never drift, and once modelled it's compared like any other. Fields, events and indexers are never reported: no designer models one. An unmodelled controller action is reported once, even when both a recipe's `own` list and the generated controller type find it.
- **New types, by convention.** New code that should have been modelled is found by the conventions the recipe file records, one `own` entry per element type whose code follows one: a folder and name pattern (every `*DTO` class in the DTO project), a base class (every class deriving from the business base class), an interface, an attribute or a member. The reverse-engineer and configure-drift-check skills record a convention only where the modelled code has one; an element type without one gets no entry, and its new types aren't detected. Types the templates generate, and types in any file a template of the run writes (another designer's included), count as modelled. Where a project mixes helpers with the element type, the convention leaves them out (for example, every class in the DTO project but a static one).
- **Exceptions.** The recipe file's `exceptions` list code that may stay unmodelled: a file pattern, a type, a `Type.Member` or the name a convention gives. A type or `Type.Member` exception covers that declaration, and a type its members' declarations, but not code a convention finds inside a member, such as each `Map…` call in a method that maps minimal API endpoints: that is exempt only by its own name or a file pattern. An exception never covers a contradicted or unverifiable fact.
- **Checking the recipes, on demand.** The drift report doesn't check the recipes. The module's `Intent.Services.Guardrails.Contracts.ValidateDriftRecipes` module task (argument `{ "applicationId": "…" }`, optionally with `"fact"`, `"element"` and `"file"`) reads every fact through its recipe, against the code on disk and the saved model (nothing from a Software Factory run), and returns JSON: per kind of fact, how many facts were read from the code, from the recipe's `default` or `absent` value, couldn't be verified or contradict the model, with samples; read cases that matched nothing; what each convention finds, with samples; the recipe probe's warnings; and the read cases that read some facts from code that names another element of the same kind and not their own (another action's attributes, another parameter's binding). The probe takes one fact per find path and read case that read a value from the code, changes the code the case matched in memory (never on disk), and reads the fact again; if the same case still reads the same value, that case isn't reading the code it seems to. Neither check proves that a recipe reads the right code, so the samples are what show it. With `fact` (a kind of fact), `element` (an element and those it holds: `OrdersController` also selects `OrdersController.GetOrder`) or `file` (a code file, by its path or its end), it covers only the facts selected, leaves out the conventions, and lists every reading in full: the model and code values, where the value came from, the find path and read case, how far it was inherited and the text it matched. The generated **validate-drift-recipes** skill runs the task for each installed Guardrails designer and has the AI check what it read against the code, for every recipe or for one recipe, element or file. The recipe memory earlier builds kept, `.cache/guardrails/<application>/Services.recipe.json`, is deleted by the next run.
- **A model-first change isn't drift, unless the code changed too.** The check remembers, per machine (`.cache/guardrails/<application>/Services.model.json`), what each generated type and member looked like in both the model and the code when they last agreed. A difference only the model has made since (an addition, a change or a removal) is left to the Software Factory until its change is applied, however many runs that takes. A difference the code made (a property deleted, a type changed) is reported, and so is one both made, since applying the model's change would overwrite the code's: its row ends by saying the model and the code have both changed. Something the model has that neither the memory nor the code has is left to the Software Factory only if its file hasn't been written since the memory was saved: on a machine whose memory predates a member that a pull added to both, deleting it by hand is reported. A type the model removes is left to the Software Factory to delete only while its code still has exactly the members it had when they last agreed, at any access level, so a member added by hand is reported rather than deleted with the file. Property accessors (`private set`, `init`) aren't compared: the model can't state them, and the merge keeps the code's. On a machine's first run, or on CI, there is nothing to remember, so a model-first change is reported until applied. Facts (routes, verbs, binding, security) are compared on every run, since the templates never write them. A fact about a type or member the model has just added, or about an operation whose parameters it changed, is compared on the same run, against the code as it will be once the run's changes are applied: the addition itself isn't drift, but a gap it leaves (an action with no route template or verb attribute) is. While the code it was read from isn't on disk yet, its row points at the file without a line.
- **On CI** (`TF_BUILD` set) the check runs and fails the run, but the report isn't written.
- **Upgrading from an earlier build** that wrote `ServicesDriftReport.md` into the codebase: the first run proposes deleting that file; apply the deletion. Any `guardrails/*.sync.json` belong to the old drift check and can be deleted.

The module also generates four files for AI agents, when Intent.Domain.Guardrails.Contracts isn't installed (otherwise that module writes them):

- `guardrails-model-sync.md` into the application's rules folder (the `AI.Context.Instructions` output): an always-on rule to update the model in the same change as the code, run the Software Factory and resolve the drift it reports.
- `skills/sync-model/SKILL.md` (the `AI.Context.Skills` output): the `/sync-model` skill, which resolves each Wrong line by updating the model, the code, a recipe or the exceptions.
- `skills/configure-drift-check/SKILL.md`: the `/configure-drift-check` skill, which writes and extends the recipe files for whatever technology the code uses.
- `skills/validate-drift-recipes/SKILL.md`: the `/validate-drift-recipes` skill, which runs each designer's ValidateDriftRecipes module task and checks what each recipe reads against the code.

All four cover every Guardrails designer in the application.

Limits: recipes read the code's text, not its meaning, so a value built in a helper method can't be read; it is reported as can't be verified, never passed. The intent associations (entity actions, invocations, published events) aren't checked yet.

## Templates

| Template                        | Model                         | Emits                                                                                                                                                         | Role                                                    |
| ------------------------------- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| ServiceContract                 | Service                       | Concrete type (never an interface) with its operations, empty bodies                                                                                          | `Guardrails.Contracts.Services.Service`                 |
| CommandContract                 | Command                       | Type with its fields as properties; no `IRequest` or other interfaces                                                                                         | `Guardrails.Contracts.Services.Command`                 |
| QueryContract                   | Query                         | Type with its fields as properties; no `IRequest` or other interfaces                                                                                         | `Guardrails.Contracts.Services.Query`                   |
| DtoContract                     | DTO                           | Type with fields as properties and its base DTO                                                                                                               | `Guardrails.Contracts.Services.Dto`                     |
| EnumContract                    | Enum                          | `enum` with its literals and values                                                                                                                           | `Guardrails.Contracts.Services.Enum`                    |
| DomainEventHandlerContract      | Domain Event Handler          | Class only, no members. Handle methods and handler interfaces stay in the code. Generated only when `Intent.Modelers.Domain.Events` is installed              | `Guardrails.Contracts.Services.DomainEventHandler`      |
| IntegrationEventContract        | Message ("Integration Event") | Type with its properties; no base type or interfaces                                                                                                          | `Guardrails.Contracts.Services.IntegrationEvent`        |
| IntegrationCommandContract      | Integration Command           | Type with its properties; no base type or interfaces                                                                                                          | `Guardrails.Contracts.Services.IntegrationCommand`      |
| EventingDtoContract             | Eventing DTO                  | Type with fields as properties and its base Eventing DTO                                                                                                      | `Guardrails.Contracts.Services.EventingDto`             |
| IntegrationEventHandlerContract | Integration Event Handler     | Class only, no members. Handle methods and handler interfaces stay in the code. Generated only when `Intent.Modelers.Services.EventInteractions` is installed | `Guardrails.Contracts.Services.IntegrationEventHandler` |
| ServiceProxyContract            | Service Proxy                 | The client class (or interface) with its operations, empty bodies. Generated only when `Intent.Modelers.Types.ServiceProxies` is installed                    | `Guardrails.Contracts.Services.ServiceProxy`            |

Put an Output Anchor carrying these roles at the Application project's root. Designer folders then supply the rest of each file's path. Integration events, integration commands and eventing DTOs often live in a separate Contracts project; give their roles an Output Anchor there instead. API clients usually live in the Infrastructure project; give the ServiceProxy role an Output Anchor there.

## Integration eventing

Integration events, integration commands, eventing DTOs and integration event handlers are guarded the same way as commands, DTOs and domain event handlers. The module depends on `Intent.Modelers.Eventing`, which adds integration events, integration commands and eventing DTOs to the Services designer. Handlers need `Intent.Modelers.Services.EventInteractions`, which it doesn't depend on: without it, there are no handlers to generate.

- **Only this application's own packages.** A subscriber normally references the publisher's eventing package, so its Services designer shows the publisher's messages, eventing DTOs and enums too. The eventing templates, and EnumContract, generate only elements in packages the application owns.
- **A subscriber's own copy of a message.** When the subscriber's code keeps its own class for a message another application publishes, model that copy in the subscriber's own eventing package, where it is generated and guarded like any other message, and link it to the published message. Right-click the copy and choose **Link To Published Integration Event** (the Integration Event Subscription association) for an integration event, or **Link To Sent Integration Command** (Integration Command Subscription) for an integration command, and pick the message in the publisher's referenced package. The copy's eventing package must reference the publisher's eventing package, so add that package reference first if it's missing. The link always runs from the copy to the published or sent message, joins an event only to an event and a command only to a command, and documents the flow between the applications; nothing is generated from it.
- **Names** are used verbatim; no `Event` suffix is added.
- **Handlers are class-only.** Handle methods and handler interfaces depend on the message broker (`IConsumer<T>`, `IHandleMessages<T>`, …), so Merge keeps the existing ones. The Subscribe Integration Event and Subscribe Integration Command associations document the subscription; the template doesn't read them.

Positional records (`record OrderPlaced(Guid OrderId)`) use Type Configuration → Type = positional record, as for commands and DTOs.

Not supported yet:

- **Property initializers on messages:** the Eventing designer has no Value on a Message or Integration Command property. Eventing DTO fields do have one.
- **Accessors:** every property is emitted as `{ get; set; }`, as for DTOs. Merge leaves an existing property's accessors alone, so `init`, `private set` and get-only properties in the code stay as written; only a newly added property gets `{ get; set; }`.

## Service proxies

Each API client in the code (a typed `HttpClient` wrapper, or a Refit, NSwag or Kiota client) is modelled as one **Service Proxy** in the Services designer, purely as a picture of the code, and ServiceProxyContract merges it into the client's own file. The module has no dependency on `Intent.Modelers.Types.ServiceProxies`: the template generates nothing until that modeler is installed and proxies are modelled. When it is installed, it must be 5.5.8-pre.1 or later, which can create a proxy by script with no mapping dialog, in a folder or at the Services package root, keeps syncing parameters when a proxy has an operation not yet mapped to an endpoint, and lets a proxy's operations share a name, so overloads can be modelled.

- **No mappings.** A proxy's Operations and Parameters are the client's methods and parameters. The template never reads a mapping, so a proxy needs none to another application's endpoints.
- **One proxy, one file.** A proxy is the concrete client class (`PaymentsClient`) by default. Set **Service Proxy Configuration → Type** to `interface` only for a client whose implementation is generated at runtime, such as a Refit interface (`IGitHubApi`). The interface a concrete client implements isn't modelled or guarded: it stays hand-written, so a signature change in the model needs a matching hand edit there.
- **Methods:** one per Operation, named verbatim, with an empty body; overloads are Operations with the same name. A class method takes its modifiers from the proxy Operation's own **Operation Configuration**, which has the same properties and rules as a service operation's (`public` without it), plus `async` per the async rules (see Conventions). An interface method has no modifiers and is never `async`, and an Operation Configuration that declares any raises an error on the operation. Return and parameter types follow Collection Type, parameters take **Parameter Modifier**, and the async shape and CancellationToken follow the proxy Operation's own **Async Configuration**, which has the same properties as a service operation's.
- **Default values:** a proxy Parameter has no Value, so apply **Default Value** and set **Value** where the code declares a default (`int page = 1`). It's formatted as described under Default values.
- **Modifiers:** a class takes Default Type Modifiers, or **Service Proxy Configuration → Modifiers**, with the same rules as Type Configuration. An interface takes only the setting's access modifier, or its own Modifiers: at most one of `public` or `internal`, plus `partial`, or `none`. Anything else raises an error on the proxy.
- **Kept from the code:** base types (`: IPaymentsClient`), attributes (Refit's `[Get("/users/{user}")]`), constructors and fields aren't generated, so Merge keeps them.
- **Contract types:** a request or response type the code declares itself is modelled as a DTO, Command, Query or Enum and guarded by its own template. A type from outside the code, such as one from a NuGet contracts package, is a Type-Definition and isn't generated.
- **Only this application's own packages** are generated, as for integration eventing.
- **Don't install Intent's own client generators** (`Intent.Application.Contracts.Clients`, `Intent.Integration.HttpClients`) alongside this module: they fail on unmapped proxies.

```csharp
// Service Proxy PaymentsClient, Service Proxy Configuration → Modifiers = internal sealed
internal sealed class PaymentsClient
{
    public async Task<PaymentDto> GetPaymentAsync(Guid id, CancellationToken cancellationToken = default)
    {
    }

    public async Task<List<PaymentDto>> ListPaymentsAsync(int page = 1, CancellationToken cancellationToken = default)
    {
    }
}

// Service Proxy IGitHubApi, Type = interface, Async Configuration → Include Cancellation Token unticked
public interface IGitHubApi
{
    Task<GitHubUser> GetUserAsync(string user);
}
```

Merged into the existing files, the class keeps its `: IPaymentsClient`, constructor, fields and method bodies, and the interface keeps its Refit attributes.

Not supported yet:

- Explicit interface implementations (`Task<PaymentDto> IPaymentsClient.GetPaymentAsync(...)`).
- Generic methods: the designer allows no generic parameters on a proxy Operation.

## Type Configuration stereotype

Every template except EnumContract emits a `class` by default. Apply **Type Configuration** to a Service, Command, Query or DTO and set **Type** to `record` to emit a `record` instead, for example for a DTO that is a record in code. An empty Type, or no stereotype, means `class`.

- The Type must match the kind of the existing type on disk. Otherwise the merge adds a second declaration with the same name and the project won't compile.
- A DTO and its base DTO must both be records (record or positional record) or both be classes, because C# doesn't allow a record and a class to inherit from each other. A mismatch raises an error on the DTO.
- **Type = positional record** emits a record whose fields are its primary constructor's positional parameters, in model order: `public record CancelOrderCommand(int OrderNumber);`. A field's Value becomes the parameter's default, and Collection Type applies as for properties. It applies to Commands, Queries, DTOs, integration events, integration commands and Eventing DTOs, not to a Service. A positional record whose base is a positional record must declare the base's parameters among its own fields; they are passed on by name (`record ChildDto(int Id, string Name) : BaseDto(Id)`). A record with both a parameter list and body properties isn't supported.
- Integration events, integration commands and eventing DTOs have their own **Type Configuration** with the same Type and Modifiers. An eventing DTO and its base eventing DTO must also have the same Type.
- Structs are not supported, because Intent's C# file builder can't declare them.

## Type modifiers

Under Merge mode the generated declaration's modifiers replace the existing ones, so they must match the code.

- **Default Type Modifiers** (module setting, in Services Guardrails Contracts Settings) applies to every class, record and enum contract: `public` (the default) or `internal`, each optionally followed by `sealed`, `partial` or `sealed partial`. For code that declares types without an access modifier, choose `sealed`, `partial`, `sealed partial`, or `none` for no modifiers at all. Set it to the codebase's own convention, e.g. `internal sealed` or `sealed`. The Domain contract module has its own setting.
- **Type Configuration → Modifiers** replaces the setting for one element, e.g. `public`, `sealed`, `internal static` or `public sealed partial`. Use at most one of `public` or `internal`, plus any of `sealed`, `abstract`, `static` and `partial`, in any order; they are written in the usual C# order. Leave the access modifier out when the code has none (`sealed` gives `sealed class CreateOrderCommandHandler`), and use `none` on its own when the declaration has no modifiers at all (`record OrderPlaced`), because an empty Modifiers means the setting applies.
- **Domain Event Handlers and Integration Event Handlers** share a Type Configuration with Modifiers only, because a handler is always a class.
- **Service Proxies** set Modifiers on **Service Proxy Configuration**. A proxy that is an interface takes only an access modifier and `partial` (see Service proxies).
- **abstract:** Is Abstract on a DTO or eventing DTO always adds `abstract`.
- **sealed from the setting** is left off an abstract DTO or eventing DTO, and off one that another DTO (or eventing DTO) inherits from.
- **Errors:** Modifiers set on an element are used as given. These raise an error on the element: `sealed` or `static` on an abstract or inherited type, `static` on a record, two access modifiers, more than one of `sealed`, `abstract` and `static`, `none` with any other keyword, or a keyword the builder can't write (`file`, `unsafe`, …).
- **Enums** take only the setting's access modifier: `internal sealed` gives `internal enum`, and a setting without one, such as `sealed` or `none`, gives `enum`. Where an enum differs, apply **Enum Configuration** and set **Access Modifier** to `public`, `internal`, or `none` for an enum declared without one.

## File Config stereotype

Each element is generated into `<ElementName>.cs` in its folder, with the namespace derived from its folders. Apply **File Config** to a Service, Command, Query, DTO, Enum, Domain Event Handler, Integration Event, Integration Command, Eventing DTO, Integration Event Handler or Service Proxy only where the code differs. Both properties are optional; set just the one you need.

- **Namespace:** replaces the derived namespace, for a file whose namespace doesn't follow its folders (e.g. `Basislink.DTO` in a sub-folder). It is used as given, with no casing changes, and references to the type get the matching `using`.
- **No Namespace:** tick it for a type declared in the global namespace (no `namespace` declaration in the file). Leave Namespace empty; setting both is an error. References to the type need no `using`.
- **Filename** (with or without `.cs`; a file name only, as the folder stays the element's own) covers the cases below.

- **File named differently from its type:** `psMembersDTO.cs` holds `PsMembersDTO`, so set `Filename = psMembersDTO`.
- **File name that starts or ends with a space**, e.g. `abc .cs`: include `.cs`. With `.cs` the name is used exactly as written; without it, surrounding spaces are trimmed.
- **File holding several types:** give each type that isn't the file's namesake the file's name, e.g. `OrderLineDto` with `Filename = OrderDto` joins `OrderDto.cs`. All contract elements that end up at the same path are generated into one file. The element whose name matches the file name writes it (otherwise the first by name), and the other elements' templates write no file of their own. References to every type in the file still resolve, with the right `using`.
- **Changing it later:** if a type already had its own generated file and you then point it at another file, or change its Filename, the Software Factory offers to delete the old file. Review that before applying.
- **Shared files and namespaces:** every type in one file must end up with the same namespace. If the file's namespace isn't the derived one, give each of its types the same File Config Namespace.
- **Invalid values:** a Filename with a folder in it (`/` or `\`) or invalid file-name characters, or a Namespace that isn't a valid C# namespace, raises an error on the element.

## Collection Type stereotype

Collections are generated as `List<T>` by default. Where the code uses a different collection, apply **Collection Type** to the DTO field (also on commands and queries), the operation parameter, the operation itself (its return type), an integration event, integration command or eventing DTO property, or a Service Proxy Operation (its return type) or Parameter, and choose the **Type**:

| Type                                                                                     | Emits                                                                                                               |
| ---------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| IEnumerable, ICollection, IList, IReadOnlyCollection, IReadOnlyList, List, HashSet, ISet | `IEnumerable<T>`, `ICollection<T>`, … (`System.Collections.Generic`)                                                |
| IQueryable                                                                               | `IQueryable<T>` (`System.Linq`)                                                                                     |
| Array                                                                                    | `T[]`                                                                                                               |
| Custom                                                                                   | **Custom Format**, with `{0}` for the element type, e.g. `System.Collections.ObjectModel.ObservableCollection<{0}>` |

The Type must match the code. Under Merge mode, the generated signature replaces the existing one, so a wrong collection type rewrites the method. On an async operation the return type is wrapped after the collection is applied, giving `Task<IEnumerable<T>>`. The stereotype is ignored on a type that isn't a collection.

## Property Configuration stereotype

Apply **Property Configuration** to a DTO field (on a Command, Query or DTO), a Message or Integration Command property, or an Eventing DTO field whose declaration isn't plain `public`. Properties are `public` without it.

- **Required:** tick it when the code declares the property `required`, giving `public required string Name { get; set; }`.
- **Access Modifier:** `public` (the default), `protected`, `internal`, `private`, `protected internal`, or `none` for a property declared without one. For example, `internal` gives `internal string Name { get; set; }`.
- **Why it matters:** under Merge mode a property's modifiers follow the generated ones. On the first merge into an existing file every existing modifier is replaced, so an unmodelled `required` would be dropped and an unmodelled `protected` or `internal` would become `public`.
- **Accessors need nothing:** Merge keeps an existing `init`, `private set` or get-only accessor as written, so `public required string Name { get; init; }` stays as it is.
- **Positional records:** their fields are constructor parameters, so both options are ignored on them.

## Operation Configuration stereotype

Method signatures are fully managed under Merge, so a service operation's modifiers replace the existing ones and must match the code. Operations are `public` without the stereotype. A Service Proxy Operation has its own Operation Configuration with the same properties and rules, which applies only when the proxy is a class (see Service proxies).

- **Access Modifier:** `public` (the default), `protected`, `internal`, `private`, `protected internal`, or `none` for a method declared without one.
- **Static**, **Virtual**, **Override**, **Sealed** and **New** checkboxes. Sealed is only valid with Override (`public sealed override`). Service operations have no Is Static of their own, so `static` is ticked here.
- `async` comes from **Async Configuration**.
- **Errors:** a combination C# rejects raises an error on the operation: `static` with `virtual`, `override` or `sealed`; `virtual` with `override`; `sealed` without `override`; `new` with `override`; or `virtual` or `override` on a `private` (or `none`) method.
- `private protected` can't be reproduced.

## Parameter Modifier stereotype

Apply **Parameter Modifier** to a service operation or Service Proxy parameter where the code declares a modifier, and set **Modifier** to `ref`, `out`, `in`, `ref readonly` or `params`. It must match the code: under Merge mode the generated signature replaces the existing one, so a missing or wrong modifier rewrites the method. For `params`, model the parameter as a collection and set Collection Type to `Array` (`params string[] tags`). `this` (extension methods) is not supported.

## Default values

Set the Value of a service operation parameter to give it a default (`string area = "Basislink"`), and the Value of a DTO, command, query or eventing DTO field to give its property an initializer (`public string Area { get; set; } = "Basislink";`). A Service Proxy Parameter has no Value, so its default is the **Default Value** stereotype's Value; applying the stereotype with an empty Value raises an error on the parameter. The Value is formatted the way Intent's DTO templates format it:

- **Strings** are quoted, unless the value is already quoted, is `null` or `default`, or looks like an expression (contains `.` or `(`, or starts with `_`). `Basislink` and `"Basislink"` both give `"Basislink"`.
- **Enums:** a literal name gets its enum type (`Info` gives `EventTypeEnum.Info`).
- **Dates:** a bare `DateTime`, `Date` or `DateTimeOffset` member gets its type (`Now` gives `DateTime.Now`).
- **`@`** passes the rest through verbatim (`@DefaultArea` gives `DefaultArea`).
- Anything else (`null`, `false`, `0`, `string.Empty`) is used as written.

The default must match the code: under Merge mode the generated signature replaces the existing one, so a missing Value drops the default. A `ref`, `out` or `params` parameter can't have a default, and giving one a Value raises an error on the parameter.

## Generated shape

```csharp
[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]

namespace Acme.Application.Orders
{
    public record OrderDto
    {
        public Guid Id { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }
}
```

Every file carries both directives: plain `Mode.Merge` for the file, plus a default for constructors and methods that fully manages their signatures and merges their attributes and comments. When merged into an existing file:

- A modelled member that is **missing** is added with the generated signature and an empty body. A new method that returns a value won't compile until its body is written.
- A modelled member that **already exists** is merged too: its signature (modifiers, return type, parameters) is brought in line with the model, and its body is merged with the empty generated body, so the existing statements stay as they are. Review the Software Factory diff before applying.
- Hand-written members that were never modelled are kept. A member that Intent itself added can be removed again when it is deleted from the model, depending on the application's RoslynWeaver "Merge Behaviour" setting.
- **Formatting:** keep RoslynWeaver's "Auto Format by Default" setting on. It lays out every merged file to the C# style rules (indentation, spacing, line breaks), including inside existing method bodies, so the first merge can reformat many lines. It never changes what the code does. With it off, the members the Software Factory rewrites lose their indentation.

## Conventions

- Every name is used exactly as it appears in the designer, with no casing changes: types, properties, methods, parameters, enum literals and namespaces. `OrderService` stays `OrderService` (no interface is generated), and a field modelled as `customerId` stays `customerId`. The designer mirrors the code, so its casing is the codebase's own convention. Set the Services designer's Service Settings naming conventions (Property, Entity, Operation and Parameter) to Manual, so the designer doesn't rename what was modelled. Parameter Naming Convention needs Intent.Modelers.Services 4.0.17 or later, which this module depends on; it defaults to Camel Case.
- Collections are `List<T>`, unless Collection Type says otherwise. They are written by their short name with a `using` (`IEnumerable<T>`, not `System.Collections.Generic.IEnumerable<T>`), even in a file that also imports `System.Collections`.
- An operation is async when its name ends in `Async`, unless the **Async Configuration** stereotype says otherwise. An async operation gets the `async` keyword, a `Task` or `Task<T>` return type (a modelled `Task`/`ValueTask` is not wrapped again) and a `CancellationToken cancellationToken = default` parameter. A new async method's empty body has no `await`, so the compiler warns (CS1998). The stereotype's properties:
  - **Async Mode** (default `async`): `async`; `no async keyword`, the same return type and token without `async`, for a method that returns its task directly (`public Task<OrderDto> GetAsync(Guid id, CancellationToken ct) => _inner.GetAsync(id, ct);`); or `sync`, the return type exactly as modelled with no token.
  - **Task Type**: `Task`, `ValueTask` or `void`. Empty means `Task`, or `ValueTask` when the return type is modelled as one; an explicit `Task` or `ValueTask` converts a modelled `Task`/`ValueTask` to the other. `void` writes `async void` (`public async void OnMessage(...)`), e.g. for an event handler; it needs Async Mode `async` and an operation with no return type, and anything else raises an error on the operation.
  - **Include Cancellation Token** (default ticked) adds the token parameter.
  - **Cancellation Token Name**: the token's name when the code doesn't call it `cancellationToken`, e.g. `ct`. Method signatures are fully managed, so any other name would otherwise be renamed.
  - **Cancellation Token Default** (default ticked) writes `= default`; untick it for a token without a default.
  - A `CancellationToken` that isn't the last parameter can't be reproduced.
  - Service Proxy Operations have their own Async Configuration with the same properties and rules. An interface method never gets the `async` keyword.
- Fields typed as Domain enums, entities, value objects or data contracts resolve through the Domain contract roles, with no reference to the Domain contract module.
- Consumer projects need the `Intent.RoslynWeaver.Attributes` NuGet package.
