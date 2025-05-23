using Intent.RoslynWeaver.Attributes;
using SqlDbProject.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace SqlDbProject.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICountryRepository : IEFRepository<Country, Country>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(string countryIso, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Country?> FindByIdAsync(string countryIso, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Country?> FindByIdAsync(string countryIso, Func<IQueryable<Country>, IQueryable<Country>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Country>> FindByIdsAsync(string[] countryIsos, CancellationToken cancellationToken = default);
    }
}