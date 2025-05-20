# Intent.EntityFramework.Application.LinqExtensions

These module add LINQ Extension methods which can be using in conjunction with the EF repositories, in the application layer, without adding the `EntityFramework` NuGet dependency in your application layer.

The following LINQ extension methods

- AsNoTracking
- AsTracking

These can then be used as follows:

```csharp

var customers = await _customerRepository.FindAllAsync(o => o.AsNoTracking(), cancellationToken);

```