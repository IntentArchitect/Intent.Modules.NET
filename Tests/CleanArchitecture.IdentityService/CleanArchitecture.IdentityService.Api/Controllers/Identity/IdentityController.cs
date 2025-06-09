using System.Transactions;
using CleanArchitecture.IdentityService.Application.Common.Validation;
using CleanArchitecture.IdentityService.Application.Identity;
using CleanArchitecture.IdentityService.Application.Interfaces.Identity;
using CleanArchitecture.IdentityService.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.Controller", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Api.Controllers.Identity
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _appService;
        private readonly IValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;

        public IdentityController(IIdentityService appService, IValidationService validationService, IUnitOfWork unitOfWork)
        {
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified string.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="404">One or more entities could not be found with the provided parameters.</response>
        [HttpGet("confirmEmail")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("ConfirmEmailAsync")]
        public async Task<ActionResult<string>> ConfirmEmailAsync(
            [FromQuery] string userId,
            [FromQuery] string code,
            [FromQuery] string? changedEmail,
            CancellationToken cancellationToken = default)
        {
            var result = default(string);
            result = await _appService.ConfirmEmailAsync(userId, code, changedEmail, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("forgotPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ForgotPasswordAsync(
            [FromBody] ForgotPasswordRequestDto resetRequest,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(resetRequest, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.ForgotPasswordAsync(resetRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Returns the specified InfoResponseDto.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        [HttpGet("manage/info")]
        [Authorize]
        [ProducesResponseType(typeof(InfoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InfoResponseDto>> GetInfoAsync(CancellationToken cancellationToken = default)
        {
            var result = default(InfoResponseDto);
            result = await _appService.GetInfoAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AccessTokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccessTokenResponseDto>> LoginAsync(
            [FromBody] LoginRequestDto login,
            [FromQuery] bool? useCookies,
            [FromQuery] bool? useSessionCookies,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(login, cancellationToken);
            var result = default(AccessTokenResponseDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.LoginAsync(login, useCookies, useSessionCookies, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AccessTokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccessTokenResponseDto>> RefreshAsync(
            [FromBody] RefreshRequestDto refreshRequest,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(refreshRequest, cancellationToken);
            var result = default(AccessTokenResponseDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.RefreshAsync(refreshRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RegisterAsync(
            [FromBody] RegisterRequestDto register,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(register, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.RegisterAsync(register, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("resendConfirmationEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResendConfirmationEmailAsync(
            [FromBody] ResendConfirmationEmailRequestDto resendRequest,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(resendRequest, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.ResendConfirmationEmailAsync(resendRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        [HttpPost("resetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetPasswordAsync(
            [FromBody] ResetPasswordRequestDto resetRequest,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(resetRequest, cancellationToken);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                await _appService.ResetPasswordAsync(resetRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok();
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        [HttpPost("manage/info")]
        [Authorize]
        [ProducesResponseType(typeof(InfoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<InfoResponseDto>> UpdateInfoAsync(
            [FromBody] InfoRequestDto infoRequest,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(infoRequest, cancellationToken);
            var result = default(InfoResponseDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.UpdateInfoAsync(infoRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok(result);
        }

        /// <summary>
        /// </summary>
        /// <response code="200">Successfully created.</response>
        /// <response code="400">One or more validation errors have occurred.</response>
        /// <response code="401">Unauthorized request.</response>
        /// <response code="403">Forbidden request.</response>
        [HttpPost("manage/2fa")]
        [Authorize]
        [ProducesResponseType(typeof(TwoFactorResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TwoFactorResponseDto>> UpdateTwoFactorAsync(
            [FromBody] TwoFactorRequestDto tfaRequest,
            CancellationToken cancellationToken = default)
        {
            await _validationService.Handle(tfaRequest, cancellationToken);
            var result = default(TwoFactorResponseDto);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                result = await _appService.UpdateTwoFactorAsync(tfaRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                transaction.Complete();
            }
            return Ok(result);
        }
    }
}