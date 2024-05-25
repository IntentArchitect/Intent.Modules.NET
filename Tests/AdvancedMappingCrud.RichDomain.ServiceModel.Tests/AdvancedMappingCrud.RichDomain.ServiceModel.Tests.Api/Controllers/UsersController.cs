using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Validation;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _appService;
        private readonly IValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUsersService appService, IValidationService validationService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> CreateUser(
            [FromBody] UserCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);
            var result = default(Guid);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.CreateUser(dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return CreatedAtAction(nameof(FindUserById), new { id = result }, result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified UserDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No UserDto could be found with the provided parameters.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> FindUserById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = default(UserDto);
            result = await _appService.FindUserById(id, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified List&lt;UserDto&gt;.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserDto>>> FindUsers(CancellationToken cancellationToken = default)
        {
            var result = default(List<UserDto>);
            result = await _appService.FindUsers(cancellationToken);
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
        public async Task<ActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.DeleteUser(id, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified TestCompanyResultDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No TestCompanyResultDto could be found with the provided parameters.</response>
        [HttpGet("test-entity/{id}")]
        [ProducesResponseType(typeof(TestCompanyResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestCompanyResultDto>> TestEntity(
            [FromRoute] Guid id,
            [FromQuery] TestEntityDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = default(TestCompanyResultDto);
            result = await _appService.TestEntity(id, dto, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified TestAddressDCResultDto?.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpGet("test-d-c/{id}")]
        [ProducesResponseType(typeof(TestAddressDCResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestAddressDCResultDto?>> TestDC(
            [FromRoute] Guid id,
            [FromQuery] TestDCDto dto,
            CancellationToken cancellationToken = default)
        {
            var result = default(TestAddressDCResultDto?);
            result = await _appService.TestDC(id, dto, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified TestContactDetailsVOResultDto.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">No TestContactDetailsVOResultDto could be found with the provided parameters.</response>
        [HttpGet("test-v-o/{id}")]
        [ProducesResponseType(typeof(TestContactDetailsVOResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TestContactDetailsVOResultDto>> TestVO(
            [FromRoute] Guid id,
            [FromQuery] TestVODto dto,
            CancellationToken cancellationToken = default)
        {
            var result = default(TestContactDetailsVOResultDto);
            result = await _appService.TestVO(id, dto, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="201">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpPost("add-collections")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddCollections(
            Guid id,
            [FromBody] AddCollectionsDto dto,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(dto, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.AddCollections(id, dto, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Created(string.Empty, null);
        }
    }
}