# Intent.Application.Wolverine.FluentValidation

Extends the Wolverine CQRS module with FluentValidation support, generating one `AbstractValidator` per Command and Query element in the Application designer.

## What This Module Generates

- `CommandValidator` — `AbstractValidator<TCommand>` for each Command element; follows the `DtoValidatorTemplateBase` infrastructure.
- `QueryValidator` — `AbstractValidator<TQuery>` for each Query element; follows the `DtoValidatorTemplateBase` infrastructure.

## Generated Validators

Each Command and Query element produces a dedicated validator class. The `modelParameterName` is set to `"command"` or `"query"` respectively, so validation rule references read naturally.

```csharp
// Generated command validator
[IntentManaged(Mode.Merge)]
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    [IntentManaged(Mode.Merge)]
    public CreateOrderCommandValidator(IOrderRepository orderRepository)
    {
        ConfigureValidationRules();
    }

    [IntentManaged(Mode.Fully)]
    private void ConfigureValidationRules()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty();

        RuleFor(command => command.Items)
            .NotEmpty();
    }
}
```

```csharp
// Generated query validator
[IntentManaged(Mode.Merge)]
public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    [IntentManaged(Mode.Merge)]
    public GetOrderByIdQueryValidator()
    {
        ConfigureValidationRules();
    }

    [IntentManaged(Mode.Fully)]
    private void ConfigureValidationRules()
    {
        RuleFor(query => query.Id)
            .NotEmpty();
    }
}
```

## Repository Injection and Unique Constraint Validation

When the application settings enable unique constraint validation, the validator constructor receives the relevant repository and generates async uniqueness rules:

```csharp
public CreateProductCommandValidator(IProductRepository productRepository)
{
    ConfigureValidationRules();
    _productRepository = productRepository;
}

private void ConfigureValidationRules()
{
    RuleFor(command => command.Sku)
        .NotEmpty()
        .MustAsync(async (sku, cancellationToken) =>
            !await _productRepository.AnyAsync(p => p.Sku == sku, cancellationToken))
        .WithMessage("A product with this SKU already exists.");
}
```

## AI Agent Skill File

This module depends on `Intent.Application.FluentValidation`, which generates an AI agent skill file at `.agents/skills/fluent-validation-custom-validation/SKILL.md`. It describes how to implement or revise a validator's custom async validation method — updating the method body, adding private helper methods, and extending Application/Domain abstractions such as repositories, while keeping infrastructure dependencies out of the validator. The same skill file is shared with `Intent.Application.MediatR.FluentValidation` apps; it is not duplicated per transport.

## Related Modules

- [Intent.Application.Wolverine](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine/intent-application-wolverine.html) — core Wolverine CQRS module; required by this module.
- [Intent.Application.Wolverine.DomainEvents](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-wolverine-domainevents/intent-application-wolverine-domainevents.html) — dispatches domain events through Wolverine's `IMessageBus`.
- [Intent.Application.FluentValidation](https://docs.intentarchitect.com/articles/modules-dotnet/intent-application-fluentvalidation/intent-application-fluentvalidation.html) — provides the shared `CustomValidationSkill` AI agent skill file inherited by this module.
