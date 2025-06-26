using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Countries;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients
{
    public interface ICountriesService : IDisposable
    {
        Task<Guid> CreateCountryAsync(CreateCountryDto dto, CancellationToken cancellationToken = default);
        Task UpdateCountryAsync(Guid id, UpdateCountryDto dto, CancellationToken cancellationToken = default);
        Task<CountryDto> FindCountryByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<CountryDto>> FindCountriesAsync(CancellationToken cancellationToken = default);
        Task DeleteCountryAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateStateAsync(Guid countryId, CreateStateDto dto, CancellationToken cancellationToken = default);
        Task UpdateStateAsync(Guid countryId, Guid id, UpdateStateDto dto, CancellationToken cancellationToken = default);
        Task<StateDto> FindStateByIdAsync(Guid countryId, Guid id, CancellationToken cancellationToken = default);
        Task<List<StateDto>> FindStatesAsync(Guid countryId, CancellationToken cancellationToken = default);
        Task DeleteStateAsync(Guid countryId, Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateCityAsync(Guid countryId, Guid stateId, CreateCityDto dto, CancellationToken cancellationToken = default);
        Task UpdateCityAsync(Guid countryId, Guid stateId, Guid id, UpdateCityDto dto, CancellationToken cancellationToken = default);
        Task<CityDto> FindCityByIdAsync(Guid countryId, Guid stateId, Guid id, CancellationToken cancellationToken = default);
        Task<List<CityDto>> FindCitiesAsync(Guid countryId, Guid stateId, CancellationToken cancellationToken = default);
        Task DeleteCityAsync(Guid countryId, Guid stateId, Guid id, CancellationToken cancellationToken = default);
    }
}