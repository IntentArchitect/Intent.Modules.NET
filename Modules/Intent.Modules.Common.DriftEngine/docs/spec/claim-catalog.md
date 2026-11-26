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

203 checks reduce to 102 claim kinds. Six kinds come from Common.Drift's recipes alone, with no eShopCustom or Java check behind them ("conversion only" below); they keep conversion whole. Twenty-four more come from neither: standard Intent stereotypes and links no old check read (OpenAPI Settings, the Domain Constraints, and the User Interface designer's Region, Entry, Publishes, Subscribes and Persisted), which a house module checked first and the engine has made stock ("stock vocabulary" below). The suite counts the stock vocabulary that claims a kind as its use.

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

### Domain constraints

From the Domain Constraints pack, `intent.constraints` (the model), read by `intent.dataannotations` (.NET's DataAnnotations) and `intent.beanvalidation` (Java's Bean Validation), on a Domain attribute's member.

| Kind                        | Label                      | Stated  | Comparer | Phase | Checks                                  |
| --------------------------- | -------------------------- | ------- | -------- | ----- | --------------------------------------- |
| `constraint.required`       | Required constraint        | whenSet | `exact`  | 4     | stock vocabulary (Required)             |
| `constraint.minLength`      | minimum length constraint  | whenSet | `number` | 4     | stock vocabulary (Text Limits)          |
| `constraint.maxLength`      | maximum length constraint  | whenSet | `number` | 4     | stock vocabulary (Text Limits)          |
| `constraint.minItems`       | minimum items constraint   | whenSet | `number` | 4     | stock vocabulary (Collection Limits)    |
| `constraint.maxItems`       | maximum items constraint   | whenSet | `number` | 4     | stock vocabulary (Collection Limits)    |
| `constraint.pattern`        | Regular Expression pattern | whenSet | `exact`  | 4     | stock vocabulary (Regular Expression)   |
| `constraint.patternMessage` | Regular Expression message | whenSet | `exact`  | 4     | stock vocabulary (Regular Expression)   |
| `constraint.email`          | Email constraint           | whenSet | `exact`  | 4     | stock vocabulary (Email)                |
| `constraint.url`            | Url constraint             | whenSet | `exact`  | 4     | stock vocabulary (Url)                  |
| `constraint.base64`         | Base64 constraint          | whenSet | `exact`  | 4     | stock vocabulary (Base64)               |
| `constraint.minValue`       | minimum value constraint   | whenSet | `number` | 4     | stock vocabulary (Numeric Limits)       |
| `constraint.maxValue`       | maximum value constraint   | whenSet | `number` | 4     | stock vocabulary (Numeric Limits)       |
| `constraint.minExclusive`   | minimum value exclusive    | whenSet | `exact`  | 4     | stock vocabulary (Numeric Limits)       |
| `constraint.maxExclusive`   | maximum value exclusive    | whenSet | `exact`  | 4     | stock vocabulary (Numeric Limits)       |

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

From the ASP.NET Core, Spring MVC and NServiceBus packs, phase 4, except where marked. `intent.publishes` and `intent.sends` are the messaging pack's, `intent.messaging`, whatever publishes or sends the message, and an NServiceBus pack sharpens them (a publish never satisfies a send). `message.copy` is the messaging pack's too, and `grpc.service.rpcs` the gRPC pack's, `intent.grpc`.

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
| `grpc.service.rpcs`      | proto service rpcs | always  | `exact`    | 4     | ESC-SVC 78, 79                            |
| `operation.openApi.operationId` | OpenAPI operationId | whenSet | `exact` | 4  | stock vocabulary (OpenAPI Settings)       |
| `operation.openApi.ignore` | hidden from OpenAPI | whenSet | `exact`  | 4     | stock vocabulary (OpenAPI Settings)       |

### User interface

