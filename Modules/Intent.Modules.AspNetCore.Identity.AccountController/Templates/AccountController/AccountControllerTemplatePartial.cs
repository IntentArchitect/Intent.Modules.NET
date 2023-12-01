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
    partial class AccountControllerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.AccountController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountControllerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityModel);
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreAuthenticationJwtBearer(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Text")
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

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {{
                await _accountEmailSender.SendEmailConfirmationRequest(
                    email: input.Email!,
                    userId: userId,
                    code: code);
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

            var claims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {{
                claims.Add(new Claim(""role"", role));
            }}

            var token = _tokenService.GenerateAccessToken(username: email, claims: claims.ToArray());
            var (refreshToken, refreshTokenExpiry) = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpired = refreshTokenExpiry;
            await _userManager.UpdateAsync(user);
            
            _logger.LogInformation(""User logged in."");
            
            return Ok(new TokenResultDto
            {{
                AuthenticationToken = token,
                RefreshToken = refreshToken
            }});".ConvertToStatements());
                    });

                    @class.AddMethod("ActionResult<TokenResultDto>", "Refresh", method =>
                    {
                        method.AddAttribute("HttpPost");
                        method.AddAttribute("Authorize");
                        method.AddParameter("RefreshTokenDto", "dto");
                        method.Async();
                        method.AddStatements($@"
            var username = User.Identity!.Name!;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != dto.RefreshToken)
            {{
                return BadRequest();
            }}

            var claims = await _userManager.GetClaimsAsync(user);

            var newJwtToken = _tokenService.GenerateAccessToken(username, claims);
            var (token, expiry) = _tokenService.GenerateRefreshToken();

            user.RefreshToken = token;
            user.RefreshTokenExpired = expiry;
            await _userManager.UpdateAsync(user);

            return Ok(new TokenResultDto
            {{
                AuthenticationToken = newJwtToken,
                RefreshToken = token
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
                })
                .AddClass("TokenResultDto", @class =>
                {
                    @class.AddProperty("string?", "AuthenticationToken");
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