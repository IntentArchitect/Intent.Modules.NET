# Rules (blocks 3 and 4)

A rule reads a claim's value from the code graph and returns a [reading](claims-and-findings.md#the-reading) ([R45](requirements.md)). There are two kinds:

| Kind         | For                                                                                       | Written as                                                   |
| ------------ | ----------------------------------------------------------------------------------------- | ------------------------------------------------------------ |
| Structure    | Types, members, signatures and enums, for any language with a language profile (block 3)  | built in, configured by conventions and the language profile |
| Claim reader | Anything else: a framework's code, or what a house's own stereotypes state                | C#, in a module's extension ([Plug-ins](#plug-ins))           |

Reading code is C# only. A pack is data, its vocabulary, conventions and language profiles, and never says how to read code (user decision, 2026-11-16: recipes, a JSON way of reading code beside claim readers, were removed, so a check has one way in).

## Choosing a rule

For each claim, the extensions' claim readers come first, the most recently registered first, then the engine's structure rules. The engine registers the stock extensions before any module's, so a module's reader is asked before a stock one, and can read any kind, the engine's included ([R46](requirements.md)). A reader returns nothing for a claim it doesn't read, and the next is asked; a claim nothing reads is `noRule`. So a house that disagrees with a stock reading for some of its code reads just those claims and leaves the rest to the stock reader.

