using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;
using SqlDbProject.Domain.Repositories;
using SqlDbProject.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlDbProject.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CountryRepository : RepositoryBase<Country, Country, ApplicationDbContext>, ICountryRepository
    {
        public CountryRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            string countryIso,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.CountryIso == countryIso, cancellationToken);
        }

        public async Task<Country?> FindByIdAsync(string countryIso, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.CountryIso == countryIso, cancellationToken);
        }

        public async Task<List<Country>> FindByIdsAsync(
            string[] countryIsos,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = countryIsos.ToList();
            return await FindAllAsync(x => idList.Contains(x.CountryIso), cancellationToken);
        }
    }
}