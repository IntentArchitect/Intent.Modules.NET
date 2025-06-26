using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Countries;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class CountriesService : ICountriesService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CountriesService(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateCountry(CreateCountryDto dto, CancellationToken cancellationToken = default)
        {
            var country = new Country
            {
                MaE = dto.MaE
            };

            _countryRepository.Add(country);
            await _countryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return country.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCountry(Guid id, UpdateCountryDto dto, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(id, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{id}'");
            }

            country.MaE = dto.MaE;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CountryDto> FindCountryById(Guid id, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(id, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{id}'");
            }
            return country.MapToCountryDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CountryDto>> FindCountries(CancellationToken cancellationToken = default)
        {
            var countries = await _countryRepository.FindAllAsync(cancellationToken);
            return countries.MapToCountryDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCountry(Guid id, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(id, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{id}'");
            }


            _countryRepository.Remove(country);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateState(
            Guid countryId,
            CreateStateDto dto,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }
            var state = new State
            {
                Name = dto.Name,
                CountryId = dto.CountryId
            };

            country.States.Add(state);
            await _countryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return state.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateState(
            Guid countryId,
            Guid id,
            UpdateStateDto dto,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.FirstOrDefault(x => x.Id == id);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{id}'");
            }

            state.Name = dto.Name;
            state.CountryId = dto.CountryId;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<StateDto> FindStateById(Guid countryId, Guid id, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.FirstOrDefault(x => x.Id == id);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{id}'");
            }
            return state.MapToStateDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<StateDto>> FindStates(Guid countryId, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var states = country.States;
            return states.MapToStateDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteState(Guid countryId, Guid id, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.FirstOrDefault(x => x.Id == id);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{id}'");
            }


            country.States.Remove(state);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateCity(
            Guid countryId,
            Guid stateId,
            CreateCityDto dto,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.SingleOrDefault(x => x.Id == stateId);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{stateId}'");
            }

            var city = new City
            {
                Name = dto.Name,
                StateId = dto.StateId
            };

            state.Cities.Add(city);
            await _countryRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return city.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateCity(
            Guid countryId,
            Guid stateId,
            Guid id,
            UpdateCityDto dto,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.SingleOrDefault(x => x.Id == stateId);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{stateId}'");
            }

            var city = state.Cities.FirstOrDefault(x => x.Id == id);
            if (city is null)
            {
                throw new NotFoundException($"Could not find City '{id}'");
            }

            city.Name = dto.Name;
            city.StateId = dto.StateId;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CityDto> FindCityById(
            Guid countryId,
            Guid stateId,
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.SingleOrDefault(x => x.Id == stateId);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{stateId}'");
            }

            var city = state.Cities.FirstOrDefault(x => x.Id == id);
            if (city is null)
            {
                throw new NotFoundException($"Could not find City '{id}'");
            }
            return city.MapToCityDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CityDto>> FindCities(
            Guid countryId,
            Guid stateId,
            CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.SingleOrDefault(x => x.Id == stateId);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{stateId}'");
            }

            var cities = state.Cities;
            return cities.MapToCityDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteCity(Guid countryId, Guid stateId, Guid id, CancellationToken cancellationToken = default)
        {
            var country = await _countryRepository.FindByIdAsync(countryId, cancellationToken);
            if (country is null)
            {
                throw new NotFoundException($"Could not find Country '{countryId}'");
            }

            var state = country.States.SingleOrDefault(x => x.Id == stateId);
            if (state is null)
            {
                throw new NotFoundException($"Could not find State '{stateId}'");
            }

            var city = state.Cities.FirstOrDefault(x => x.Id == id);
            if (city is null)
            {
                throw new NotFoundException($"Could not find City '{id}'");
            }


            state.Cities.Remove(city);
        }
    }
}