A reader is keyed by kind, not by the pack whose vocabulary produced the claim. A framework's reader therefore reads claims a module's or a house pack's vocabulary states, such as On Delete from eShopCustom's own stereotype ([packs.md](packs.md#who-ships-what)).

## Structure (block 3)

Structure claims are compared from the code graph, describe-only: nothing is generated and no template is involved ([R41](requirements.md)). Common.Drift compared each type a Guardrails contract template generated with its file; that tied structure to C# and to the templates. The engine instead locates each element's code and compares what the element can state ([R44](requirements.md)).

### Locating an element's code

1. **Types.** The pack's conventions give, per element type, the code type kinds allowed (`Class` → `class` or `record`; `Enum` → `enum`), an optional name pattern (`I{Name}` for a Repository's interface) and optional locations (globs such as `src/Contracts/**`). The element's type is the in-scope type node with that name (by default the language profile's naming), of any kind, within those locations. An element type whose kinds include both `class` and `interface` (a Service, Service Proxy, Domain Service or Repository) with no type of its name, and no name pattern, is the interface its language names it (C#'s `IPricingService`), which a class of another name implements. None gives `notFound` (so `type.exists` is contradicted), and several give `ambiguous` with every candidate ([R43](requirements.md)). The allowed kinds are `type.kind`'s expected value, so a type of the wrong kind is found and reported as the wrong kind, not as missing.
2. **Members.** Inside the located type: an attribute is a property (or, in Java, a field with its accessors); a constructor is a constructor; a parameter is a parameter by name; an enum literal is an enum member. An attribute with a `member.backingField` claim is its property, or, when the type has no property of that name, the field the claim names.
3. **Operations.** An operation is the method of its name. When several overloads have that name, the one whose parameter names are the claim's `parameterNames`, in order, is chosen, and of several with those names, the one whose parameter types match too (basislink5 has `Update(BidsDTO bid)` beside `Update(bids bid)`); failing that, the one with as many parameters; failing that, `ambiguous`. A single method of that name is the operation's method whatever its parameters, so a wrong parameter list is a contradicted `operation.parameters`, not a missing member.

The packs' conventions also name **injected parameters**: parameters the framework supplies, which are part of no operation's signature, such as `CancellationToken`, `ServerCallContext` and anything marked `[FromServices]`. `operation.parameters` and the `parameter.*` kinds skip them.

#### Which conventions apply

Conventions are keyed by element type, and each may be narrowed by a `where` expression, so an application service and a hosted service can be located differently. When two packs give conventions for the same element type that both apply to an element, the pack that requires the other wins: a house pack's conventions override a stack pack's, and a stack pack's override the core's. Two packs neither of which requires the other are a `config/conventionConflict` error for that element type.

### What is compared

| Kind                                                        | Model                                          | Code                                                       | Comparer                                       |
| ----------------------------------------------------------- | ---------------------------------------------- | ---------------------------------------------------------- | ---------------------------------------------- |
| `type.exists`                                               | the element                                    | its type node                                              | `presence`                                     |
| `type.kind`                                                 | the code kinds the conventions allow           | `typeKind`                                                 | `within`                                       |
| `type.abstract`                                             | `abstract` on the element                      | the `abstract` modifier                                    | `exact`                                        |
| `type.generalization`                                       | the generalization's target                    | the base class, by name, resolved where possible           | `type`                                         |
| `type.generics`                                             | the element's generic type parameters          | `typeParameters`                                           | `list`                                         |
| `member.exists`                                             | the attribute, operation or constructor        | the member node                                            | `presence`                                     |
| `member.type`                                               | the attribute's type reference                 | the property's type ref                                    | `type`                                         |
| `member.nullable`                                           | the type reference's nullable                  | the type ref's `nullable`                                  | `exact` (an `unknown` reading is `unresolved`) |
| `member.static`                                             | `static` on the element                        | the `static` modifier                                      | `exact`                                        |
| `operation.returns`                                         | the operation's type reference                 | the return type, unwrapped of the profile's async wrappers and the conventions' result wrappers | `type`                    |
| `operation.parameters`                                      | the parameters, in order, each `name: type`    | the method's parameters, less injected ones                | `list` of `type`                               |
| `parameter.type`, `parameter.nullable`, `parameter.default` | the parameter's type reference and default     | the parameter node                                         | `type`, `exact`, `exact`                       |
| `enum.literals`                                             | the literals, in model order                   | the enum members                                           | `subsequence`                                  |
| `enum.literal.value`                                        | the literal's value, where modelled            | the member's `value`                                       | `number`                                       |

All but `parameter.default` and `enum.literal.value` are `stated: always`: a class with no `abstract` modifier states that it isn't abstract. A modelled member that isn't there is contradicted ([R8](requirements.md)). An unmodelled member is never a finding unless the `unmodelled.member` opt-in rule is on.

Four more structure kinds take their expected value from vocabulary and conventions rather than from the element alone: `type.location` (where the type's file must be), `type.base` (a base type the model implies, such as `IAggregateRoot` for an aggregate root), `type.marker` (a named marker, present exactly when modelled) and `file.exists` (a file an element stands for, such as a Vue component). [claim-catalog.md](claim-catalog.md#structure) lists every structure kind.

### Language profiles

A language profile is pack data ([R42](requirements.md)). Its schema is [`schemas/language-profile.schema.json`](schemas/language-profile.schema.json). One run may read several languages, and each file's code is compared under its own language's profile: an element's type is looked for under the name each language gives it, and a member, a type reference or a parameter list is read as its own language writes it.

| Section       | Says                                                    | C# example                                           | Java example                              |
| ------------- | ------------------------------------------------------- | ---------------------------------------------------- | ----------------------------------------- |
| `types`       | Each model type's names in code                         | `datetime` → `DateTime`, `System.DateTime`           | `datetime` → `LocalDateTime`, `Instant`   |
| `collections` | The generic types and forms that mean "a collection of" | `List<T>`, `ICollection<T>`, `IEnumerable<T>`, `T[]` | `List<T>`, `Set<T>`, `Collection<T>`      |
| `async`       | Wrappers unwrapped from return types                    | `Task<T>`, `ValueTask<T>`; `Task` means void         | `CompletableFuture<T>`, `Mono<T>`         |
| `nullable`    | How a type ref says nullable                            | `T?` in a nullable context; `Nullable<T>`            | `@Nullable`; a boxed type for value types |
| `naming`      | From an element name to a type, member and file name    | interface `I{Name}`; the rest as modelled            | field, method `{Name\|camel}`             |
| `typeKinds`   | The code kinds the language has                         | `class`, `record`, `struct`, `interface`, `enum`     | `class`, `record`, `interface`, `enum`    |

The `type` comparer maps the model's type through `types` and `collections`, then compares with the code's type ref: model `OrderItem[]` matches C# `ICollection<OrderItem>` and Java `List<OrderItem>`. Nullability isn't part of it: model `guid?` against C# `Guid` is one finding, from `member.nullable` or `parameter.nullable`, never a second from `member.type`. A model type that is a modelled element matches the code type that element is located as: where the reader resolves names, the code's type ref must resolve to that node, so a same-named type in another namespace doesn't match; where it can't resolve, the names are compared. A library type, or a primitive the profile doesn't list, is compared by name.

Result wrappers beyond async ones belong to frameworks, so the conventions of a framework pack list them, with `T` for the result and `…` for any further type arguments: ASP.NET Core's `ActionResult<T>`, `Ok<T>`, `Results<Ok<T>, …>`; Spring's `ResponseEntity<T>`. `operation.returns` unwraps them as it unwraps `Task<T>`. A return type that names no result, ASP.NET Core's `IActionResult`, `ActionResult` and `IResult` or Web API 2's `IHttpActionResult` (as Spring's `ResponseEntity<Object>`), is read from the method's body: what each return's success result carries, `Ok(data)` being `data`'s type and `NoContent()` nothing. Error results aren't modelled; a result the stack doesn't read, or a body whose type the code doesn't say, is can't verify.

## Find paths

A convention's `find` ([`schemas/conventions.schema.json`](schemas/conventions.schema.json)) reaches places in the code graph: the code an `unmodelled.*` opt-in rule looks at, or the types a type convention narrows to. Its steps are joined by ` > `, each a kind of node, optionally `with` a .NET regular expression its canonical text must match somewhere ([code-reader.md](code-reader.md#canonical-text)). `type:` (or `class:`) starts it; `method`, `property`, `field`, `constructor` and `member` go into a type's members, `parameter` into a member's parameters, `call` into a member's calls, `chain` into its call chains, `annotation` onto the node before, and `markup` into the markup elements of a type's files or of the element before. `^` and `$` anchor to the start and end of a node's text. A markup element's text includes its children's, so `markup with` keeps only the innermost elements that match.

Names in a find path, and a convention's `name`, are `{Name}` placeholders, filtered and chained as Common.Drift's were: `camel`, `pascal`, `kebab`, `lower`, `upper`, `trimStart:`, `trimEnd:` and `replace:` (`{Name|trimEnd:Service}Controller`).

A find path only finds code. Reading a value from what it finds is a claim reader's job.

## Plug-ins

A module extends the engine in C#, through its extension (`DriftEngineExtensionBase`), with one or more of these ([R48](requirements.md)):

| Interface          | Given                                                    | Returns                                                       |
| ------------------ | -------------------------------------------------------- | ------------------------------------------------------------- |
| `ICodeReader`      | in-scope files of its languages, and the project nodes   | graph nodes and edges, and its capabilities                   |
| `IFrameworkReader` | committed artifacts its patterns match                   | framework nodes, located in the artifacts                     |
| `IClaimReader`     | one claim (kind, element, bindings) and the scoped graph | a reading, or nothing to leave the claim to the next rule     |
| `IValueComparer`   | the expected value and a reading                         | verified or contradicted, with the text to show for each side |
| `IModelFunction`   | a model element and arguments                            | a value; it declares the stereotypes and properties it reads  |
| `IOptInRule`       | the scoped graph and the claims' readings                | the code it finds unmodelled                                  |
| `IDriftRule`       | the scoped graph, the model and the claims' readings     | findings that are no one claim's, under its own id            |

A claim reader that finds the code it needs only in a file the Drift Scope leaves out says so (`ClaimReading.OutOfScope`), and the claim is can't verify, `outOfScope`, naming the scope row. One that finds nothing anywhere states silence without a place (`ClaimReading.Absent(null, value)`).

An `IDriftRule` has an id (`messaging/unmodelled`, `aspnetcore/conventional.route`), and only the last registered rule of each id runs: a module's rule replaces a stock one of its id (user decision, 2026-11-16), and the Software Factory log says which it replaced (`scope/replacedRule`). A house that disagrees with a stock rule copies it and changes it, or turns it off with one of the same id that reports nothing. An opt-in rule is replaced the same way, by its id.

The rules a plug-in must follow:

- It sees only in-scope code, through the graph. It never opens files itself.
- It never writes anything: no files, no model.
- It throws only for its own bugs. A throw is caught and becomes `ruleFailed` on each claim it was reading, naming the module.

## Opt-in "must be modelled" rules

By principle 1, code the model doesn't mention is never drift. A team that wants some kind of code always modelled turns a rule on in the model's `Drift Check` stereotype ([scope.md](scope.md#the-drift-check-stereotype)). Each rule applies within scope only, and each finding can be excepted, with a reason ([R49](requirements.md)).

| Rule                     | Reports                                                                                                                      | Replaces                                 |
| ------------------------ | ---------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------- |
| `unmodelled.type`        | An in-scope type under the row's Glob (`Basislink.Data/Model/Entities/**`) that no claim located                             | Common.Drift's `own` conventions         |
| `unmodelled.member`      | A public member of a located type, of a kind its element type can state, that no claim located                               | Common.Drift's unmodelled public members |
| `unmodelled.interaction` | A call the model's interactions can state (a repository's `Add` in a handler) that no `intent.*` claim located               | eShopCustom's code-only interactions     |
| `unmodelled.exposed`     | An in-scope type an endpoint exposes (a request or response type) that no element describes                                  | eShopCustom Services' exposure check     |
| `unmodelled.file`        | An in-scope file in which no claim located anything                                                                          | eShopCustom's Strict scope mode          |
| `unmodelled.value`       | An explicit value the code states for a `whenSet` kind that the model leaves unset (a `ToTable` with no modelled table name) | decision 6                               |
| `unmodelled.index`       | An index the EF Core configuration declares (`HasIndex`) that no Index element or Index stereotype describes                  | Common.Drift's `own` entry for indexes   |
| `unmodelled.endpoint`    | A Minimal API endpoint, a lambda or a method group, that no Operation's `endpoint.exists` located                             | Common.Drift's `own` entry for endpoints |

Code is **located** when some claim's reading came from it or from code inside it, as Common.Drift's ownership is. An element's type is located by its `type.exists` reading, so "the type an element is located as" and "code a claim located" are the same idea. A located class's interfaces are located with it, since a Service's, Domain Service's or Repository's interface beside its class is no element of its own; the other way round, such an element located as its interface locates the classes implementing it. A subscription's reading locates every method of the handler that takes its message.

What each rule looks at, precisely:

- **`unmodelled.type`** reports the in-scope types under the row's Glob that no claim located, so the row alone sets it up, as a Common.Drift `own` entry's `files` did (user decision, 2026-11-16). A type nested in another is part of it, and what a file declares outside its types (TypeScript's `module`) isn't a type. A finding names the designer whose located types are in the type's folder, or the nearest folder above it, so a new entity beside the modelled ones is reported with the Domain designer's findings; with none, it is reported with the scope's. A module that wants more, such as only a base class's subclasses, gives an element type's convention a `find`: within the convention's `files` only the types it reaches (and of its `typeKinds`) are reported, as that element type, and outside them the row alone decides.
- **`unmodelled.member`** looks inside located types only, as do the conventions' `members` entries. It reports the public properties, fields, methods, constructors and events of a kind the element type can state (its type convention's `states`: a Class's properties, fields, constructors and methods, a DTO's properties and fields) that no claim located. An override of a modelled base type's member, a record's primary constructor, a located field's accessors (the language profile's `accessors`, as Java's `getName`) and the members every type has (`Equals`, `GetHashCode`, `ToString`; Java's `equals`, `hashCode`, `toString`) are never reported. An override of a framework's base (a hosted service's `StopAsync`) is the type's own, and is. A `members` convention's find path may end inside a member, at a parameter or a call; the hit is unmodelled when no claim's reading came from it. Parameters aren't members of their own: a parameter list is `operation.parameters`.
- **`unmodelled.interaction`** reports the calls the conventions' `interactions` reach inside located methods that no `intent.*` claim located.
- **`unmodelled.exposed`** follows what an endpoint exposes: each located operation's parameter types and unwrapped return type, then, transitively, the types of those types' public properties. It reports each in-scope type on the way that no element describes. Library types and generic type parameters are skipped.
- **`unmodelled.file`** reports in-scope files in which no claim located anything.
- **`unmodelled.endpoint`** (ASP.NET Core's) reports each Minimal API endpoint the code maps, a lambda or a method group, that no `endpoint.exists` located, named by its verb and route. A controller's actions are public methods, which `unmodelled.member` reports, and a router or controller no Service describes is `unmodelled.type`'s, so neither is reported twice.
- **`unmodelled.value`** runs a `whenSet` kind's rule for each element of the specialization the kind's vocabulary entry selects (its `for`, ignoring `where` and `when`) that has no claim of that kind, and reports each value read from code. Silence is never a value, so a framework convention is never reported.

A finding's subject is where the code is: the line a node starts on (its first attribute or annotation, if it has any), or line 1 for a file. It is in the Area of the designer whose convention or element found it (`Domain (DriftEngine)`), and names the element whose type holds the code, if any; `unmodelled.file`'s is in `Drift Scope (DriftEngine)`. A rule reports nothing in a file a `Context` scope row decides: that code is read, not checked ([scope.md](scope.md#what-scope-decides)).

A module adds an opt-in rule as an `IOptInRule` (`OptInRules` on its extension): given the run, it returns the code it finds unmodelled, using what the claims located (`RuleContext.IsLocated`), and the engine applies everything else above the same way as for its own rules. A module's rule of the same id replaces the engine's. A rule turned on whose convention has a find path the engine can't read is a `config/ruleInvalid` error, and the rule doesn't run.

An exception is a row of `Rule`, `Match` and `Reason` on the `Drift Check` stereotype. `Match` lists one or more entries, one per line, each a file glob (relative to the output folder), a type name, a `Type.Member`, or the name a finding gives, so code skipped for one reason is one row; the designer shows a row as its entries and reason. An entry that matches nothing is an `optin/staleException` warning naming it; one with no reason is a `config/exceptionReason` error. Both name the Root Folder that holds the row, and the row.

Each opt-in rule has one default severity, an error. A team that wants some of a rule's findings as warnings overrides the rule's severity ([claims-and-findings.md](claims-and-findings.md#overrides)); eShopCustom's notes-only checks become errors unless it does.

Opt-in rules are policy (block 5) and arrive in phase 1 with scope. Each case's phase is the latest phase of the packs and kinds it relies on: a rule over Domain conventions is phase 1, one over EF Core configuration phase 4.

## Messages the diagram could show

Domain events, integration events and integration commands are the one exception to principle 1 (user decision, 2026-11-14): they are always significant, so they are checked both ways by default, with no `Drift Check` row. The test is whether the diagram could show it: "if the diagram can't reflect it, it's not drift; if the diagram could reflect it and they're not in sync, it's drift." Other interactions stay case by case: a query that fetches the aggregate a service exists for may matter, incidental lookups are noise, so `unmodelled.interaction` stays opt-in.

The messaging pack, `intent.messaging`, reads them whatever the codebase raises them through (`AddDomainEvent`, an outbox, a bus, its own wrapper):

- **A link** (`Domain Event Origin`, `Publish Integration Event`, `Send Integration Command`) is an `intent.raises`, `intent.publishes` or `intent.sends` claim on its target end. It is verified when the code it starts from hands a new message of the linked type to a call: a constructor's or operation's method, a command's method that takes it, or a handler's method that takes what it subscribes to, each with the code's own methods it calls, its class's and its collaborators' (a field of an interface the code implements is each implementation). A message handed to a library's method (a bus, an outbox, a list) is raised. One handed to a method of the code is raised only when that method hands the parameter on in turn, into a call that does, something it creates, or something it keeps: a method that only reads it processes it in place. A call in what a method returns, when the method returns the message, builds it for its caller (`return Task.FromResult(message)`). A link says the code can raise the message, not that it always does (user decision, 2026-11-16): a raise in any branch bears it out, and so does a raise of what a method of the code returns, awaited or not, when one of the messages it can return is the linked one (`AddDomainEvent(StatusChanged())`). A message handed on that the code doesn't say, such as a library factory's or one typed only as what the message derives from (`INotification`), is can't verify, `unreadable`. When the code doesn't raise it, the link is contradicted, at the method.
- **A message type the diagram doesn't show** is reported by `messaging/unmodelled.message`, at its declaration, in checked code only. The messages are the modelled ones and every type of the code that has what all the modelled ones of that kind derive from or implement (`INotification`, an `IntegrationEvent` base); with none modelled there is nothing to learn from, and nothing is reported.
- **A raise no link covers** is reported by `messaging/unmodelled.raise`, `.publish` or `.send`, at the call, when the diagram has somewhere to draw the link from, which the fix names:
  - a modelled constructor, operation, command, query or handler subscription whose code reaches the call, itself or through the code's own methods it calls, nearest first: a helper's send is the handler's that calls it;
  - a method of a modelled class or service, which the diagram can show as an operation;
  - a method of a handler of a modelled message, which the diagram can show as a subscription.

  A raise from code the diagram can't show, such as a background router, isn't drift: only its message must be modelled. When the message isn't modelled either, the raise's finding asks for both, and the message has no finding of its own.

A Severity Overrides row can lower a `messaging/*` rule like any other. A per-technology pack (NServiceBus, MassTransit) may sharpen the reading, such as telling a publish from a send, and a house extension may add a mechanism the neutral reading can't see, such as an interceptor that creates events from state changes.

## Routes Web API 2 serves conventionally

ASP.NET Web API 2's attribute routing covers only actions with a `[Route]` of their own. A controller's `[RoutePrefix]` doesn't route the others: Web API 2 serves them on its conventional routes, the first `MapHttpRoute` template with a `{controller}` (`api/{controller}/{id}` by default), with the action's `{controller}` and `{action}` filled in and an optional value it doesn't take (`id = RouteParameter.Optional`) left out. So an unattributed `PostAsync` on `EndowmentController` is at `api/Endowment`, outside its controller's prefix. The `intent.aspnetcore` extension reads it two ways:

- **A Route the Operation states** is compared with that route as an absolute one, `~/api/Endowment`, which is how the model writes a route outside its service's.
- **An empty Route** makes no `operation.route` claim (it is stated only when set), yet it puts the endpoint at its service's Route. When Web API 2 serves the action elsewhere, `aspnetcore/conventional.route` reports it at the action, asking for a `[Route]` in the code or the absolute route in the model. A service whose Route is the conventional route itself (a controller with no prefix, modelled at `api/Claims`) isn't reported.

A parameter's source reads the same route: such an action binds from the conventional template, not its controller's prefix.