| Kind                | Label              | Stated  | Comparer   | Phase | Checks                     |
| ------------------- | ------------------ | ------- | ---------- | ----- | -------------------------- |
| `component.kind`    | component kind     | always  | `exact`    | 6     | ESC-FE 8, 10, 11           |
| `page.route`        | page route         | whenSet | `subset` of `route` | 6     | ESC-FE 12; JAV-FE 4        |
| `page.title`        | page title         | whenSet | `contains` | 6     | ESC-FE 13                  |
| `property.query`    | query parameter    | always  | `exact`    | 6     | ESC-FE 18                  |
| `property.route`    | route parameter    | always  | `exact`    | 6     | ESC-FE 19                  |
| `property.bindable` | bindable           | always  | `exact`    | 6     | ESC-FE 20                  |
| `region.outlet`     | region outlet      | always  | `exact`    | 6     | stock vocabulary (Region)  |
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
| `entry.label`       | entry label        | whenSet | `exact`    | 6     | stock vocabulary (Entry)   |
| `entry.icon`        | entry icon         | whenSet | `exact`    | 6     | stock vocabulary (Entry)   |
| `entry.key`         | entry key          | whenSet | `exact`    | 6     | stock vocabulary (Entry)   |
| `entry.shortcut`    | entry shortcut     | whenSet | `exact`    | 6     | stock vocabulary (Entry)   |
| `appEvent.publishes` | publishes         | always  | `presence` | 6     | stock vocabulary (Publishes) |
| `appEvent.subscribes` | subscribes       | always  | `presence` | 6     | stock vocabulary (Subscribes) |
| `store.persisted`   | persisted          | whenSet | `exact`    | 6     | stock vocabulary (Persisted) |

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
- **`operation.route` on Web API 2.** Attribute routing covers only an action with a `[Route]`: one without isn't under its controller's `[RoutePrefix]`, and is served on the first conventional `MapHttpRoute` route with a `{controller}`, with its `{controller}` and `{action}` filled in and an optional value it doesn't take left out. That is read as an absolute route, `~/api/Policy`. An empty Route makes no claim, so the `aspnetcore/conventional.route` rule reports an Operation whose empty Route puts it at its service's Route when Web API 2 serves it elsewhere ([rules.md](rules.md#routes-web-api-2-serves-conventionally)). ASP.NET Core MVC is read alike for an action whose controller has no `[Route]` either, from `MapControllerRoute` and `MapDefaultControllerRoute()`.
- **`api.versionScheme` from stock Api Version Settings (user decision, 2026-11-23).** The stereotype has no scheme of its own, and the designer puts `{version}` in the Service's Route, so where the Route has it says the scheme: in its path, `url segment`; in its query string under a name, `query: <name>`; under Asp.Versioning's default, `api-version`, or nowhere, no claim (decision 4).
- **`operation.verb` on an ASP.NET Core action with no verb attribute.** It serves every verb, so the Verb the designer requires is verified, as a convention (a `default` reading, so `unmodelled.value` doesn't report it). Web API 2 still takes the verb from the action's name.
- **`operation.returns` on an MVC view.** The model the view renders is what the action returns, as Intent's MVC module models it: `View(model)` and `View(name, model)` (and `PartialView`) return `model`'s type, `View()` and `View(name)` nothing. The code's own controller extension methods are followed (`this.LoadingPage(...)` whose body is `controller.View(name, new RedirectViewModel { ... })`). An action whose every return redirects, challenges or signs out returns nothing. An action whose successes are all views may render several (its own page, a redirect page, an error page), and the model states one, so it verifies when it gives one of the view models rendered; an API's bodies must all be the model's type. A model that states the action's own result type (`IActionResult`) verifies whatever the action renders or returns, as a less precise statement that is still true.
- **`operation.status` for a handler returning its data.** A handler whose return type is its data rather than a result (`Task<string>`, `OrderDto`, `ActionResult<OrderDto>` returning the DTO), and that builds no success result, is sent with the framework's 200: a default reading, which a modelled 200 verifies and `unmodelled.value` doesn't report. Intent's controllers send 201 for a POST with no Success Response Code, so the model must state the 200 such a POST sends.
- **`key.generated` on JPA.** JPA states a user-supplied key only by saying nothing (no `@GeneratedValue`). That is silence, so a modelled "User supplied" Data source is contradicted as not stated on JPA, and the model leaves Data source at its default there (decision 4). EF Core states it with `ValueGeneratedNever()`.
- **No `column.required`.** An attribute's Is Nullable is the only requiredness the model states, and `member.nullable` checks it, reading `[Required]`, `.IsRequired()` and `@Column(nullable = false)` where the code's own nullability says nothing. A required column on a nullable property (`bool?` with `.IsRequired()`) is something the model can't express, so it isn't drift. Common.Drift recipes for `column.required` convert to `member.nullable` ([conversion.md](conversion.md#semantic-changes), change 11).
- **`column.version`.** Intent has no stock stereotype for EF Core's `IsRowVersion`, so only the Java spike's Version stereotype states it; its cases are JPA's.
- **`relationship.reference`.** The classification proposed `reference.target` for ESC-DOM-59. A References relationship has no association, so the claim is stated by the foreign-key attribute's References stereotype, whose `Target` holds the referenced class.
- **`intent.target`.** The `type` comparer with resolution: the type a Razor tag or an `@inject` resolves to must be the one the target element is located as ([code-reader.md](code-reader.md#markup-references)).
- **`entity.persisted` is what maps a Table or View class (2026-11-15).** EF Core: a DbContext's `DbSet` or `Entity<T>`, the user (and role) of an ASP.NET Core Identity base it derives from (`IdentityDbContext<ApplicationUser>`, whose own `DbSet`s are a package's), a configuration it applies one by one or with its assembly's (`ApplyConfigurationsFromAssembly`, the project of the type `typeof(T).Assembly` names), `modelBuilder.Entity<T>()` elsewhere, else by convention a mapped entity's navigation to it (defaulted). JPA: `@Entity` or `@Embeddable`. Silence is `false`. With no DbContext in scope it can't be verified, `notFound`. The class's other persistence claims wait on it (`waitsOn`), so a table nothing maps is one finding (user decision, 2026-11-15: "flag it once").
- **`relationship.multiplicity` is the end no navigation states (user decision, 2026-11-15).** A navigable association end is its property: `member.exists` finds it, `member.type` compares its target and whether it is a collection, `member.nullable` whether it is optional. So `relationship.multiplicity` claims only the other end, where only one end is navigable and the navigation's owner is a table or view (a navigation from a class the model doesn't persist is no mapped relationship, so it has no claim; a table nothing maps is `entity.persisted`'s one finding): how many of the navigation's owners each target has (`1`, `0..1`, `*`, `1..*`), read from EF Core's `WithOne`/`WithMany`/`IsRequired`/owned types and the dependent's foreign key, or JPA's relationship annotation, cascade and join column. One mismatch is one finding.
- **`column.mapped` is every attribute of a Table or View class (user, 2026-11-15).** The RDBMS stereotypes only adjust the mapping where the database differs from convention, and a model usually has none, so an attribute of a persisted class is a column whether or not it carries one. A property the code ignores (`Ignore`, `[NotMapped]`) that the model has as an attribute is drift: either the model has a property that isn't domain state, or the code dropped a column.
- **`security` is per operation (2026-11-15).** Secured or Unsecured on the operation, else on its service, against the code's authorization on the endpoint, its group or controller, `[AllowAnonymous]` anywhere winning (Spring: a method's annotation over its class's). `security.roles` and `security.policy` wait on it. They replace the engine's earlier `service.secured`, `operation.secured`, `operation.anonymous`, `service.roles` and `operation.roles`.
- **`endpoint.exists`.** Claimed on each Operation of an HTTP service: one without Http Settings states it isn't an endpoint, so a controller action or mapped handler for it is drift (a client copy's model had four same-named overloads the reverse engineering left without Http Settings). Verb, route, status, parameter and security claims wait on it.
- **`parameter.type`, `.nullable`, `.default`.** Claimed for an HTTP endpoint's parameters. `operation.parameters` then doesn't compare the type of a parameter with its own `parameter.type` claim, and a parameter the method lacks is one finding.
- **`enum.literals` replaces `member.exists` on each Enum-Literal**, so a renamed literal is one finding. It waits on `type.kind`, and `enum.literal.value` on it.
- **`message.copy` (2026-11-14, extended 2026-11-26).** On a subscription to another application's message (one this application's designer doesn't hold), against the subscriber's copy: the type of the message's name its handler takes, whose own properties must be among the message's modelled ones, of the same types, an enum read as its name or number. The types the message carries through its fields (an Eventing DTO, and those it carries in turn) are claims of their own on the first subscription whose message carries each, keyed by the type's name: the copy is the type of its name that the copy of its carrier names in a property, and its properties, its bases' included, must be among the type's fields. A copy may leave fields out. The message is the subscription's target, so its fields are followed whether or not the host hands over the referenced package. With no copy to read (no handler, or a carrier's copy that names none) the claim is `blocked`, with no finding.
- **`grpc.service.rpcs` (2026-11-26).** On every Service, as only the code says which implement a `.proto` service: its Operations as sorted `Name(Request): Response` signatures. A class that derives, itself or through a class of the code's own, from the base protoc generates for `service X` (`X.XBase`) implements it, and its rpcs must be the Operations, by name, each taking the rpc's request message and returning its response (a streamed side is its message, one or a collection). Of several in-scope services of the name, the one generated into the namespace the code names the base in (the file's `csharp_namespace`, else its package in Pascal case) is it. A class with no such base, or one whose `.proto` isn't in scope, states nothing (`blocked`, with no finding). The reading is the proto service's declaration, so the service, its rpcs and its file are located.
- **`operation.openApi.operationId` and `operation.openApi.ignore` (2026-11-26).** On an HTTP Operation whose OpenAPI Settings (Intent.Metadata.WebApi) set them: the OperationId with `{MethodName}` and `{ServiceName}` replaced by the Operation's and its Service's names, first letter upper, as Intent's ASP.NET Core modules write it, claimed only where neither the Operation's nor its Service's Ignore hides the endpoint (the designer hides the field then); Ignore from the Operation's settings, else its Service's. Both `whenSet`: an Operation with no OpenAPI Settings, on it or its Service, claims nothing, and `unmodelled.value` reports a name or an exclusion the code gives it. ASP.NET Core states them as its OpenAPI document reads them ([rules.md](rules.md#openapi-operationids-and-exclusions)); an endpoint with no name states no operationId, and one nothing excludes is described (`false`). A lambda endpoint is its Operation by its name, the Operation's or its OperationId.
- **The `constraint.*` kinds (2026-11-26).** On a Domain attribute whose Domain Constraints stereotype sets a value, each `whenSet`, so an empty Text Limits (applied by default) states nothing. Text Limits on a string are `constraint.minLength` and `.maxLength`; Collection Limits on a collection `constraint.minItems` and `.maxItems`, kinds of their own since a string collection can have both and they mean different things. Where Text Constraints MaxLength is set, Intent validates that in place of the Text Limits Max Length (its FluentValidation rules: "physical overrides logical"), so that is the maximum claimed. A Regular Expression's message waits on its pattern, and a Numeric Limits boundary on its value, and is claimed only for a `decimal`, `float` or `double`, the types the designer offers it for. `unmodelled.value` probes only where a kind applies: Required on a nullable attribute (the stereotype's note: a non-nullable field is implicitly required), a maximum length where Text Constraints doesn't state one. EF Core's reading of `[Required]` and `[MaxLength]` as a column's nullability and length (`member.nullable`, `column.maxLength`) stays a claim of its own ([rules.md](rules.md#domain-constraints-from-validation-attributes)).
- **`region.outlet` (2026-11-26).** On each Region of a Layout, Page, Dialog or Component (Intent.Modelers.UI 2.0.0), valued with its name and waiting on its parent's `type.exists`: the named placement point the parent's code must have. The kind names no framework; Blazor reads it as a `<SectionOutlet SectionName>` of the name in the parent's markup, a layout's `@Body` being its one Region no outlet names (or, of several, the one placing a Page), and an MVC view as a section it renders (`RenderSection("scripts")`), its `RenderBody()` read as `@Body` is. Its code-to-model side is the opt-in rule `unmodelled.region` ([rules.md](rules.md#opt-in-must-be-modelled-rules)). A View Model has `type.exists`, `file.exists` and its Properties' `member.*` claims as a Store does, and a Uses from a Layout, Page, Dialog or Component to a Behaviour binds `Behaviour`, which the Blazor reader reads as the behaviour's script acting on the component's markup.
- **The User Interface kinds from MVC views (2026-11-26).** No kind is added: `intent.mvc` reads the stock ones from ASP.NET Core MVC views and Razor Pages. `component.kind` is `Layout` for a view a view's or `_ViewStart.cshtml`'s `Layout` names, `Page` for one an action renders with `View(...)` or a Razor Page, else `Component`, and `Dialog` has no rule. `page.route` (a `subset`) is the URLs of the actions that take a GET and render the view, each a conventional route's forms with its defaults left out in turn, so a model stating fewer verifies; a view only a POST renders states none. `page.title` is `ViewData["Title"]` or `ViewBag.Title`. `security` is `None` with no `security.roles` or `.policy`: the rendering action's authorization is the Services model's, on its Operation; a Razor Page, its own endpoint, reads its `[Authorize]`. `intent.uses` and `intent.target` on a Composition are a partial or view component the view renders, or, from a Region, the page shown in that layout or filling that section; `intent.navigates` a link reaching a GET action that renders the target, or the target Razor Page; `intent.invokes` and `intent.target` on a Call Service Operation Action a form reaching the operation's controller action with its Http Settings Verb.
- **The User Interface kinds from .NET MAUI (2026-11-26).** `intent.maui` reads the stock kinds from a MAUI app's views, a view being its XAML and code-behind ([code-reader.md](code-reader.md#the-xaml-reader)). `component.kind` is the first framework base on the type's base chain: `Layout` for `Shell`, `NavigationPage`, `TabbedPage` and `FlyoutPage`, `Page` for `ContentPage`, `Dialog` for CommunityToolkit.Maui's `Popup`, `Component` for any other view; a `Dialog` modelled for a page can't be verified (`noRule`), as a page's code doesn't say it is pushed modally. `page.route` (a `subset`) is the page's Shell contents' paths (`/Main/Catalog`) and the routes `Routing.RegisterRoute` registers for it. `page.title` is the XAML root's `Title`, else a `Title` its constructor sets. `security` is `None`. `property.query` is a `[QueryProperty]` on the page or its binding context naming the property (a page taking its query through `IQueryAttributable` can't be verified), `property.bindable` a `BindableProperty` backing it, and `property.route` always `false`, as Shell has no route parameters. `region.outlet` is a Shell item named by its Route, or by its only content's when it has none; a Region naming no item, when exactly one item is named by no Region, is that item renamed, so its Compositions are still read through it. On a Region's Composition, `intent.uses` and `intent.target` are a `ShellContent` of the item showing the page; on a view's, an element of the component in its XAML, a data template making it, or an `{x:Type}` naming it. A Uses is the binding context, an object the XAML creates, or a dependency as any class's is read. `intent.navigates` is a navigation from the view's code or its binding context's: `GoToAsync` on the Shell, a call of the code's own method that hands the route it is given to one (found by data flow, whatever it is named), or `Navigation.PushAsync(new X())`; from a Shell, also a content showing the page.
- **`entry.*` (2026-11-26).** On a Composition whose target end has the Entry stereotype (`target.hasStereotype('Entry')`), each `whenSet` and waiting on the Composition's `intent.uses`, so a page the Region no longer shows is one finding. MAUI reads a Region's Composition's entry from the `ShellContent` showing the page: Label its `Title`, Icon its `Icon` (a `StaticResource` or `DynamicResource` by its key), Key its `Route`, each its own or that of the tab or item holding it alone; Shell has no keyboard shortcut, so a modelled Shortcut is contradicted. `unmodelled.value` reports an entry the code states that the model leaves unset, except a label that is the page's own name, which the designer needs no Entry for. Entries of other frameworks have no reader yet, so they get `noRule`.
- **`appEvent.publishes`, `appEvent.subscribes` (2026-11-26).** Existence kinds on the Publishes and Subscribes associations, binding `Component` and `Event`; an App Event itself is `type.exists`, by the `App Event` convention (a class, record, struct, interface or type alias, with its properties and fields as members), so a message deriving from the MVVM Toolkit's `ValueChangedMessage<T>` has the `Value` the toolkit's declared library type gives it. `intent.mvvmtoolkit` reads them from CommunityToolkit.Mvvm's messenger in C# code of any XAML framework or Blazor: a publish is a `Send` on a messenger of the event (created in place, handed on, or `Send<T>()`), a subscription a `Register<TRecipient, TMessage>` or the class being an `IRecipient<T>`, in the element's own code and the code's own methods it reaches. One whose event the code doesn't have is `blocked`, as the event's `type.exists` is its finding. Other frameworks' event buses have no reader yet.
- **`store.persisted` (2026-11-26).** On a Store with the Persisted stereotype, valued `true` and waiting on its `type.exists`. MAUI reads it as the store's class reading or writing the device's `Preferences` or `SecureStorage` (their `Default` instances, or an `IPreferences` or `ISecureStorage` it is given), its property accessors included; a store of another stack has no reader yet (`noRule`), as each saves its state its own way.
- **`request.returns` and `request.handler`.** On every Command and Query, from `intent.cqrs` (the model) and `intent.mediatr`, `intent.mediator` or `intent.wolverine` (the code; the imported namespace tells MediatR's `IRequest` from Mediator's). The handler is looked for by the result the request's code gives, and waits on `request.returns`, so a changed result is one finding.
- **House-stated kinds.** `type.location`, `type.marker`, `file.exists`, `type.registered`, `service.implementation`, `request.validated`, `command.identified`, `invocation.wrapped`, `member.backingField`, `column.backingField`, `column.hilo`, `column.enumStorage`, `column.version`, `relationship.onDelete` and `relationship.reference` are stated by house stereotypes, so house packs claim them; the engine defines the kinds, the structure rules read `type.location` and `type.marker`, the EF Core and JPA readers read the persistence ones, and a house module's claim reader reads the rest. `file.exists` is also the stock User Interface pack's, for an Implementation stereotype's Owned Files, and the engine reads it itself from a `File` binding when no house reader does.
- **`member.type` of a type parameter (2026-11-26).** A member typed by a type parameter (`T Value` in `ValidatableObject<T>`) verifies a model stating it as written (`T`), as the type argument the class being checked gives it through its bases (`UserName : ValidatableObject<string>` makes it `string`), or as the language profile's `object` when nothing binds it: each is true of the code, so a designer without generics, which can only say `object`, is clean, and one with them still states `T`.
- **`member.type` and nullability.** `type` compares the type and whether it is a collection. Nullability is compared only by `member.nullable`, `parameter.nullable` and `operation.nullable` (the return, through its async wrappers), so one mismatch is one finding. Every operation's and constructor's parameters are claimed, not only an HTTP endpoint's, and a parameter a class renames from its interface is read from the interface's declaration.
- **Computed values.** A value the code computes (an interpolated `<PageTitle>`, a computed route prefix) is `unreadable`, an error, where eShopCustom gave a warning. Several of its "unsupported" warnings become can't-verify the same way (principle 2).

Some checks are traced only in part. A case shows what a stock reader, a house module's reader or the structure comparison checks; the rest of the check (following private calls transitively, a URI built across methods, a registration in another file) needs more of a reader than the case shows. Each such case's prose names the part it leaves out, for the phase that brings the plug-in.

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
