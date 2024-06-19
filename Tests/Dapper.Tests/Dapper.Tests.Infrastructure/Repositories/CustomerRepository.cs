using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dapper.Tests.Domain.Entities;
using Dapper.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapper.Repository", Version = "1.0")]

namespace Dapper.Tests.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
INSERT INTO [Customer]
(Name, Surname, Email, IsActive)
OUTPUT Inserted.Id
VALUES
(@Name, @Surname, @Email, @IsActive)
";

                var newId = await connection.QuerySingleAsync<Guid>(sql, entity);
                entity.Id = newId;
            }
        }

        public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = @"
UPDATE [Customer] SET
    Name = @Name,
    Surname = @Surname,
    Email = @Email,
    IsActive = @IsActive
WHERE Id = @Id
";

                await connection.ExecuteAsync(sql, entity);
            }
        }

        public async Task RemoveAsync(Customer entity, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "DELETE FROM [Customer] WHERE Id = @Id";

                await connection.ExecuteAsync(sql, new { Id = entity.Id });
            }
        }

        public async Task<Customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [Customer] WHERE Id = @Id";

                return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
            }
        }

        public async Task<List<Customer>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                var sql = "SELECT * FROM [Customer]";

                var result = await connection.QueryAsync<Customer>(sql);
                return result.ToList();
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<Customer> SearchCustomer(string searchTerm)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}