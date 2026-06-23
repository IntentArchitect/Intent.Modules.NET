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

## Related Modules

- [Intent.Application.Wolverine](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine/README.md) — core Wolverine CQRS module; required by this module.
- [Intent.Application.Wolverine.DomainEvents](https://github.com/IntentArchitect/Intent.Modules.NET/blob/master/Modules/Intent.Modules.Application.Wolverine.DomainEvents/README.md) — dispatches domain events through Wolverine's `IMessageBus`.
