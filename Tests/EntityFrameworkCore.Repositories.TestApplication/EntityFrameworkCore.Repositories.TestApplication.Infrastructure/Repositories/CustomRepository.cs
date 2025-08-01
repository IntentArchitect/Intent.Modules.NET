using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Infrastructure.Repositories
{
    public class CustomRepository : ICustomRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SpResult> Sp_params0_returnsD_collection0_schemaName0(CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE sp_params0_returnsD_collection0_schemaName0")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<SpResult> Sp_params0_returnsD_collection0_schemaName1(CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<List<SpResult>> Sp_params0_returnsD_collection1_schemaName0(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE sp_params0_returnsD_collection1_schemaName0")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<SpResult>> Sp_params0_returnsD_collection1_schemaName1(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<AggregateRoot1> Sp_params0_returnsE_collection0_schemaName0(CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE sp_params0_returnsE_collection0_schemaName0")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<AggregateRoot1> Sp_params0_returnsE_collection0_schemaName1(CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<List<AggregateRoot1>> Sp_params0_returnsE_collection1_schemaName0(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE sp_params0_returnsE_collection1_schemaName0")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<AggregateRoot1>> Sp_params0_returnsE_collection1_schemaName1(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task Sp_params0_returnsV_collection0_schemaName0(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_params0_returnsV_collection0_schemaName0", cancellationToken);
        }

        public async Task Sp_params0_returnsV_collection0_schemaName1(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE dbo.sp_custom", cancellationToken);
        }

        public async Task<SpResult> Sp_params1_returnsD_collection0_schemaName0(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE sp_params1_returnsD_collection0_schemaName0 {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<SpResult> Sp_params1_returnsD_collection0_schemaName1(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<List<SpResult>> Sp_params1_returnsD_collection1_schemaName0(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE sp_params1_returnsD_collection1_schemaName0 {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<SpResult>> Sp_params1_returnsD_collection1_schemaName1(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<AggregateRoot1> Sp_params1_returnsE_collection0_schemaName0(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE sp_params1_returnsE_collection0_schemaName0 {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<AggregateRoot1> Sp_params1_returnsE_collection0_schemaName1(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<List<AggregateRoot1>> Sp_params1_returnsE_collection1_schemaName0(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE sp_params1_returnsE_collection1_schemaName0 {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<AggregateRoot1>> Sp_params1_returnsE_collection1_schemaName1(
            string param1,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task Sp_params1_returnsV_collection0_schemaName0(
            string param1,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_params1_returnsV_collection0_schemaName0 {param1}", cancellationToken);
        }

        public async Task Sp_params1_returnsV_collection0_schemaName1(
            string param1,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE dbo.sp_custom {param1}", cancellationToken);
        }

        public async Task<SpResult> Sp_params2_returnsD_collection0_schemaName0(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE sp_params2_returnsD_collection0_schemaName0 {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<SpResult> Sp_params2_returnsD_collection0_schemaName1(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<List<SpResult>> Sp_params2_returnsD_collection1_schemaName0(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE sp_params2_returnsD_collection1_schemaName0 {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<SpResult>> Sp_params2_returnsD_collection1_schemaName1(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.SpResults
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<AggregateRoot1> Sp_params2_returnsE_collection0_schemaName0(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE sp_params2_returnsE_collection0_schemaName0 {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<AggregateRoot1> Sp_params2_returnsE_collection0_schemaName1(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var result = (await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken))
                .Single();

            return result;
        }

        public async Task<List<AggregateRoot1>> Sp_params2_returnsE_collection1_schemaName0(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE sp_params2_returnsE_collection1_schemaName0 {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<AggregateRoot1>> Sp_params2_returnsE_collection1_schemaName1(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.AggregateRoot1s
                .FromSqlInterpolated($"EXECUTE dbo.sp_custom {param1}, {param2}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task Sp_params2_returnsV_collection0_schemaName0(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE sp_params2_returnsV_collection0_schemaName0 {param1}, {param2}", cancellationToken);
        }

        public async Task Sp_params2_returnsV_collection0_schemaName1(
            string param1,
            string param2,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE dbo.sp_custom {param1}, {param2}", cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation_Params0_ReturnsV_Collection0()
        {
            // TODO: Implement Operation_Params0_ReturnsV_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public SpResult Operation_Params0_ReturnsD_Collection0()
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<SpResult> Operation_Params0_ReturnsD_Collection1()
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection1 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public AggregateRoot1 Operation_Params0_ReturnsE_Collection0()
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<AggregateRoot1> Operation_Params0_ReturnsE_Collection1()
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection1 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Operation_Params0_ReturnsV_Collection0Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsV_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SpResult> Operation_Params0_ReturnsD_Collection0Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<SpResult>> Operation_Params0_ReturnsD_Collection1Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection1Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<AggregateRoot1> Operation_Params0_ReturnsE_Collection0Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<AggregateRoot1>> Operation_Params0_ReturnsE_Collection1Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection1Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation_Params1_ReturnsV_Collection0(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsV_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public SpResult Operation_Params1_ReturnsD_Collection0(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<SpResult> Operation_Params1_ReturnsD_Collection1(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection1 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public AggregateRoot1 Operation_Params1_ReturnsE_Collection0(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<AggregateRoot1> Operation_Params1_ReturnsE_Collection1(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection1 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Operation_Params1_ReturnsV_Collection0Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsV_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SpResult> Operation_Params1_ReturnsD_Collection0Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<SpResult>> Operation_Params1_ReturnsD_Collection1Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection1Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<AggregateRoot1> Operation_Params1_ReturnsE_Collection0Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<AggregateRoot1>> Operation_Params1_ReturnsE_Collection1Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection1Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation_Params3_ReturnsV_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsV_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public SpResult Operation_Params3_ReturnsD_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<SpResult> Operation_Params3_ReturnsD_Collection1(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection1 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public AggregateRoot1 Operation_Params3_ReturnsE_Collection0(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection0 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<AggregateRoot1> Operation_Params3_ReturnsE_Collection1(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection1 (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Operation_Params3_ReturnsV_Collection0Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsV_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<SpResult> Operation_Params3_ReturnsD_Collection0Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<SpResult>> Operation_Params3_ReturnsD_Collection1Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection1Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<AggregateRoot1> Operation_Params3_ReturnsE_Collection0Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection0Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<List<AggregateRoot1>> Operation_Params3_ReturnsE_Collection1Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection1Async (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}