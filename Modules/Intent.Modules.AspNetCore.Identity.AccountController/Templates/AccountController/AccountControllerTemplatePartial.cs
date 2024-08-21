using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AccountControllerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.AccountController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityModel(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.ComponentModel.DataAnnotations")
                .AddUsing("System.Linq")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Text")
                .AddUsing("System.Text.Encodings.Web")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Authorization")
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("Microsoft.AspNetCore.Mvc.ModelBinding")
                .AddUsing("Microsoft.AspNetCore.WebUtilities")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddClass("AccountController", @class =>
                {
                    @class.AddAttribute("[Route(\"api/[controller]/[action]\")]");
                    @class.AddAttribute("[ApiController]");
                    @class.WithBaseType("ControllerBase");

                    @class.AddField("EmailAddressAttribute", "EmailAddressAttribute", f => f
                        .WithComments("// Validate the email address using DataAnnotations like the UserValidator does when RequireUniqueEmail = true.")
                        .PrivateReadOnly()
                        .Static()
                        .WithAssignment(new CSharpStatement("new EmailAddressAttribute()")));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"IUserStore<{this.GetIdentityUserClass()}>", "userStore", param => param.IntroduceReadonlyField());
                        ctor.AddParameter($"UserManager<{this.GetIdentityUserClass()}>", "userManager", param => param.IntroduceReadonlyField());
                        ctor.AddParameter($"ILogger<{ClassName}>", "logger", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetAccountEmailSenderInterfaceName(), "accountEmailSender", param => param.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetTokenServiceInterfaceName(), "tokenService", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod("Task<IActionResult>", "Register", method =>
                    {
                        method.AddAttribute("HttpPost");
                        method.AddAttribute("AllowAnonymous");
                        method.AddParameter("RegisterDto", "input");
                        method.Async();
                        method.AddStatements($@"
                            if (string.IsNullOrWhiteSpace(input.Email))
                            {{
                                ModelState.AddModelError<RegisterDto>(x => x.Email, ""Mandatory"");
                            }}

                            if (string.IsNullOrWhiteSpace(input.Password))
                            {{
                                ModelState.AddModelError<RegisterDto>(x => x.Password, ""Mandatory"");
                            }}

                            if (!ModelState.IsValid)
                            {{
                                return BadRequest(ModelState);
                            }}

                            var user = new {this.GetIdentityUserClass()}();

                            await _userStore.SetUserNameAsync(user, input.Email, CancellationToken.None);
                            await _userManager.SetEmailAsync(user, input.Email);
                            var result = await _userManager.CreateAsync(user, input.Password!);

                            if (!result.Succeeded)
                            {{
                                foreach (var error in result.Errors)
                                {{
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }}

                                return BadRequest(ModelState);
                            }}

                            _logger.LogInformation(""User created a new account with password."");

                            if (_userManager.Options.SignIn.RequireConfirmedAccount)
                            {{
                                await SendConfirmationEmail(user);
                            }}

                            return Ok();".ConvertToStatements());
                    });

                    @class.AddMethod("Task<ActionResult<TokenResultDto>>", "Login", method =>
                    {
                        method.AddAttribute("HttpPost");
                        method.AddAttribute("AllowAnonymous");
                        method.AddParameter("LoginDto", "input");
                        method.Async();
                        method.AddStatements($@"
                            if (string.IsNullOrWhiteSpace(input.Email))
                            {{
                                ModelState.AddModelError<LoginDto>(x => x.Email, ""Mandatory"");
                            }}

                            if (string.IsNullOrWhiteSpace(input.Password))
                            {{
                                ModelState.AddModelError<LoginDto>(x => x.Password, ""Mandatory"");
                            }}

                            if (!ModelState.IsValid)
                            {{
                                return BadRequest(ModelState);
                            }}

                            var email = input.Email!;
                            var password = input.Password!;

                            var user = await _userManager.FindByEmailAsync(email);
                            if (user == null ||
                                !await _userManager.CheckPasswordAsync(user, password))
                            {{
                                _logger.LogWarning(""Invalid login attempt."");
                                return Forbid();
                            }}

                            if (await _userManager.IsLockedOutAsync(user))
                            {{
                                _logger.LogWarning(""User account locked out."");
                                return Forbid();
                            }}

                            var claims = await GetClaims(user);

                            var (token, expiry) = _tokenService.GenerateAccessToken(username: user.Email!, claims: claims.ToArray());
                            var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshToken(user.Email!);

                            user.RefreshToken = refreshToken;
                            user.RefreshTokenExpired = refreshTokenExpiry;
                            await _userManager.UpdateAsync(user);

                            _logger.LogInformation(""User logged in."");

                            return Ok(new TokenResultDto
                            {{
                                AuthenticationToken = token,
                                ExpiresIn = (int)(expiry - DateTime.UtcNow).TotalSeconds,
                                RefreshToken = refreshToken
                            }});".ConvertToStatements());
                    });

                    @class.AddMethod("ActionResult<TokenResultDto>", "Refresh", method =>
                    {
                        method.AddAttribute("HttpPost");
                        method.AddAttribute("AllowAnonymous");
                        method.AddParameter("RefreshTokenDto", "dto");
                        method.Async();
                        method.AddStatements($@"
                            var username = _tokenService.GetUsernameFromRefreshToken(dto.RefreshToken);
                            if (username == null)
                            {{
                                return BadRequest();
                            }}

                            var user = await _userManager.FindByNameAsync(username);
                            if (user == null || user.RefreshToken != dto.RefreshToken)
                            {{
                                return BadRequest();
                            }}

                            var claims = await GetClaims(user);

                            var (token, expiry) = _tokenService.GenerateAccessToken(user.Email!, claims);
                            var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshToken(user.Email!);

                            user.RefreshToken = refreshToken;
                            user.RefreshTokenExpired = refreshTokenExpiry;
                            await _userManager.UpdateAsync(user);

                            return Ok(new TokenResultDto
                            {{
                                AuthenticationToken = token,
                                ExpiresIn = (int)(expiry - DateTime.UtcNow).TotalSeconds,
                                RefreshToken = refreshToken
                            }});".ConvertToStatements());
                    });

                    @class.AddMethod("Task<IActionResult>", "ConfirmEmail", method =>
                    {
                        method.AddAttribute("HttpPost");
                        method.AddAttribute("AllowAnonymous");
                        method.AddParameter("ConfirmEmailDto", "input");
                        method.Async();
                        method.AddStatements($@"
                            if (string.IsNullOrWhiteSpace(input.UserId))
                            {{
                                ModelState.AddModelError<ConfirmEmailDto>(x => x.UserId, ""Mandatory"");
                            }}

                            if (string.IsNullOrWhiteSpace(input.Code))
                            {{
                                ModelState.AddModelError<ConfirmEmailDto>(x => x.Code, ""Mandatory"");
                            }}

                            if (!ModelState.IsValid)
                            {{
                                return BadRequest(ModelState);
                            }}

                            var userId = input.UserId!;
                            var code = input.Code!;
                            var user = await _userManager.FindByIdAsync(input.UserId!);
                            if (user == null)
                            {{
                                return NotFound($""Unable to load user with ID '{{userId}}'."");
                            }}

                            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

                            var result = await _userManager.ConfirmEmailAsync(user, code);
                            if (!result.Succeeded)
                            {{
                                ModelState.AddModelError<ConfirmEmailDto>(x => x, ""Error confirming your email."");
                                return BadRequest(ModelState);
                            }}

                            return Ok();".ConvertToStatements());
                    });

                    @class.AddMethod("IActionResult", "ForgotPassword", method =>
                    {
                        method.AddAttribute("[HttpPost(\"~/api/[controller]/forgotPassword\")]");
                        method.AddAttribute("AllowAnonymous");
                        method.Async();
                        method.AddParameter("ForgotPasswordDto", "resetRequest");
                        method.AddStatements($@"
                            var user = await _userManager.FindByEmailAsync(resetRequest.Email!);

                            if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
                            {{
                                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                                await _accountEmailSender.SendPasswordResetCode(resetRequest.Email!, user.Id,
                                    HtmlEncoder.Default.Encode(code));
                            }}

                            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                            // returned a 400 for an invalid code given a valid user email.
                            return Ok();".ConvertToStatements());
                    });

                    @class.AddMethod("IActionResult", "ResetPassword", method =>
                    {
                        method.AddAttribute("[HttpPost(\"~/api/[controller]/resetPassword\")]");
                        method.AddAttribute("AllowAnonymous");
                        method.Async();
                        method.AddParameter("ResetPasswordDto", "resetRequest");
                        method.AddStatements($@"
                            var modelState = new ModelStateDictionary();

                            var user = await _userManager.FindByEmailAsync(resetRequest.Email!);

                            if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
                            {{
                                // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
                                // returned a 400 for an invalid code given a valid user email.
                                modelState.AddModelError<ResetPasswordDto>(x => x.ResetCode, ""Invalid token"");
                                return ValidationProblem();
                            }}

                            IdentityResult result;
                            try
                            {{
                                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode!));
                                result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword!);
                            }}
                            catch (FormatException)
                            {{
                                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
                            }}

                            if (!result.Succeeded)
                            {{
                                foreach (var error in result.Errors)
                                {{
                                    modelState.AddModelError(string.Empty, error.Description);
                                }}

                                return ValidationProblem(modelState);
                            }}

                            return Ok();".ConvertToStatements());
                    });

                    @class.AddMethod("ActionResult<InfoResponseDto>", "GetInfo", method =>
                    {
                        method.AddAttribute("[HttpGet(\"~/api/[controller]/manage/info\")]");
                        method.AddAttribute("Authorize");
                        method.Async();
                        method.AddStatements($@"
                            var user = await _userManager.GetUserAsync(User);

                            return new InfoResponseDto
                            {{
                                Email = user?.Email
                            }};".ConvertToStatements());
                    });

                    @class.AddMethod("ActionResult<InfoResponseDto>", "PostInfo", method =>
                    {
                        method.AddAttribute("[HttpPost(\"~/api/[controller]/manage/info\")]");
                        method.AddAttribute("Authorize");
                        method.Async();
                        method.AddParameter("UpdateInfoDto", "infoRequest");
                        method.AddStatements($@"
                            if (await _userManager.GetUserAsync(User) is not {{ }} user)
                            {{
                                return NotFound();
                            }}

                            var modelState = new ModelStateDictionary();

                            if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !EmailAddressAttribute.IsValid(infoRequest.NewEmail))
                            {{
                                modelState.AddModelError<UpdateInfoDto>(x => x.NewEmail, ""Invalid email address."");
                                return ValidationProblem(modelState);
                            }}

                            if (!string.IsNullOrEmpty(infoRequest.NewPassword))
                            {{
                                if (string.IsNullOrEmpty(infoRequest.OldPassword))
                                {{
                                    modelState.AddModelError<UpdateInfoDto>(x => x.OldPassword, ""The old password is required to set a new password. If the old password is forgotten, use /resetPassword."");
                                    return ValidationProblem(modelState);
                                }}

                                var changePasswordResult = await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                                if (!changePasswordResult.Succeeded)
                                {{
                                    foreach (var error in changePasswordResult.Errors)
                                    {{
                                        modelState.AddModelError<UpdateInfoDto>(x => x.NewPassword, error.Description);
                                    }}

                                    return ValidationProblem(modelState);
                                }}
                            }}

                            if (!string.IsNullOrEmpty(infoRequest.NewEmail))
                            {{
                                var email = await _userManager.GetEmailAsync(user);
                                if (email != infoRequest.NewEmail)
                                {{
                                    await _userStore.SetUserNameAsync(user, infoRequest.NewEmail, CancellationToken.None);
                                    await _userManager.SetEmailAsync(user, infoRequest.NewEmail);
                                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                                    {{
                                        await SendConfirmationEmail(user);
                                    }}
                                }}
                            }}

                            return new InfoResponseDto
                            {{
                                Email = user.Email
                            }};".ConvertToStatements());
                    });

                    @class.AddMethod("IActionResult", "Logout", method =>
                    {
                        method.AddAttribute("HttpPost");
                        method.AddAttribute("Authorize");
                        method.Async();
                        method.AddStatements($@"
                            var username = User.Identity!.Name!;
                            var user = (await _userManager.FindByNameAsync(username))!;
                            user.RefreshToken = null;
                            user.RefreshTokenExpired = null;
                            await _userManager.UpdateAsync(user);
                            
                            _logger.LogInformation($""User [{{username}}] logged out the system."");
                            return Ok();".ConvertToStatements());
                    });

                    @class.AddMethod("void", "SendConfirmationEmail", method =>
                    {
                        method.Private().Async();
                        method.AddParameter(this.GetIdentityUserClass(), "user");
                        method.AddStatements(@"
                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                            var userId = await _userManager.GetUserIdAsync(user);

                            await _accountEmailSender.SendEmailConfirmationRequest(
                                email: user.Email!,
                                userId: userId,
                                code: code);".ConvertToStatements());
                    });

                    @class.AddMethod("IList<Claim>", "GetClaims", method =>
                    {
                        method.Private().Async();
                        method.AddParameter(this.GetIdentityUserClass(), "user");
                        method.AddStatements(@"
                            var claims = await _userManager.GetClaimsAsync(user);
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

                            var roles = await _userManager.GetRolesAsync(user);
                            foreach (var role in roles)
                            {
                                claims.Add(new Claim(""role"", role));
                            }

                            return claims;".ConvertToStatements());
                    });
                })
                .AddClass("TokenResultDto", @class =>
                {
                    @class.AddProperty("string", "TokenType", p => p.WithoutSetter().Getter.WithExpressionImplementation("\"Bearer\""));
                    @class.AddProperty("string?", "AuthenticationToken");
                    @class.AddProperty("int", "ExpiresIn");
                    @class.AddProperty("string?", "RefreshToken");
                })
                .AddClass("RegisterDto", @class =>
                {
                    @class.AddProperty("string?", "Email");
                    @class.AddProperty("string?", "Password");
                })
                .AddClass("LoginDto", @class =>
                {
                    @class.AddProperty("string?", "Email");
                    @class.AddProperty("string?", "Password");
                })
                .AddClass("ConfirmEmailDto", @class =>
                {
                    @class.AddProperty("string?", "UserId");
                    @class.AddProperty("string?", "Code");
                })
                .AddClass("RefreshTokenDto", @class =>
                {
                    @class.AddProperty("string?", "RefreshToken");
                })
                .AddClass("UpdateInfoDto", @class =>
                {
                    @class.AddProperty("string?", "NewEmail");
                    @class.AddProperty("string?", "NewPassword");
                    @class.AddProperty("string?", "OldPassword");
                })
                .AddClass("InfoResponseDto", @class =>
                {
                    @class.AddProperty("string?", "Email");
                })
                .AddClass("ForgotPasswordDto", @class =>
                {
                    @class.AddProperty("string?", "Email");
                })
                .AddClass("ResetPasswordDto", @class =>
                {
                    @class.AddProperty("string?", "Email");
                    @class.AddProperty("string?", "ResetCode");
                    @class.AddProperty("string?", "NewPassword");
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        public override void BeforeTemplateExecution()
        {
            var randomBytesBuffer = new byte[32];
            Random.Shared.NextBytes(randomBytesBuffer);

            this.ApplyAppSetting("JwtToken", new
            {
                Issuer = OutputTarget.ExecutionContext.GetApplicationConfig().Name,
                Audience = OutputTarget.ExecutionContext.GetApplicationConfig().Name,
                SigningKey = Convert.ToBase64String(randomBytesBuffer),
                AuthTokenExpiryMinutes = 120,
                RefreshTokenExpiryMinutes = 3
            });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}