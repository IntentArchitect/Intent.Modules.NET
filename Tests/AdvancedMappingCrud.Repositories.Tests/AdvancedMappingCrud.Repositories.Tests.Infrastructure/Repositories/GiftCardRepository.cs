using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GiftCardRepository : RepositoryBase<GiftCard, GiftCard, ApplicationDbContext>, IGiftCardRepository
    {
        public GiftCardRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            string cardCode,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.CardCode == cardCode, cancellationToken);
        }

        public async Task<GiftCard?> FindByIdAsync(string cardCode, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CardCode == cardCode, cancellationToken);
        }

        public async Task<GiftCard?> FindByIdAsync(
            string cardCode,
            Func<IQueryable<GiftCard>, IQueryable<GiftCard>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CardCode == cardCode, queryOptions, cancellationToken);
        }

        public async Task<List<GiftCard>> FindByIdsAsync(string[] cardCodes, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = cardCodes.ToList();
            return await FindAllAsync(x => idList.Contains(x.CardCode), cancellationToken);
        }
    }
}