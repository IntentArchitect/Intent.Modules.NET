using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Application.Identity.AccountController.Api.Services;
using Application.Identity.AccountController.Application.Account;
using Application.Identity.AccountController.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Identity.AccountController.AccountController", Version = "1.0")]

namespace Application.Identity.AccountController.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.
        private static readonly EmailAddressAttribute EmailAddressAttribute = new EmailAddressAttribute();
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly IUserStore<ApplicationIdentityUser> _userStore;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountEmailSender _accountEmailSender;
        private readonly ITokenService _tokenService;

        public AccountController(IUserStore<ApplicationIdentityUser> userStore,
            UserManager<ApplicationIdentityUser> userManager,
            ILogger<AccountController> logger,
            IAccountEmailSender accountEmailSender,
            ITokenService tokenService)
        {
            _userStore = userStore;
            _userManager = userManager;
            _logger = logger;
            _accountEmailSender = accountEmailSender;
            _tokenService = tokenService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
            {
                ModelState.AddModelError<RegisterDto>(x => x.Email, "Mandatory");
            }

            if (string.IsNullOrWhiteSpace(input.Password))
            {
                ModelState.AddModelError<RegisterDto>(x => x.Password, "Mandatory");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationIdentityUser();

            await _userStore.SetUserNameAsync(user, input.Email, CancellationToken.None);
            await _userManager.SetEmailAsync(user, input.Email);
            var result = await _userManager.CreateAsync(user, input.Password!);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }

            _logger.LogInformation("User created a new account with password.");

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                await SendConfirmationEmail(user);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResultDto>> Login(LoginDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
            {
                ModelState.AddModelError<LoginDto>(x => x.Email, "Mandatory");
            }

            if (string.IsNullOrWhiteSpace(input.Password))
            {
                ModelState.AddModelError<LoginDto>(x => x.Password, "Mandatory");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var email = input.Email!;
            var password = input.Password!;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null ||
                !await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Invalid login attempt.");
                return Forbid();
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("User account locked out.");
                return Forbid();
            }

            var claims = await GetClaims(user);

            var (token, expiry) = _tokenService.GenerateAccessToken(username: user.Email!, claims: claims.ToArray());
            var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshToken(user.Email!);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpired = refreshTokenExpiry;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User logged in.");

            return Ok(new TokenResultDto
            {
                AuthenticationToken = token,
                ExpiresIn = (int)(expiry - DateTime.UtcNow).TotalSeconds,
                RefreshToken = refreshToken
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResultDto>> Refresh(RefreshTokenDto dto)
        {
            var username = _tokenService.GetUsernameFromRefreshToken(dto.RefreshToken);
            if (username == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != dto.RefreshToken)
            {
                return BadRequest();
            }

            var claims = await GetClaims(user);

            var (token, expiry) = _tokenService.GenerateAccessToken(user.Email!, claims);
            var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshToken(user.Email!);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpired = refreshTokenExpiry;
            await _userManager.UpdateAsync(user);

            return Ok(new TokenResultDto
            {
                AuthenticationToken = token,
                ExpiresIn = (int)(expiry - DateTime.UtcNow).TotalSeconds,
                RefreshToken = refreshToken
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto input)
        {
            if (string.IsNullOrWhiteSpace(input.UserId))
            {
                ModelState.AddModelError<ConfirmEmailDto>(x => x.UserId, "Mandatory");
            }

            if (string.IsNullOrWhiteSpace(input.Code))
            {
                ModelState.AddModelError<ConfirmEmailDto>(x => x.Code, "Mandatory");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = input.UserId!;
            var code = input.Code!;
            var user = await _userManager.FindByIdAsync(input.UserId!);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                ModelState.AddModelError<ConfirmEmailDto>(x => x, "Error confirming your email.");
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost("~/api/[controller]/forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto resetRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetRequest.Email!);

            if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                await _accountEmailSender.SendPasswordResetCode(resetRequest.Email!, user.Id,
                    HtmlEncoder.Default.Encode(code));
            }

            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
            // returned a 400 for an invalid code given a valid user email.
            return Ok();
        }

        [HttpPost("~/api/[controller]/resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetRequest)
        {
            var modelState = new ModelStateDictionary();

            var user = await _userManager.FindByEmailAsync(resetRequest.Email!);

            if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                // returned a 400 for an invalid code given a valid user email.
                modelState.AddModelError<ResetPasswordDto>(x => x.ResetCode, "Invalid token");
                return ValidationProblem();
            }

            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode!));
                result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword!);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem(modelState);
            }

            return Ok();
        }

        [HttpGet("~/api/[controller]/manage/info")]
        [Authorize]
        public async Task<ActionResult<InfoResponseDto>> GetInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            return new InfoResponseDto
            {
                Email = user?.Email
            };
        }

        [HttpPost("~/api/[controller]/manage/info")]
        [Authorize]
        public async Task<ActionResult<InfoResponseDto>> PostInfo(UpdateInfoDto infoRequest)
        {
            if (await _userManager.GetUserAsync(User) is not { } user)
            {
                return NotFound();
            }

            var modelState = new ModelStateDictionary();

            if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !EmailAddressAttribute.IsValid(infoRequest.NewEmail))
            {
                modelState.AddModelError<UpdateInfoDto>(x => x.NewEmail, "Invalid email address.");
                return ValidationProblem(modelState);
            }

            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
            {
                if (string.IsNullOrEmpty(infoRequest.OldPassword))
                {
                    modelState.AddModelError<UpdateInfoDto>(x => x.OldPassword, "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
                    return ValidationProblem(modelState);
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        modelState.AddModelError<UpdateInfoDto>(x => x.NewPassword, error.Description);
                    }

                    return ValidationProblem(modelState);
                }
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
            {
                var email = await _userManager.GetEmailAsync(user);
                if (email != infoRequest.NewEmail)
                {
                    await _userStore.SetUserNameAsync(user, infoRequest.NewEmail, CancellationToken.None);
                    await _userManager.SetEmailAsync(user, infoRequest.NewEmail);
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        await SendConfirmationEmail(user);
                    }
                }
            }

            return new InfoResponseDto
            {
                Email = user.Email
            };
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity!.Name!;
            var user = (await _userManager.FindByNameAsync(username))!;
            user.RefreshToken = null;
            user.RefreshTokenExpired = null;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User [{username}] logged out the system.");
            return Ok();
        }

        private async Task SendConfirmationEmail(ApplicationIdentityUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var userId = await _userManager.GetUserIdAsync(user);

            await _accountEmailSender.SendEmailConfirmationRequest(
                email: user.Email!,
                userId: userId,
                code: code);
        }

        private async Task<IList<Claim>> GetClaims(ApplicationIdentityUser user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            return claims;
        }
    }

    public class TokenResultDto
    {
        public string TokenType => "Bearer";
        public string? AuthenticationToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class RegisterDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class ConfirmEmailDto
    {
        public string? UserId { get; set; }
        public string? Code { get; set; }
    }

    public class RefreshTokenDto
    {
        public string? RefreshToken { get; set; }
    }

    public class UpdateInfoDto
    {
        public string? NewEmail { get; set; }
        public string? NewPassword { get; set; }
        public string? OldPassword { get; set; }
    }

    public class InfoResponseDto
    {
        public string? Email { get; set; }
    }

    public class ForgotPasswordDto
    {
        public string? Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public string? Email { get; set; }
        public string? ResetCode { get; set; }
        public string? NewPassword { get; set; }
    }
}
