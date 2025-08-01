using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Identity;
using Intent.Modules.AspNetCore.IdentityService.Settings;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityEmailSenderInterface;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface;
using Intent.Modules.AspNetCore.IdentityService.Templates.TokenServiceInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManager
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityServiceManagerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IdentityService.IdentityServiceManager";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityServiceManagerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Authentication")
                .AddUsing("Microsoft.AspNetCore.WebUtilities")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.AspNetCore.Authentication.BearerToken")
                .AddUsing("System.Text.Encodings.Web")
                .AddUsing("System.Text")
                .AddUsing("System.Diagnostics")
                .AddUsing("System.IdentityModel.Tokens.Jwt")
                .AddUsing("System.Security.Claims")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Routing")
                .AddClass($"IdentityServiceManager", @class =>
                {
                    GetTypeName(TokenServiceInterfaceTemplate.TemplateId);
                    var interfaceTypeName = GetTypeName("Intent.AspNetCore.IdentityService.IdentityServiceManagerInterface");
                    var dtoModels = this.ExecutionContext.MetadataManager.GetDesigner(this.ExecutionContext.GetApplicationConfig().Id, Designers.Services).GetDTOModels();
                    var loginRequestDto = dtoModels.FirstOrDefault(x => x.Name == "LoginRequestDto");

                    if (loginRequestDto is null)
                    {
                        var package = this.ExecutionContext.MetadataManager.GetDesigner(this.ExecutionContext.GetApplicationConfig().Id, Designers.Services).Packages.FirstOrDefault();
                        if (package is null)
                        {
                            throw new Exception("No package found. Please create a package and uninstall and re-install the Intent.AspNetCore.IdentityService module.");
                        }
                        throw new ElementException(package, "No LoginRequestDto found. Please uninstall and re-install the Intent.AspNetCore.IdentityService module.");
                    }

                    var typeName = GetFullyQualifiedTypeName("Intent.Application.Dtos.DtoModel", loginRequestDto);
                    UseType(typeName);

                    @class.ImplementsInterface(interfaceTypeName);

                    @class.AddField("string", "confirmEmailEndpointName", c => c.PrivateConstant("\"ConfirmEmail\""));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"UserManager<{this.GetIdentityUserClass()}>", "userManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"SignInManager<{this.GetIdentityUserClass()}>", "signInManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"IUserStore<{this.GetIdentityUserClass()}>", "userStore", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("TimeProvider", "timeProvider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IOptionsMonitor<BearerTokenOptions>", "bearerTokenOptions", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"{GetTypeName(IdentityEmailSenderInterfaceTemplate.TemplateId)}", "emailSender", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("LinkGenerator", "linkGenerator", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("ITokenService", "tokenService", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    GetTypeName("Intent.Application.Identity.ForbiddenAccessException");
                    GetTypeName("Intent.Entities.NotFoundException");

                    @class.AddMethod("string", "ConfirmEmail", m =>
                    {
                        m.Async();

                        m.AddParameter("string", "userId");
                        m.AddParameter("string", "code");
                        m.AddParameter("string?", "changedEmail");

                        m.AddIfStatement("await _userManager.FindByIdAsync(userId) is not { } user", c => c.AddStatement("throw new ForbiddenAccessException();"));

                        m.AddTryBlock(t =>
                        {
                            t.AddStatement("code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));");
                        }).AddCatchBlock(c => c.AddStatement("throw new ForbiddenAccessException();"));

                        m.AddStatement("IdentityResult result;");

                        m.AddIfStatement("string.IsNullOrEmpty(changedEmail)", c =>
                        {
                            c.AddStatement("result = await _userManager.ConfirmEmailAsync(user, code);");
                        })
                        .AddElseStatement(e =>
                        {
                            e.AddStatement("result = await _userManager.ChangeEmailAsync(user, changedEmail, code);");
                            e.AddIfStatement("result.Succeeded", eIf => eIf.AddStatement("result = await _userManager.SetUserNameAsync(user, changedEmail);"));
                        });

                        m.AddIfStatement("!result.Succeeded", c => c.AddStatement("throw new ForbiddenAccessException();"));

                        m.AddReturn("\"Thank you for confirming your email.\"");
                    });

                    @class.AddMethod("Task", "ForgotPassword", m =>
                    {
                        m.Async();
                        m.AddParameter("ForgotPasswordRequestDto", "resetRequest");

                        m.AddStatement("var user = await _userManager.FindByEmailAsync(resetRequest.Email);");

                        m.AddIfStatement("user is not null && await _userManager.IsEmailConfirmedAsync(user)", i =>
                        {
                            i.AddStatement("var code = await _userManager.GeneratePasswordResetTokenAsync(user);");
                            i.AddStatement("code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));");
                            i.AddStatement("await _emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));");
                        });
                    });

                    @class.AddMethod("InfoResponseDto", "GetInfo", m =>
                    {
                        m.Async();

                        m.AddIfStatement("await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value) is not { } user", c => c.AddStatement("throw new NotFoundException();"));

                        m.AddReturn("await CreateInfoResponse(user, _userManager);");
                    });

                    @class.AddMethod("AccessTokenResponseDto", "Login", m =>
                    {
                        m.Async();
                        m.AddParameter("LoginRequestDto", "login");
                        m.AddParameter("bool?", "useCookies");
                        m.AddParameter("bool?", "useSessionCookies");

                        m.AddStatement("var useCookieScheme = (useCookies == true) || (useSessionCookies == true);");
                        m.AddStatement("var isPersistent = (useCookies == true) && (useSessionCookies != true);");
                        m.AddStatement("_signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;");

                        m.AddStatement("var user = await _userManager.FindByEmailAsync(login.Email);");

                        m.AddIfStatement("user is null", i =>
                        {
                            i.AddStatement("throw new ForbiddenAccessException();");
                        });

                        m.AddStatement("var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);");

                        m.AddIfStatement("result.RequiresTwoFactor", i =>
                        {
                            i.AddIfStatement("!string.IsNullOrEmpty(login.TwoFactorCode)", iIf =>
                            {
                                iIf.AddStatement("result = await _signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, isPersistent, rememberClient: isPersistent);");
                            }).AddElseIfStatement("!string.IsNullOrEmpty(login.TwoFactorRecoveryCode)", e =>
                            {
                                e.AddStatement("result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);");
                            });
                        });

                        m.AddIfStatement("!result.Succeeded", i =>
                        {
                            i.AddStatement("throw new ForbiddenAccessException(result.ToString());");
                        });

                        m.AddStatement("var claims = await _userManager.GetClaimsAsync(user);");

                        m.AddIfStatement("!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub)", i =>
                        {
                            i.AddStatement("claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));");
                        });

                        m.AddStatement("var token = _tokenService.GenerateAccessToken(user.UserName, claims);");

                        var hasRefreshToken = ExecutionContext.MetadataManager.IdentityClassHasRefreshToken(ExecutionContext.GetApplicationConfig().Id);
                        if (hasRefreshToken)
                        {
                            m.AddStatement("var refreshToken = _tokenService.GenerateRefreshToken(user.UserName);");
                            m.AddStatement("user.RefreshToken = refreshToken.Token;");
                            m.AddStatement("user.RefreshTokenExpired = refreshToken.Expiry;");
                            m.AddStatement("await _userManager.UpdateAsync(user);");
                        }

                        m.AddObjectInitializerBlock("var response = new AccessTokenResponseDto", c =>
                        {
                            c.AddInitStatement("AccessToken", "token.Token");
                            c.AddInitStatement("ExpiresIn", "token.Expiry");
                            if (hasRefreshToken)
                            {
                                c.AddInitStatement("RefreshToken", "refreshToken.Token");
                            }
                            c.AddInitStatement("TokenType", "\"Bearer\"");
                            c.WithSemicolon();
                        });

                        m.AddReturn("response");
                    });

                    @class.AddMethod("AccessTokenResponseDto", "Refresh", m =>
                    {
                        m.Async();
                        m.AddParameter("RefreshRequestDto", "refreshRequest");

                        m.AddStatement("var username = _tokenService.GetUsernameFromRefreshToken(refreshRequest.RefreshToken);");

                        m.AddIfStatement("username is null", i =>
                        {
                            i.AddStatement("throw new ForbiddenAccessException();");
                        });

                        m.AddStatement("var user = await _userManager.FindByEmailAsync(username);");

                        var hasRefreshToken = ExecutionContext.MetadataManager.IdentityClassHasRefreshToken(ExecutionContext.GetApplicationConfig().Id);

                        m.AddIfStatement($"user == null {(hasRefreshToken ? "|| user.RefreshToken != refreshRequest.RefreshToken" : "")}", i =>
                        {
                            i.AddStatement("throw new ForbiddenAccessException();");
                        });

                        m.AddStatement("var claims = await _userManager.GetClaimsAsync(user);");

                        m.AddIfStatement("!claims.Any(c => c.Type == JwtRegisteredClaimNames.Sub)", i =>
                        {
                            i.AddStatement("claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));");
                        });

                        m.AddStatement("var token = _tokenService.GenerateAccessToken(user.UserName, claims);");
                        
                        if (hasRefreshToken)
                        {
                            m.AddStatement("var refreshToken = _tokenService.GenerateRefreshToken(user.UserName);");
                            m.AddStatement("user.RefreshToken = refreshToken.Token;");
                            m.AddStatement("user.RefreshTokenExpired = refreshToken.Expiry;");
                            m.AddStatement("await _userManager.UpdateAsync(user);");
                        }

                        m.AddObjectInitializerBlock("var response = new AccessTokenResponseDto", c =>
                        {
                            c.AddInitStatement("AccessToken", "token.Token");
                            c.AddInitStatement("ExpiresIn", "token.Expiry");
                            if (hasRefreshToken)
                            {
                                c.AddInitStatement("RefreshToken", "refreshToken.Token");
                            }
                            c.AddInitStatement("TokenType", "\"Bearer\"");
                            c.WithSemicolon();
                        });

                        m.AddReturn("response");
                    });

                    @class.AddMethod("Task", "Register", m =>
                    {
                        m.Async();
                        m.AddParameter("RegisterRequestDto", "registration");

                        m.AddIfStatement("!_userManager.SupportsUserEmail", i => i.AddStatement("throw new NotSupportedException($\"{nameof(IdentityServiceManager)} requires a user store with email support.\");"));

                        m.AddStatement($"var emailStore = (IUserEmailStore<{this.GetIdentityUserClass()}>)_userStore;");
                        m.AddStatement("var email = registration.Email;");

                        m.AddStatement($"var user = new {this.GetIdentityUserClass()} {{ Id = Guid.NewGuid().ToString() }};");
                        m.AddStatement("await _userStore.SetUserNameAsync(user, email, CancellationToken.None);");
                        m.AddStatement("await emailStore.SetEmailAsync(user, email, CancellationToken.None);");
                        m.AddStatement("var result = await _userManager.CreateAsync(user, registration.Password);");

                        m.AddIfStatement("!result.Succeeded", i => i.AddStatement("throw new Exception();"));

                        if (ExecutionContext.Settings.GetIdentityServiceSettings().RequiresConfirmedAccount())
                        {
                            m.AddStatement("await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, email);");
                        }
                    });

                    @class.AddMethod("bool", "ResendConfirmationEmail", m =>
                    {
                        m.Async();
                        m.AddParameter("ResendConfirmationEmailRequestDto", "resendRequest");

                        m.AddIfStatement("await _userManager.FindByEmailAsync(resendRequest.Email) is not { } user", i => i.AddReturn("false"));

                        m.AddStatement("await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, resendRequest.Email);");
                        m.AddReturn("true");
                    });

                    @class.AddMethod("Task", "ResetPassword", m =>
                    {
                        m.Async();
                        m.AddParameter("ResetPasswordRequestDto", "resetRequest");

                        m.AddStatement("var user = await _userManager.FindByEmailAsync(resetRequest.Email);");

                        m.AddIfStatement("user is null || !(await _userManager.IsEmailConfirmedAsync(user))", i => i.AddStatement("throw new Exception(\"Invalid Token\");"));

                        m.AddStatement("IdentityResult result;");

                        m.AddTryBlock(t =>
                        {
                            t.AddStatement("var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));");
                            t.AddStatement("result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);");
                        }).AddCatchBlock(c =>
                        {
                            c.AddStatement("throw new Exception(\"Invalid Token\");");
                        });
                    });

                    @class.AddMethod("InfoResponseDto", "UpdateInfo", m =>
                    {
                        m.Async();
                        m.AddParameter("InfoRequestDto", "infoRequest");

                        m.AddIfStatement("await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext?.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value) is not { } user", c => c.AddStatement("throw new NotFoundException();"));

                        m.AddIfStatement("!string.IsNullOrEmpty(infoRequest.NewPassword)", i =>
                        {
                            i.AddStatement("var changePasswordResult = await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);");
                            i.AddIfStatement("!changePasswordResult.Succeeded", iIf => iIf.AddStatement("throw new Exception(CreateValidationProblem(changePasswordResult));"));
                        });

                        m.AddIfStatement("!string.IsNullOrEmpty(infoRequest.NewEmail)", i =>
                        {
                            i.AddStatement("var email = await _userManager.GetEmailAsync(user);");

                            i.AddIfStatement("email != infoRequest.NewEmail", iIf => iIf.AddStatement("await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, infoRequest.NewEmail, isChange: true);"));
                        });

                        m.AddReturn("await CreateInfoResponse(user, _userManager);");
                    });

                    @class.AddMethod("TwoFactorResponseDto", "UpdateTwoFactor", m =>
                    {
                        m.Async();
                        m.AddParameter("TwoFactorRequestDto", "tfaRequest");

                        m.AddIfStatement("await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User) is not { } user", c => c.AddStatement("throw new NotFoundException();"));

                        m.AddIfStatement("tfaRequest.Enable == true", i =>
                        {
                            i.AddIfStatement("tfaRequest.ResetSharedKey", iIf => iIf.AddStatement("throw new Exception(CreateValidationProblem(\"RequiresTwoFactor\", \"No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.\"));"))
                            .AddElseIfStatement("string.IsNullOrEmpty(tfaRequest.TwoFactorCode)", ieIf => ieIf.AddStatement("throw new Exception(CreateValidationProblem(\"RequiresTwoFactor\",\"No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.\"));"))
                            .AddElseIfStatement("!await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode)", ieIf => ieIf.AddStatement("throw new Exception(CreateValidationProblem(\"InvalidTwoFactorCode\",\"The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.\"));"));
                            i.AddStatement("await _userManager.SetTwoFactorEnabledAsync(user, true);");
                        })
                        .AddElseIfStatement("tfaRequest.Enable == false || tfaRequest.ResetSharedKey", e =>
                        {
                            e.AddStatement("await _userManager.SetTwoFactorEnabledAsync(user, false);");
                        });

                        m.AddIfStatement("tfaRequest.ResetSharedKey", i => i.AddStatement("await _userManager.ResetAuthenticatorKeyAsync(user);"));

                        m.AddStatement("string[]? recoveryCodes = null;");

                        m.AddIfStatement("tfaRequest.ResetRecoveryCodes || (tfaRequest.Enable == true && await _userManager.CountRecoveryCodesAsync(user) == 0)", i =>
                        {
                            i.AddStatement("var recoveryCodesEnumerable = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);");
                            i.AddStatement("recoveryCodes = recoveryCodesEnumerable?.ToArray();");
                        });

                        m.AddIfStatement("tfaRequest.ForgetMachine", i => i.AddStatement("await _signInManager.ForgetTwoFactorClientAsync();"));

                        m.AddStatement("var key = await _userManager.GetAuthenticatorKeyAsync(user);");

                        m.AddIfStatement("string.IsNullOrEmpty(key)", i =>
                        {
                            i.AddStatement("await _userManager.ResetAuthenticatorKeyAsync(user);");
                            i.AddStatement("key = await _userManager.GetAuthenticatorKeyAsync(user);");

                            i.AddIfStatement("string.IsNullOrEmpty(key)", iIf => iIf.AddStatement("throw new NotSupportedException(\"The user manager must produce an authenticator key after reset.\");"));
                        });

                        m.AddObjectInitializerBlock("var response = new TwoFactorResponseDto", c =>
                        {
                            c.AddInitStatement("SharedKey ", "key");
                            c.AddInitStatement("RecoveryCodes", "recoveryCodes.ToList()");
                            c.AddInitStatement("RecoveryCodesLeft", "recoveryCodes?.Length ?? await _userManager.CountRecoveryCodesAsync(user)");
                            c.AddInitStatement("IsTwoFactorEnabled ", "await _userManager.GetTwoFactorEnabledAsync(user)");
                            c.AddInitStatement("IsMachineRemembered ", "await _signInManager.IsTwoFactorClientRememberedAsync(user)");
                            c.WithSemicolon();
                        });

                        m.AddReturn("response");
                    });

                    @class.AddMethod("InfoResponseDto", "CreateInfoResponse", c =>
                    {
                        c.Private()
                        .Static()
                        .Async();

                        c.AddGenericParameter("TUser", out var tUser)
                            .AddGenericTypeConstraint(tUser, c => c
                            .AddType("class"));
                        c.AddParameter("TUser", "user");
                        c.AddParameter("UserManager<TUser>", "userManager");

                        c.AddReturn("new ()" +
                            "{" +
                            "   Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException(\"Users must have an email.\")," +
                            "   IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user)" +
                            "}");
                    });

                    @class.AddMethod("string", "CreateValidationProblem", c =>
                    {
                        c.Private()
                        .Static();

                        c.AddParameter("IdentityResult", "result");

                        c.AddStatement("Debug.Assert(!result.Succeeded);");
                        c.AddStatement("var errorDictionary = new Dictionary<string, string[]>(1);");

                        c.AddForEachStatement("error", "result.Errors", f =>
                        {
                            f.AddStatement("string[] newDescriptions;");

                            f.AddIfStatement("errorDictionary.TryGetValue(error.Code, out var descriptions)", i =>
                            {
                                i.AddStatement("newDescriptions = new string[descriptions.Length + 1];");
                                i.AddStatement("Array.Copy(descriptions, newDescriptions, descriptions.Length);");
                                i.AddStatement("newDescriptions[descriptions.Length] = error.Description;");
                            }).AddElseStatement(e =>
                            {
                                e.AddStatement("newDescriptions = [error.Description];");
                            });

                            f.AddStatement("errorDictionary[error.Code] = newDescriptions;");
                        });

                        c.AddReturn("string.Join(\"; \", errorDictionary.Select(kvp => $\"{kvp.Key}={string.Join(\",\", kvp.Value)}\"))");
                    });

                    @class.AddMethod("string", "CreateValidationProblem", c =>
                    {
                        c.Private()
                        .Static();

                        c.AddParameter("string", "errorCode");
                        c.AddParameter("string", "errorDescription");

                        c.AddReturn("$\"{errorCode}={errorDescription}\"");
                    });

                    @class.AddMethod("Task", "SendConfirmationEmailAsync", c =>
                    {
                        c.Private()
                        .Async();

                        c.AddParameter($"{this.GetIdentityUserClass()}", "user");
                        c.AddParameter($"UserManager<{this.GetIdentityUserClass()}>", "userManager");
                        c.AddParameter("HttpContext ", "context");
                        c.AddParameter("string", "email");
                        c.AddParameter("bool", "isChange = false");

                        c.AddIfStatement("confirmEmailEndpointName is null", i => i.AddStatement("throw new NotSupportedException(\"No email confirmation endpoint was registered!\");"));

                        c.AddStatement("var code = isChange ? await userManager.GenerateChangeEmailTokenAsync(user, email) : await userManager.GenerateEmailConfirmationTokenAsync(user);");
                        c.AddStatement("code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));");

                        c.AddStatement("var userId = await userManager.GetUserIdAsync(user);");

                        c.AddObjectInitializerBlock("var routeValues = new RouteValueDictionary()", block =>
                        {
                            block.AddInitStatement("[\"userId\"]", "userId");
                            block.AddInitStatement("[\"code\"]", "code");
                            block.WithSemicolon();
                        });

                        c.AddIfStatement("isChange", i => i.AddStatement("routeValues.Add(\"changedEmail\", email);"));

                        c.AddStatement("var confirmEmailUrl = _linkGenerator.GetUriByName(context, confirmEmailEndpointName, routeValues) ?? throw new NotSupportedException($\"Could not find endpoint named '{confirmEmailEndpointName}'.\");");
                        c.AddStatement("await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            if (!CanRunTemplate()) return;
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this).ForInterface(GetTemplate<IdentityServiceManagerInterfaceTemplate>(IdentityServiceManagerInterfaceTemplate.TemplateId)));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
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