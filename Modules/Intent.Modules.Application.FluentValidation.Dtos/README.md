# Intent.Application.FluentValidation.Dtos

This module contains logic and templates to create `Dto validators` using [FluentValidation](https://fluentvalidation.net/).

## DTO validation creation

Intent Architect will only generate a `validator` for a `DTO` under the following scenarios:

- The `DTO` is **inbound**: If the `DTO` is **only outbound**  (returned from a `query` or `operation`) a validator will not be created for it. Only if the `DTO` is inbound (or both inbound and outbound) will the validator be created.
- The `DTO` validator would **not be generated empty**: If the `validator` would be created empty (i.e contain no validations), then it will not be generated (even if it is for an inbound `DTO`)

A validator can contain `implicit/inferred validation` rules or `user configured` validation rules.

- `Implicit/inferred validation` rules: more information on when implicit validation rules are applied can be found [here](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.FluentValidation/README.md#implicitinferred-validation-rules)
- `User configured` rules: these are rules explicitly configured on a `DTO Field` by the user
