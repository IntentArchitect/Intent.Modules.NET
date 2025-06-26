using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AdvancedMappingCrud.Repositories.Tests.Api.Controllers.ResponseTypes;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Eventing;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Validation;
using AdvancedMappingCrud.Repositories.Tests.Application.Countries;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountriesService _appService;
        private readonly IValidationService _validationService;
        private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public CountriesController(ICountriesService appService,
            IValidationService validationService,
            IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork,
            IUnitOfWork unitOfWork,
            IEventBus eventBus)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCountry(
            [FromBody] CreateCountryDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = Guid.Empty;

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await _appService.CreateCountry(dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return CreatedAtAction(nameof(FindCountryById), new { id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCountry(
            [FromRoute] Guid id,
            [FromBody] UpdateCountryDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.UpdateCountry(id, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CountryDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CountryDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CountryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CountryDto>> FindCountryById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(CountryDto);
            result = await _appService.FindCountryById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CountryDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CountryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CountryDto>>> FindCountries(CancellationToken cancellationToken = default)
        {
            var result = default(List<CountryDto>);
            result = await _appService.FindCountries(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCountry([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.DeleteCountry(id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("{countryId}/states")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateState(
            [FromRoute] Guid countryId,
            [FromBody] CreateStateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = Guid.Empty;

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await _appService.CreateState(countryId, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return CreatedAtAction(nameof(FindStateById), new { countryId = countryId, id = result }, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("{countryId}/states/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateState(
            [FromRoute] Guid countryId,
            [FromRoute] Guid id,
            [FromBody] UpdateStateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.UpdateState(countryId, id, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified StateDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No StateDto could be found with the provided parameters.</response>
        [HttpGet("{countryId}/states/{id}")]
        [ProducesResponseType(typeof(StateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StateDto>> FindStateById(
            [FromRoute] Guid countryId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(StateDto);
            result = await _appService.FindStateById(countryId, id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;StateDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;StateDto&gt; could be found with the provided parameters.</response>
        [HttpGet("{countryId}/states")]
        [ProducesResponseType(typeof(List<StateDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<StateDto>>> FindStates(
            [FromRoute] Guid countryId,
            CancellationToken cancellationToken = default)
        {
            var result = default(List<StateDto>);
            result = await _appService.FindStates(countryId, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{countryId}/states/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteState(
            [FromRoute] Guid countryId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.DeleteState(countryId, id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("{countryId}/states/{stateId}/cities")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(JsonResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<JsonResponse<Guid>>> CreateCity(
            [FromRoute] Guid countryId,
            [FromRoute] Guid stateId,
            [FromBody] CreateCityDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = Guid.Empty;

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    result = await _appService.CreateCity(countryId, stateId, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return Created(string.Empty, new JsonResponse<Guid>(result));
        }

        /// <summary>
        /// </summary>
        /// <response code="204">Successfully updated.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPut("{countryId}/states/{stateId}/cities/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCity(
            [FromRoute] Guid countryId,
            [FromRoute] Guid stateId,
            [FromRoute] Guid id,
            [FromBody] UpdateCityDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.UpdateCity(countryId, stateId, id, dto, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified CityDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No CityDto could be found with the provided parameters.</response>
        [HttpGet("{countryId}/states/{stateId}/cities/{id}")]
        [ProducesResponseType(typeof(CityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CityDto>> FindCityById(
            [FromRoute] Guid countryId,
            [FromRoute] Guid stateId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(CityDto);
            result = await _appService.FindCityById(countryId, stateId, id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;CityDto&gt;.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No List&lt;CityDto&gt; could be found with the provided parameters.</response>
        [HttpGet("{countryId}/states/{stateId}/cities")]
        [ProducesResponseType(typeof(List<CityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CityDto>>> FindCities(
            [FromRoute] Guid countryId,
            [FromRoute] Guid stateId,
            CancellationToken cancellationToken = default)
        {
            var result = default(List<CityDto>);
            result = await _appService.FindCities(countryId, stateId, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully deleted.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpDelete("{countryId}/states/{stateId}/cities/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteCity(
            [FromRoute] Guid countryId,
            [FromRoute] Guid stateId,
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _appService.DeleteCity(countryId, stateId, id, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }

                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _eventBus.FlushAllAsync(cancellationToken);
            return Ok();
        }
    }
}