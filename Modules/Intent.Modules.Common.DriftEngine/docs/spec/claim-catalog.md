# Claim catalog

The 321 checks in [check-classification.md](../check-classification.md), reduced to what the engine checks. Each check gets one disposition, and sometimes others it also needs:

| Disposition     | Meaning                                                                                                                                                                    |
| --------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `claim:<kind>`  | A claim kind the model states. A vocabulary entry produces it, and a rule reads it.                                                                                        |
| `scope:<case>`  | Behaviour of scope ([scope.md](scope.md))                                                                                                                                  |
| `engine:<case>` | Behaviour of the engine itself: outcomes, readers, the report                                                                                                              |
| `optin:<rule>`  | An opt-in "must be modelled" rule ([rules.md](rules.md#opt-in-must-be-modelled-rules)): the check reported code the model doesn't describe, which principle 1 makes opt-in |
| `model-rule`    | A rule about the model alone, such as naming or "a Layer needs a Path". It belongs to the designer's own validation, not to drift (decision 11).                           |
| `dropped`       | Not carried over: `inventory` lists code the model can't state (decision 12); `codeOnly` reports code with no model claim and no opt-in rule to carry it (principle 1)     |

Check ids are `<source>-<number>`, numbered as in the classification's tables: `ESC-DOM` (eShopCustom Domain), `ESC-SVC` (Services), `ESC-FE` (Front End, Blazor), `ESC-ARC` (Architecture), `JAV-DOM` (Java spike Domain, JPA), `JAV-SVC` (Spring MVC), `JAV-FE` (Vue) and `JAV-ARC`. The suite's `Acceptance/checks.json` holds every check's dispositions, and `Acceptance/catalog.json` every kind, scope case, engine case and opt-in rule. The suite checks the two against each other and against the classification ([acceptance-suite.md](acceptance-suite.md#what-runs-now)).

## Summary

By each check's first disposition:

| Source    | Checks  | Claim   | Scope  | Engine | Opt-in | Model rule | Dropped |
| --------- | ------- | ------- | ------ | ------ | ------ | ---------- | ------- |
| ESC-DOM   | 73      | 51      | 3      | 4      | 9      | 0          | 6       |
| ESC-SVC   | 87      | 57      | 5      | 3      | 17     | 4          | 1       |
| ESC-FE    | 74      | 43      | 4      | 3      | 15     | 3          | 6       |
| ESC-ARC   | 9       | 2       | 3      | 1      | 1      | 2          | 0       |
| JAV-DOM   | 32      | 23      | 0      | 1      | 3      | 5          | 0       |
| JAV-SVC   | 21      | 13      | 0      | 0      | 6      | 2          | 0       |
| JAV-FE    | 19      | 11      | 0      | 0      | 7      | 1          | 0       |
| JAV-ARC   | 6       | 3       | 0      | 0      | 1      | 2          | 0       |
| **Total** | **321** | **203** | **15** | **12** | **59** | **19**     | **13**  |

203 checks reduce to 101 claim kinds. Six kinds come from Common.Drift's recipes alone, with no eShopCustom or Java check behind them ("conversion only" below); they keep conversion whole.

## Claim kinds

`Stated` and `Comparer` are defined in [claims-and-findings.md](claims-and-findings.md). `Phase` is the plan's phase that must pass the kind's acceptance cases; "later" kinds have cases but no phase gate yet.

### Structure

Compared from the code graph for any language with a profile ([rules.md](rules.md#structure-block-3)).

| Kind                   | Label              | Stated  | Comparer      | Phase | Checks                                                                                                                  |
| ---------------------- | ------------------ | ------- | ------------- | ----- | ----------------------------------------------------------------------------------------------------------------------- |
| `type.exists`          | type               | always  | `presence`    | 1     | ESC-DOM 12, 69; ESC-SVC 11, 82; ESC-FE 39, 50; JAV-DOM 2; JAV-SVC 2, 18; JAV-FE 3                                       |
| `type.kind`            | kind               | always  | `within`      | 1     | ESC-DOM 13; JAV-DOM 4                                                                                                   |
| `type.location`        | location           | always  | `exact`       | 2     | ESC-DOM 14, 70; ESC-SVC 12; ESC-FE 9, 40, 51; JAV-DOM 3; JAV-FE 2                                                       |
| `type.abstract`        | abstract           | always  | `exact`       | 1     | ESC-DOM 18; ESC-SVC 42; JAV-DOM 5                                                                                       |
| `type.generalization`  | base class         | always  | `type`        | 1     | ESC-DOM 19; ESC-SVC 44; JAV-DOM 6                                                                                       |
| `type.base`            | required base type | always  | `subset`      | 1     | ESC-DOM 7, 16, 27; ESC-SVC 50, 58, 70, 76; ESC-FE 48                                                                    |
| `type.marker`          | marker             | always  | `exact`       | 1     | ESC-DOM 17, 20; ESC-SVC 43                                                                                              |
| `type.generics`        | type parameters    | always  | `list`        | 1     | ESC-SVC 45; ESC-FE 41                                                                                                   |
| `type.registered`      | registration       | always  | `presence`    | later | ESC-SVC 62, 71, 77                                                                                                      |
| `file.exists`          | file               | always  | `presence`    | 1     | ESC-SVC 75; JAV-FE 2; JAV-ARC 4                                                                                         |
| `member.exists`        | member             | always  | `presence`    | 1     | ESC-DOM 28, 33, 54, 60, 63, 73; ESC-SVC 46, 66, 67, 72, 78, 83; ESC-FE 16, 22, 25, 42, 45, 52; JAV-DOM 11, 20, 26; JAV-FE 8, 17 |
| `member.type`          | type               | always  | `type`        | 1     | ESC-DOM 28, 34, 37, 55; ESC-SVC 46, 83; ESC-FE 17, 23, 43; JAV-DOM 12, 23; JAV-SVC 19                                   |
| `member.nullable`      | nullable           | always  | `exact`       | 1     | ESC-DOM 28, 35, 56; ESC-SVC 46; JAV-DOM 14, 25                                                                          |
| `member.static`        | static             | always  | `exact`       | 1     | ESC-DOM 64                                                                                                              |
| `member.backingField`  | backing field      | whenSet | `exact`       | later | ESC-DOM 40                                                                                                              |
| `member.wireName`      | serialized name    | whenSet | `exact`       | later | ESC-SVC 48                                                                                                              |
| `operation.returns`    | return type        | always  | `type`        | 1     | ESC-DOM 64; ESC-SVC 23; ESC-FE 26, 46, 53; JAV-SVC 8; JAV-FE 9                                                          |
| `operation.nullable`   | return nullable    | always  | `exact`       | 1     | ESC-DOM 64; ESC-SVC 23                                                                                                  |
| `operation.parameters` | parameters         | always  | `list` of `type` | 1  | ESC-DOM 61, 64, 73; ESC-SVC 26, 79; JAV-FE 10                                                                           |
| `parameter.type`       | parameter type     | always  | `type`        | 1     | ESC-SVC 30; JAV-SVC 10                                                                                                  |
| `parameter.nullable`   | parameter nullable | always  | `exact`       | 1     | ESC-SVC 30; JAV-SVC 11                                                                                                  |
| `parameter.default`    | parameter default  | whenSet | `exact`       | 1     | ESC-SVC 31                                                                                                              |
| `enum.literals`        | literals           | always  | `subsequence` | 1     | ESC-DOM 24, 26; ESC-SVC 49; JAV-DOM 28                                                                                  |
| `enum.literal.value`   | literal value      | whenSet | `number`      | 1     | ESC-DOM 25                                                                                                              |

`type.generalization` is the model's own inheritance (a Generalization to another modelled class). `type.base` is a base the model implies without modelling it: eShopCustom's aggregate roots implement `IAggregateRoot` and its entities derive `Entity`, which a house pack's vocabulary states from the element's stereotypes and associations. `type.marker` is a named marker both ways: modelled means present, not modelled means absent.

### Persistence

From the EF Core and JPA packs, phase 4.

| Kind                        | Label                     | Stated  | Comparer     | Phase | Checks                                 |
| --------------------------- | ------------------------- | ------- | ------------ | ----- | -------------------------------------- |
| `table.name`                | table name                | whenSet | `exact`      | 4     | ESC-DOM 52; JAV-DOM 7                  |
| `table.schema`              | schema                    | whenSet | `exact`      | 4     | ESC-DOM 9                              |
| `table.kind`                | table or view             | always  | `exact`      | 4     | conversion only                        |
| `entity.persisted`          | persisted                 | always  | `exact`      | 4     | ESC-DOM 10; JAV-DOM 4                  |
| `key`                       | primary key               | always  | `list`       | 4     | ESC-DOM 8, 36; JAV-DOM 18              |
| `key.generated`             | key generation            | whenSet | `exact`      | 4     | ESC-DOM 47; JAV-DOM 18                 |
| `column.name`               | column name               | whenSet | `exact`      | 4     | ESC-DOM 42, 45; JAV-DOM 15             |
| `column.type`               | column type               | whenSet | `ignoreCase` | 4     | ESC-DOM 46                             |
| `column.textType`           | text type                 | always  | `exact`      | 4     | conversion only                        |
| `column.maxLength`          | max length                | whenSet | `number`     | 4     | ESC-DOM 49; JAV-DOM 16                 |
| `column.precision`          | precision                 | whenSet | `number`     | 4     | JAV-DOM 17                             |
| `column.scale`              | scale                     | whenSet | `number`     | 4     | JAV-DOM 17                             |
| `column.default`            | default                   | whenSet | `exact`      | 4     | conversion only                        |
| `column.defaultSql`         | SQL expression default    | whenSet | `exact`      | 4     | conversion only                        |
| `index.name`                | index name                | whenSet | `exact`      | 4     | conversion only                        |
| `column.mapped`             | mapped                    | always  | `exact`      | 4     | ESC-DOM 39                             |
| `column.owned`              | owned                     | always  | `exact`      | 4     | ESC-DOM 38                             |
| `column.enumStorage`        | enum storage              | whenSet | `exact`      | 4     | ESC-DOM 51                             |
| `column.version`            | concurrency token         | always  | `exact`      | 4     | JAV-DOM 19                             |
| `column.backingField`       | backing field             | whenSet | `exact`      | later | ESC-DOM 41                             |
| `column.hilo`               | hi-lo sequence            | whenSet | `exact`      | later | ESC-DOM 44                             |
| `relationship.multiplicity` | multiplicity              | always  | `exact`      | 4     | JAV-DOM 23, 24, 25                     |
| `relationship.foreignKey`   | foreign key               | whenSet | `list`       | 4     | ESC-DOM 57                             |
| `relationship.joinColumn`   | join column               | whenSet | `exact`      | 4     | JAV-DOM 27                             |
| `relationship.onDelete`     | on delete                 | whenSet | `exact`      | 4     | ESC-DOM 58                             |
| `relationship.navigable`    | navigable                 | always  | `exact`      | 4     | JAV-DOM 26                             |
| `relationship.reference`    | navigation                | always  | `presence`   | 4     | ESC-DOM 59                             |
| `index.columns`             | index columns             | always  | `list`       | 4     | ESC-DOM 48; JAV-DOM 30                 |
| `index.unique`              | unique                    | always  | `exact`      | 4     | ESC-DOM 48                             |
| `repository.entity`         | repository entity         | always  | `type`       | 4     | ESC-DOM 71; JAV-DOM 9                  |
| `repository.implementation` | repository implementation | always  | `presence`   | later | ESC-DOM 72                             |

### Behaviour

From the messaging pack, `intent.messaging`, whatever raises the event.

| Kind            | Label  | Stated | Comparer   | Phase | Checks        |
| --------------- | ------ | ------ | ---------- | ----- | ------------- |
| `intent.raises` | raises | always | `presence` | 1     | ESC-DOM 8, 30 |

### Services

From the ASP.NET Core, Spring MVC and NServiceBus packs, phase 4, except where marked. `intent.publishes` and `intent.sends` are the messaging pack's, `intent.messaging`, whatever publishes or sends the message, and an NServiceBus pack sharpens them (a publish never satisfies a send).

| Kind                     | Label              | Stated  | Comparer   | Phase | Checks                                    |
| ------------------------ | ------------------ | ------- | ---------- | ----- | ----------------------------------------- |
| `service.route`          | route              | whenSet | `route`    | 4     | ESC-SVC 13; JAV-SVC 6                     |
| `operation.verb`         | verb               | always  | `exact`    | 4     | ESC-SVC 21; JAV-SVC 6                     |
| `operation.route`        | route              | whenSet | `route`    | 4     | ESC-SVC 22; JAV-SVC 6                     |
| `operation.status`       | status code        | whenSet | `number`   | 4     | ESC-SVC 24; JAV-SVC 7                     |
| `endpoint.exists`        | endpoint           | always  | `presence` | 4     | ESC-SVC 19, 20; JAV-SVC 4                 |
| `security`               | security           | always  | `exact`    | 4     | ESC-SVC 14, 15, 16; ESC-FE 14; JAV-SVC 13 |
| `security.roles`         | roles              | whenSet | `set`      | 4     | conversion only                           |
| `security.policy`        | policy             | whenSet | `set`      | 4     | JAV-SVC 14                                |
| `service.versions`       | API versions       | whenSet | `set` of `version` | 4     | ESC-SVC 17                                |
| `operation.versions`     | API versions       | whenSet | `set` of `version` | 4     | ESC-SVC 17                                |
| `api.versionScheme`      | versioning scheme  | whenSet | `exact`    | later | ESC-SVC 18                                |
| `parameter.source`       | binding            | whenSet | `exact`    | 4     | ESC-SVC 27, 28; JAV-SVC 9                 |
| `parameter.name`         | bound name         | whenSet | `exact`    | 4     | ESC-SVC 29                                |
| `intent.creates`         | creates            | always  | `presence` | 4     | JAV-SVC 15                                |
| `intent.updates`         | updates            | always  | `presence` | 4     | JAV-SVC 15                                |
| `intent.deletes`         | deletes            | always  | `presence` | 4     | JAV-SVC 15                                |
| `intent.queries`         | queries            | always  | `presence` | 4     | JAV-SVC 15                                |
| `intent.invokes`         | invokes            | always  | `presence` | 6     | ESC-SVC 33, 35, 37, 41; ESC-FE 35         |
| `intent.publishes`       | publishes          | always  | `presence` | 1     | ESC-SVC 39                                |
| `intent.sends`           | sends              | always  | `presence` | 1     | conversion only                           |
| `invocation.wrapped`     | wrapped invocation | whenSet | `exact`    | later | ESC-SVC 36                                |
| `request.returns`        | request result     | always  | `type`     | later | ESC-SVC 51                                |
| `request.handler`        | handler            | always  | `presence` | later | ESC-SVC 52                                |
| `request.validated`      | validator          | always  | `presence` | later | ESC-SVC 54, 55                            |
| `command.identified`     | identified command | always  | `exact`    | later | ESC-SVC 53                                |
| `subscription.handler`   | message handler    | always  | `presence` | 4     | ESC-SVC 60, 61                            |
| `service.implementation` | implementation     | always  | `presence` | later | ESC-SVC 65                                |
| `message.copy`           | message copy       | always  | `presence` | later | ESC-SVC 64                                |

### User interface

| Kind                | Label              | Stated  | Comparer   | Phase | Checks                     |
| ------------------- | ------------------ | ------- | ---------- | ----- | -------------------------- |
| `component.kind`    | component kind     | always  | `exact`    | 6     | ESC-FE 8, 10, 11           |
| `page.route`        | page route         | whenSet | `subset` of `route` | 6     | ESC-FE 12; JAV-FE 4        |
| `page.title`        | page title         | whenSet | `contains` | 6     | ESC-FE 13                  |
| `property.query`    | query parameter    | always  | `exact`    | 6     | ESC-FE 18                  |
| `property.route`    | route parameter    | always  | `exact`    | 6     | ESC-FE 19                  |
| `property.bindable` | bindable           | always  | `exact`    | 6     | ESC-FE 20                  |
| `intent.uses`       | uses               | always  | `presence` | 6     | ESC-FE 27, 69; JAV-FE 5, 7 |
| `intent.navigates`  | navigates          | always  | `presence` | 6     | ESC-FE 32; JAV-FE 6        |
| `intent.opens`      | opens              | always  | `presence` | 6     | conversion only            |
| `intent.target`     | interaction target | always  | `type`     | 6     | ESC-FE 30, 38              |
| `proxy.request`     | proxy request      | always  | `presence` | 6     | ESC-FE 56, 58, 59          |
| `proxy.verb`        | proxy verb         | always  | `exact`    | 6     | ESC-FE 60; JAV-FE 11       |
| `proxy.route`       | proxy route        | always  | `route`    | 6     | ESC-FE 61; JAV-FE 11       |
| `proxy.version`     | proxy API version  | whenSet | `version`  | 6     | ESC-FE 62                  |
| `proxy.header`      | proxy header       | whenSet | `subset`   | 6     | ESC-FE 63                  |
| `proxy.rpc`         | RPC method         | always  | `exact`    | 6     | ESC-FE 64                  |
| `proxy.grpcService` | gRPC service       | always  | `exact`    | 6     | ESC-FE 65                  |

### Architecture

From house packs, since no stock designer has Layers.

| Kind                 | Label              | Stated | Comparer | Phase | Checks               |
| -------------------- | ------------------ | ------ | -------- | ----- | -------------------- |
| `layer.populated`    | layer contents     | always | `exact`  | 2     | ESC-ARC 3; JAV-ARC 3 |
| `layer.dependencies` | layer dependencies | always | `within` | 3     | ESC-ARC 8; JAV-ARC 2 |

### Notes on kinds

What a kind's table row can't say, settled while the cases were written.

- **`type.kind`.** An element's type is located by name among every kind the language has, so a type of the wrong kind is found and reported as the wrong kind rather than as missing. The expected value is the set of kinds the conventions allow for the element type (`class, record` for a Class, `enum` for an Enum), and `within` verifies when the code's kind is one of them.
- **`page.route`.** The model's route must be among the page's `@page` routes; further routes the code serves aren't drift (principle 1). Items compare as routes.
- **`service.versions`, `operation.versions`.** Sets of versions whose items compare as versions, so `1`, `v1` and `1.0` are one item.
- **`index.columns`.** Ordered: column order is part of an index. JAV-DOM-30 compared column sets; an index whose columns are reordered is now contradicted. An Index stereotype on an association's target end puts that end's foreign key column in the index of the class whose table holds it, named by its Foreign Key Column Name, else the end's name.
- **`parameter.source`.** Where the framework binds the parameter (user decision, 2026-11-14, replacing 2026-11-13's attribute-only rule: the Services model is a logical projection of the endpoint). A binding attribute states it; without one, ASP.NET's own rules decide it, alike for Minimal APIs, `[ApiController]` and Web API 2 controllers: a name the route template names is the route, a simple type the query string, a form file the form, a service the container's, any other type the body. An MVC controller without `[ApiController]` binds a complex type from wherever its fields are, so it can't be verified. Spring MVC reads its annotations. A Source of `Default` makes no claim.
- **`operation.route` on Web API 2.** Attribute routing covers only an action with a `[Route]`: one without isn't under its controller's `[RoutePrefix]`, and is served on the first conventional `MapHttpRoute` route with a `{controller}`, with its `{controller}` and `{action}` filled in and an optional value it doesn't take left out. That is read as an absolute route, `~/api/Endowment`. An empty Route makes no claim, so the `aspnetcore/conventional.route` rule reports an Operation whose empty Route puts it at its service's Route when Web API 2 serves it elsewhere ([rules.md](rules.md#routes-web-api-2-serves-conventionally)).
- **`key.generated` on JPA.** JPA states a user-supplied key only by saying nothing (no `@GeneratedValue`). That is silence, so a modelled "User supplied" Data source is contradicted as not stated on JPA, and the model leaves Data source at its default there (decision 4). EF Core states it with `ValueGeneratedNever()`.
- **No `column.required`.** An attribute's Is Nullable is the only requiredness the model states, and `member.nullable` checks it, reading `[Required]`, `.IsRequired()` and `@Column(nullable = false)` where the code's own nullability says nothing. A required column on a nullable property (`bool?` with `.IsRequired()`) is something the model can't express, so it isn't drift. Common.Drift recipes for `column.required` convert to `member.nullable` ([conversion.md](conversion.md#semantic-changes), change 11).
- **`column.version`.** Intent has no stock stereotype for EF Core's `IsRowVersion`, so only the Java spike's Version stereotype states it; its cases are JPA's.
- **`relationship.reference`.** The classification proposed `reference.target` for ESC-DOM-59. A References relationship has no association, so the claim is stated by the foreign-key attribute's References stereotype, whose `Target` holds the referenced class.
- **`intent.target`.** The `type` comparer with resolution: the type a Razor tag or an `@inject` resolves to must be the one the target element is located as ([code-reader.md](code-reader.md#markup-references)).
- **`entity.persisted` is what maps a Table or View class (2026-11-15).** EF Core: a DbContext's `DbSet` or `Entity<T>`, a configuration it applies one by one or with its assembly's (`ApplyConfigurationsFromAssembly`, the project of the type `typeof(T).Assembly` names), `modelBuilder.Entity<T>()` elsewhere, else by convention a mapped entity's navigation to it (defaulted). JPA: `@Entity` or `@Embeddable`. Silence is `false`. With no DbContext in scope it can't be verified, `notFound`. The class's other persistence claims wait on it (`waitsOn`), so a table nothing maps is one finding (user decision, 2026-11-15: "flag it once").
- **`relationship.multiplicity` is the end no navigation states (user decision, 2026-11-15).** A navigable association end is its property: `member.exists` finds it, `member.type` compares its target and whether it is a collection, `member.nullable` whether it is optional. So `relationship.multiplicity` claims only the other end, where only one end is navigable and the navigation's owner is a table or view (a navigation from a class the model doesn't persist is no mapped relationship, so it has no claim; a table nothing maps is `entity.persisted`'s one finding): how many of the navigation's owners each target has (`1`, `0..1`, `*`, `1..*`), read from EF Core's `WithOne`/`WithMany`/`IsRequired`/owned types and the dependent's foreign key, or JPA's relationship annotation, cascade and join column. One mismatch is one finding.
- **`column.mapped` is every attribute of a Table or View class (user, 2026-11-15).** The RDBMS stereotypes only adjust the mapping where the database differs from convention, and a model usually has none, so an attribute of a persisted class is a column whether or not it carries one. A property the code ignores (`Ignore`, `[NotMapped]`) that the model has as an attribute is drift: either the model has a property that isn't domain state, or the code dropped a column.
- **`security` is per operation (2026-11-15).** Secured or Unsecured on the operation, else on its service, against the code's authorization on the endpoint, its group or controller, `[AllowAnonymous]` anywhere winning (Spring: a method's annotation over its class's). `security.roles` and `security.policy` wait on it. They replace the engine's earlier `service.secured`, `operation.secured`, `operation.anonymous`, `service.roles` and `operation.roles`.
- **`endpoint.exists`.** Claimed on each Operation of an HTTP service: one without Http Settings states it isn't an endpoint, so a controller action or mapped handler for it is drift (a client copy's model had four same-named overloads the reverse engineering left without Http Settings). Verb, route, status, parameter and security claims wait on it.
- **`parameter.type`, `.nullable`, `.default`.** Claimed for an HTTP endpoint's parameters. `operation.parameters` then doesn't compare the type of a parameter with its own `parameter.type` claim, and a parameter the method lacks is one finding.
- **`enum.literals` replaces `member.exists` on each Enum-Literal**, so a renamed literal is one finding. It waits on `type.kind`, and `enum.literal.value` on it.
- **`request.returns` and `request.handler`.** On every Command and Query, from `intent.cqrs` (the model) and `intent.mediatr`, `intent.mediator` or `intent.wolverine` (the code; the imported namespace tells MediatR's `IRequest` from Mediator's). The handler is looked for by the result the request's code gives, and waits on `request.returns`, so a changed result is one finding.
- **Recipe-only kinds.** `type.location`, `type.marker`, `file.exists`, `type.registered`, `service.implementation`, `request.validated`, `command.identified`, `invocation.wrapped`, `member.backingField`, `column.backingField`, `column.hilo`, `column.enumStorage`, `column.version`, `relationship.onDelete` and `relationship.reference` are stated by house stereotypes, so house packs claim them; the engine defines the kinds, and the EF Core and JPA readers read the persistence ones.
- **`member.type` and nullability.** `type` compares the type and whether it is a collection. Nullability is compared only by `member.nullable`, `parameter.nullable` and `operation.nullable` (the return, through its async wrappers), so one mismatch is one finding. Every operation's and constructor's parameters are claimed, not only an HTTP endpoint's, and a parameter a class renames from its interface is read from the interface's declaration.
- **Computed values.** A value the code computes (an interpolated `<PageTitle>`, a computed route prefix) is `unreadable`, an error, where eShopCustom gave a warning. Several of its "unsupported" warnings become can't-verify the same way (principle 2).

Some checks are traced only in part. A case shows what a recipe, a query or the structure comparison checks; the rest of the check (following private calls transitively, a URI built across methods, a registration in another file) needs a plug-in, which a case can't carry. Each such case's prose names the part it leaves out, for the phase that brings the plug-in.

## Scope, engine and opt-in cases

| Case                                                                                                 | Checks                                                                                                                                              |
| ---------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| `scope/missing`                                                                                      | ESC-DOM 1; ESC-SVC 1; ESC-FE 1; ESC-ARC 5                                                                                                           |
| `scope/stale`                                                                                        | ESC-DOM 2; ESC-SVC 2, 3; ESC-FE 2, 3; ESC-ARC 6                                                                                                     |
| `scope/coverage`                                                                                     | ESC-DOM 4; ESC-SVC 5; ESC-FE 5                                                                                                                      |
| `scope/gate`                                                                                         | ESC-SVC 6                                                                                                                                           |
| `scope/shadow`                                                                                       | ESC-SVC 5; ESC-ARC 4                                                                                                                                |
| `scope/reason`, `notRead`, `builtin`, `type`, `nested`                                               | none: new with the scope model                                                                                                                      |
| `engine/noVocabulary`                                                                                | ESC-DOM 6; ESC-SVC 8; ESC-FE 7; JAV-DOM 8                                                                                                           |
| `engine/readerFailed`                                                                                | ESC-DOM 5; ESC-SVC 7; ESC-FE 6; ESC-ARC 1                                                                                                           |
| `engine/ambiguous`                                                                                   | ESC-DOM 15                                                                                                                                          |
| `engine/unreadable`                                                                                  | ESC-DOM 32; ESC-SVC 10; ESC-FE 72                                                                                                                   |
| `engine/noRule`, `ruleFailed`, `unresolved`, `notFound`, `blocked`, `silence`, `comparers`, `report` | none: the engine's own outcomes and policy                                                                                                          |
| `optin/unmodelled.type`                                                                              | ESC-DOM 21, 22; ESC-SVC 25, 56, 59, 63, 69, 74, 80, 84; ESC-FE 15, 49, 55; JAV-DOM 31; JAV-SVC 3; JAV-FE 19                                         |
| `optin/unmodelled.member`                                                                            | ESC-DOM 24, 29, 43, 62, 65, 66, 73; ESC-SVC 32, 47, 68, 73, 80, 84; ESC-FE 21, 24, 44, 47, 54; JAV-DOM 21, 28, 32; JAV-SVC 5, 12, 20; JAV-FE 16, 18 |
| `optin/unmodelled.interaction`                                                                       | ESC-DOM 31; ESC-SVC 34, 38, 40; ESC-FE 28, 29, 33, 34, 36, 37; JAV-SVC 16; JAV-FE 13, 14, 15                                                        |
| `optin/unmodelled.exposed`                                                                           | ESC-SVC 57; JAV-SVC 21                                                                                                                              |
| `optin/unmodelled.file`                                                                              | ESC-DOM 3; ESC-SVC 4; ESC-FE 4; ESC-ARC 7; JAV-FE 12; JAV-ARC 6                                                                                     |
| `optin/unmodelled.value`                                                                             | ESC-DOM 52                                                                                                                                          |

## Model rules and dropped checks

| Disposition         | Checks                                                                                                                                              |
| ------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| `model-rule`        | ESC-SVC 81, 85, 86, 87; ESC-FE 31, 57, 74; ESC-ARC 2, 9; JAV-DOM 1, 10, 13, 22, 29; JAV-SVC 1, 17; JAV-FE 1; JAV-ARC 1, 5                           |
| `dropped:inventory` | ESC-DOM 11, 53, 67, 68; ESC-SVC 9; ESC-FE 66, 67, 68, 71, 73                                                                                        |
| `dropped:codeOnly`  | ESC-DOM 23 (an adopted type of no recognised kind), ESC-DOM 50 (`HasMaxLength` on a non-string), ESC-FE 70 (a navigation to a route no page serves) |

The 19 model rules match the classification's estimate that about 19 of its 69 C checks are designer validation or model-only rules. Each belongs to the module that owns the designer, as live validation.
