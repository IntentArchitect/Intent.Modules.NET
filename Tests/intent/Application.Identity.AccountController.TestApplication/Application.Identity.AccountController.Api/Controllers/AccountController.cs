using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private readonly SignInManager<ApplicationIdentityUser> _signInManager;
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly IUserStore<ApplicationIdentityUser> _userStore;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountEmailSender _accountEmailSender;
        private readonly ITokenService _tokenService;

        public AccountController(
            SignInManager<ApplicationIdentityUser> signInManager,
            IUserStore<ApplicationIdentityUser> userStore,
            UserManager<ApplicationIdentityUser> userManager,
            ILogger<AccountController> logger,
            IAccountEmailSender accountEmailSender,
            ITokenService tokenService)
        {
            _signInManager = signInManager;
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

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                await _accountEmailSender.SendEmailConfirmationRequest(
                    email: input.Email!,
                    userId: userId,
                    code: code);
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

            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var token = _tokenService.GenerateAccessToken(username: email, claims: claims.ToArray());
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpired = newRefreshToken.Expiry;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User logged in.");

            return Ok(new TokenResultDto
            {
                AuthenticationToken = token,
                RefreshToken = newRefreshToken.Token
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResultDto>> RefreshToken(string authenticationToken, string refreshToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(authenticationToken);
            var username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshToken) return BadRequest();

            var newJwtToken = _tokenService.GenerateAccessToken(username, principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpired = newRefreshToken.Expiry;
            await _userManager.UpdateAsync(user);

            return Ok(new TokenResultDto
            {
                AuthenticationToken = newJwtToken,
                RefreshToken = newRefreshToken.Token
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            var user = await _userManager.FindByNameAsync(username);
            user.RefreshToken = null;
            user.RefreshTokenExpired = null;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"User [{username}] logged out the system.");
            return Ok();
        }
    }

    public class TokenResultDto
    {
        public string? AuthenticationToken { get; set; }
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
}
