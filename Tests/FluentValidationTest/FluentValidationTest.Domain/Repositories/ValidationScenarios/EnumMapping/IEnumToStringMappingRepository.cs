using FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace FluentValidationTest.Domain.Repositories.ValidationScenarios.EnumMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IEnumToStringMappingRepository : IEFRepository<EnumToStringMapping, EnumToStringMapping>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<EnumToStringMapping?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<EnumToStringMapping?> FindByIdAsync(Guid id, Func<IQueryable<EnumToStringMapping>, IQueryable<EnumToStringMapping>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<EnumToStringMapping>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}