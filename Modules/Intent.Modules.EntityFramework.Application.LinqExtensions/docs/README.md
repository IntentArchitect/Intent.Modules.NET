# Intent.EntityFramework.Application.LinqExtensions

These module adds LINQ Extension methods which can be using in conjunction with the EF repositories, in the application layer, without adding the `EntityFramework` NuGet dependency in your application layer.

The following LINQ extension methods are added

- AsNoTracking
- AsTracking

Usage Example:

```csharp

var customers = await _customerRepository.FindAllAsync(o => o.AsNoTracking(), cancellationToken);

```