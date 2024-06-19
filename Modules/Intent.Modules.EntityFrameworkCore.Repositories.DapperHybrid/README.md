# Intent.EntityFrameworkCore.Repositories.DapperHybrid

This module extends our `Intent.EntityFrameworkCore.Repository` module making it easy to add `Dapper` based repository methods to the repository.

This module does the following

- Install the Dapper Nuget package
- Adds a `GetConnection()` method to the base repository for usage with Dapper.


## Dapper Repository method example 

```csharp
		public async Task<List<Customer>> GetWithDapperQuery(CancellationToken cancellationToken)
		{
			var customers = await GetConnection().QueryAsync<Customer>("Select * from [dbo].[Customers]");
			return customers.ToList();
		}
```