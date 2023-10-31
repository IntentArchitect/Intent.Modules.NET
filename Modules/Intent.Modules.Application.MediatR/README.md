# Intent.Application.MediatR

This Intent Architect module realizes a CQRS design pattern using MediatR, for Service's modeled in the Services Designer using the CQRS paradigm.

CQRS (Command Query Responsibility Segregation) using MediatR is a software design pattern and library combination that separates read and write operations in an application. MediatR serves as the Mediator, handling commands and queries, and routing them to their respective command and query handlers. Commands represent intentions to change the application's state, while queries retrieve data without making modifications. This approach promotes separation of concerns, scalability, and testability, allowing for clearer codebases and aligning well with Domain-Driven Design principles, ultimately resulting in more maintainable and efficient applications.

This module produces the following artifacts:

- **Command Interface** - `ICommand` interface for identifying commands.
- **Query Interface**- `IQuery` interface for identifying querys.
- **Commands** - Classes representing all the modelled `Command`s.
- **CommandHandlers** - MediatR request handlers for all the modelled `Command`s
- **Querys**- Classes representing all the modelled `Query`s.
- **QueryHandlers** - MediatR request handlers for all the modelled `Query`s

## CQRS Settings

### CQRS Settings - Consolidate Command/Query associated files into single file

When set to `disabled` (the default), each command/query is generated into its own sub-folder with its handler and validator (when applicable) alongside it in the same sub-folder.

![File view when consolidation disabled](docs/cqrs-consolidate-disabled.png)

When set to `enabled`, commands and queries no longer have their own sub-folders and files which used to be generated alongside them are now instead generated into the handler/command file itself.

![File view when consolidation enabled](docs/cqrs-consolidate-enabled.png)

## Examples

This module consumes:

![CQRTS Modelling](docs/images/cqrs-modeling.png)

And produce artifacts similar to:

### Command Example

```csharp
public class CreateCustomerCommand : IRequest<Guid>, ICommand
{
    public CreateCustomerCommand(string name, string surname, string email)
    {
        Name = name;
        Surname = surname;
        Email = email;
    }

    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
}
```

### CommandHandler Example

```csharp
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        ...
    }
}

```

For more information on MediatR, check out their [official GitHub](https://github.com/jbogard/MediatR/).
