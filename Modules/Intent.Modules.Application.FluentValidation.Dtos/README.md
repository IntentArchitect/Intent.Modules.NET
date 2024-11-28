# Intent.Application.FluentValidation.Dtos

This module provides the logic and templates for creating `DTO validators` using [FluentValidation](https://fluentvalidation.net/).

## Creating DTO Validators

A `validator` is automatically generated for a `DTO` under the following conditions:

- **Inbound DTO**: A validator is created only if the `DTO` is **inbound** (or both inbound and outbound). If the `DTO` is **only outbound** (i.e., returned from a `query` or `operation`), no validator will be generated.
- **Non-empty Validator**: A validator will not be generated if it would be created empty (i.e., if it contains no validation rules), even if the `DTO` is inbound.

The generated validator can include either `implicit/inferred validation` rules or `user-configured` validation rules.

- **Implicit/Inferred Validation Rules**: For more information on when implicit validation rules are applied, refer to the [Implicit/Inferred Validation Rules documentation](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.FluentValidation/README.md#implicitinferred-validation-rules).
- **User-Configured Validation Rules**: These are rules explicitly configured by the user on a `DTO Field`.
