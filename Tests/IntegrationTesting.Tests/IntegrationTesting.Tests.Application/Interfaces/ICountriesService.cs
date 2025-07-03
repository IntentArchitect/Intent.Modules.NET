using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Application.Countries;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Interfaces
{
    public interface ICountriesService
    {
        Task<Guid> CreateCity(Guid countryId, Guid stateId, CreateCityDto dto, CancellationToken cancellationToken = default);
        Task UpdateCity(Guid countryId, Guid stateId, Guid id, UpdateCityDto dto, CancellationToken cancellationToken = default);
        Task<CityDto> FindCityById(Guid countryId, Guid stateId, Guid id, CancellationToken cancellationToken = default);
        Task<List<CityDto>> FindCities(Guid countryId, Guid stateId, CancellationToken cancellationToken = default);
        Task DeleteCity(Guid countryId, Guid stateId, Guid id, CancellationToken cancellationToken = default);
        Task ChangeName(Guid countryId, Guid stateId, Guid id, ChangeNameDto dto, CancellationToken cancellationToken = default);
        Task<Guid> CreateState(Guid countryId, CreateStateDto dto, CancellationToken cancellationToken = default);
        Task UpdateState(Guid countryId, Guid id, UpdateStateDto dto, CancellationToken cancellationToken = default);
        Task<StateDto> FindStateById(Guid countryId, Guid id, CancellationToken cancellationToken = default);
        Task<List<StateDto>> FindStates(Guid countryId, CancellationToken cancellationToken = default);
        Task DeleteState(Guid countryId, Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateCountry(CreateCountryDto dto, CancellationToken cancellationToken = default);
        Task UpdateCountry(Guid id, UpdateCountryDto dto, CancellationToken cancellationToken = default);
        Task<CountryDto> FindCountryById(Guid id, CancellationToken cancellationToken = default);
        Task<List<CountryDto>> FindCountries(CancellationToken cancellationToken = default);
        Task DeleteCountry(Guid id, CancellationToken cancellationToken = default);
    }
}