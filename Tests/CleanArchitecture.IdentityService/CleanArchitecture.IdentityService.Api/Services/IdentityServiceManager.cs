using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using CleanArchitecture.IdentityService.Application.Common.Exceptions;
using CleanArchitecture.IdentityService.Application.Identity;
using CleanArchitecture.IdentityService.Application.Interfaces;
using CleanArchitecture.IdentityService.Domain.Common.Exceptions;
using CleanArchitecture.IdentityService.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IdentityService.IdentityServiceManager", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Api.Services
{
    public class IdentityServiceManager : IIdentityServiceManager
    {
        private const string confirmEmailEndpointName = "ConfirmEmail";
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly SignInManager<ApplicationIdentityUser> _signInManager;
        private readonly IUserStore<ApplicationIdentityUser> _userStore;
        private readonly TimeProvider _timeProvider;
        private readonly IOptionsMonitor<BearerTokenOptions> _bearerTokenOptions;
        private readonly IIdentityEmailSender _emailSender;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public IdentityServiceManager(UserManager<ApplicationIdentityUser> userManager,
            SignInManager<ApplicationIdentityUser> signInManager,
            IUserStore<ApplicationIdentityUser> userStore,
            TimeProvider timeProvider,
            IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
            IIdentityEmailSender emailSender,
            LinkGenerator linkGenerator,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _timeProvider = timeProvider;
            _bearerTokenOptions = bearerTokenOptions;
            _emailSender = emailSender;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<string> ConfirmEmail(string userId, string code, string? changedEmail)
        {
            if (await _userManager.FindByIdAsync(userId) is not { } user)
            {
                throw new ForbiddenAccessException();
            }

            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch
            {
                throw new ForbiddenAccessException();
            }
            IdentityResult result;

            if (string.IsNullOrEmpty(changedEmail))
            {
                result = await _userManager.ConfirmEmailAsync(user, code);
            }
            else
            {
                result = await _userManager.ChangeEmailAsync(user, changedEmail, code);

                if (result.Succeeded)
                {
                    result = await _userManager.SetUserNameAsync(user, changedEmail);
                }
            }

            if (!result.Succeeded)
            {
                throw new ForbiddenAccessException();
            }
            return "Thank you for confirming your email.";
        }

        public async Task ForgotPassword(ForgotPasswordRequestDto resetRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetRequest.Email);

            if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                await _emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
            }
        }

        public async Task<InfoResponseDto> GetInfo()
        {
            if (await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value) is not { } user)
            {
                throw new NotFoundException();
            }
            return await CreateInfoResponse(user, _userManager); ;
        }

        public async Task<AccessTokenResponseDto> Login(LoginRequestDto login, bool? useCookies, bool? useSessionCookies)
        {
            var useCookieScheme = (useCookies == true) || (useSessionCookies == true);
            var isPersistent = (useCookies == true) && (useSessionCookies != true);
            _signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;
            var user = await _userManager.FindByEmailAsync(login.Email);

            if (user is null)
            {
                throw new ForbiddenAccessException();
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                if (!string.IsNullOrEmpty(login.TwoFactorCode))
                {
                    result = await _signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);
                }
                else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
                {
                    result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
                }
            }

            if (!result.Succeeded)
            {
                throw new ForbiddenAccessException(result.ToString());
            }
            var claims = await _userManager.GetClaimsAsync(user);

            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            }
            var token = _tokenService.GenerateAccessToken(user.UserName, claims);
            var refreshToken = _tokenService.GenerateRefreshToken(user.UserName);
            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpired = refreshToken.Expiry;
            await _userManager.UpdateAsync(user);

            var response = new AccessTokenResponseDto
            {
                AccessToken = token.Token,
                ExpiresIn = token.Expiry,
                RefreshToken = refreshToken.Token,
                TokenType = "Bearer"
            };

            return response;
        }

        public async Task<AccessTokenResponseDto> Refresh(RefreshRequestDto refreshRequest)
        {
            var username = _tokenService.GetUsernameFromRefreshToken(refreshRequest.RefreshToken);

            if (username is null)
            {
                throw new ForbiddenAccessException();
            }
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null || user.RefreshToken != refreshRequest.RefreshToken)
            {
                throw new ForbiddenAccessException();
            }
            var claims = await _userManager.GetClaimsAsync(user);

            if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            }
            var token = _tokenService.GenerateAccessToken(user.UserName, claims);
            var refreshToken = _tokenService.GenerateRefreshToken(user.UserName);
            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpired = refreshToken.Expiry;
            await _userManager.UpdateAsync(user);

            var response = new AccessTokenResponseDto
            {
                AccessToken = token.Token,
                ExpiresIn = token.Expiry,
                RefreshToken = refreshToken.Token,
                TokenType = "Bearer"
            };

            return response;
        }

        public async Task Register(RegisterRequestDto registration)
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException($"{nameof(IdentityServiceManager)} requires a user store with email support.");
            }
            var emailStore = (IUserEmailStore<ApplicationIdentityUser>)_userStore;
            var email = registration.Email;
            var user = new ApplicationIdentityUser();
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                throw new Exception();
            }
        }

        public async Task<bool> ResendConfirmationEmail(ResendConfirmationEmailRequestDto resendRequest)
        {
            if (await _userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
            {
                return false;
            }
            await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, resendRequest.Email);
            return true;
        }

        public async Task ResetPassword(ResetPasswordRequestDto resetRequest)
        {
            var user = await _userManager.FindByEmailAsync(resetRequest.Email);

            if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                throw new Exception("Invalid Token");
            }
            IdentityResult result;

            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
                result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
            }
            catch
            {
                throw new Exception("Invalid Token");
            }
        }

        public async Task<InfoResponseDto> UpdateInfo(InfoRequestDto infoRequest)
        {
            if (await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value) is not { } user)
            {
                throw new NotFoundException();
            }

            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    throw new Exception(CreateValidationProblem(changePasswordResult));
                }
            }

            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
            {
                var email = await _userManager.GetEmailAsync(user);

                if (email != infoRequest.NewEmail)
                {
                    await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, infoRequest.NewEmail, isChange: true);
                }
            }
            return await CreateInfoResponse(user, _userManager); ;
        }

        public async Task<TwoFactorResponseDto> UpdateTwoFactor(TwoFactorRequestDto tfaRequest)
        {
            if (await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User) is not { } user)
            {
                throw new NotFoundException();
            }

            if (tfaRequest.Enable == true)
            {
                if (tfaRequest.ResetSharedKey)
                {
                    throw new Exception(CreateValidationProblem("RequiresTwoFactor", "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa."));
                }
                else if (string.IsNullOrEmpty(tfaRequest.TwoFactorCode))
                {
                    throw new Exception(CreateValidationProblem("RequiresTwoFactor", "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa."));
                }
                else if (!await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode))
                {
                    throw new Exception(CreateValidationProblem("InvalidTwoFactorCode", "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa."));
                }
                await _userManager.SetTwoFactorEnabledAsync(user, true);
            }
            else if (tfaRequest.Enable == false || tfaRequest.ResetSharedKey)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
            }

            if (tfaRequest.ResetSharedKey)
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
            }
            string[]? recoveryCodes = null;

            if (tfaRequest.ResetRecoveryCodes || (tfaRequest.Enable == true && await _userManager.CountRecoveryCodesAsync(user) == 0))
            {
                var recoveryCodesEnumerable = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                recoveryCodes = recoveryCodesEnumerable?.ToArray();
            }

            if (tfaRequest.ForgetMachine)
            {
                await _signInManager.ForgetTwoFactorClientAsync();
            }
            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);

                if (string.IsNullOrEmpty(key))
                {
                    throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
                }
            }

            var response = new TwoFactorResponseDto
            {
                SharedKey = key,
                RecoveryCodes = recoveryCodes.ToList(),
                RecoveryCodesLeft = recoveryCodes?.Length ?? await _userManager.CountRecoveryCodesAsync(user),
                IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };

            return response;
        }

        private static async Task<InfoResponseDto> CreateInfoResponse<TUser>(TUser user, UserManager<TUser> userManager)
            where TUser : class
        {
            return new() { Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."), IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user) };
        }

        private static string CreateValidationProblem(IdentityResult result)
        {
            Debug.Assert(!result.Succeeded);
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }
                errorDictionary[error.Code] = newDescriptions;
            }
            return string.Join("; ", errorDictionary.Select(kvp => $"{kvp.Key}={string.Join(",", kvp.Value)}"));
        }

        private static string CreateValidationProblem(string errorCode, string errorDescription)
        {
            return $"{errorCode}={errorDescription}";
        }

        private async Task SendConfirmationEmailAsync(
            ApplicationIdentityUser user,
            UserManager<ApplicationIdentityUser> userManager,
            HttpContext context,
            string email,
            bool isChange = false)
        {
            if (confirmEmailEndpointName is null)
            {
                throw new NotSupportedException("No email confirmation endpoint was registered!");
            }
            var code = isChange ? await userManager.GenerateChangeEmailTokenAsync(user, email) : await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var userId = await userManager.GetUserIdAsync(user);

            var routeValues = new RouteValueDictionary()
            {
                ["userId"] = userId,
                ["code"] = code
            };

            if (isChange)
            {
                routeValues.Add("changedEmail", email);
            }
            var confirmEmailUrl = _linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues) ?? throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");
            await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
        }
    }
}