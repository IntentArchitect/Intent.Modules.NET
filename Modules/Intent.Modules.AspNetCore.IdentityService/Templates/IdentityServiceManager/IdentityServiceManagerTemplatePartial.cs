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
                .AddUsing("Microsoft.AspNetCore.WebUtilities")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.AspNetCore.Authentication.BearerToken")
                .AddUsing("System.Text.Encodings.Web")
                .AddUsing("System.Text")
                .AddClass($"IdentityServiceManager", @class =>
                {
                    @class.AddGenericParameter("TUser", out var tUser)
                    .AddGenericTypeConstraint(tUser, c => c
                            .AddType("class")
                            .AddType("new()"));

                    var interfaceTypeName = GetTypeName("Intent.AspNetCore.IdentityService.IdentityServiceManagerInterface");
                    @class.ImplementsInterface($"{interfaceTypeName}<TUser>");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("UserManager<TUser>", "userManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("SignInManager<TUser>", "signInManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("UserStore<TUser>", "userStore", param =>
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
                        ctor.AddParameter("IEmailSender<TUser>", "emailSender", param =>
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
                    });

                    @class.AddMethod("string", "ConfirmEmailAsync", m =>
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

                    @class.AddMethod("Task", "ForgotPasswordAsync", m =>
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

                    @class.AddMethod("InfoResponseDto", "GetInfoAsync", m =>
                    {
                        m.Async();

                        m.AddIfStatement("await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User) is not { } user", c => c.AddStatement("throw new NotFoundException();"));

                        m.AddReturn("await CreateInfoResponseAsync(user, _userManager);");
                    });

                    @class.AddMethod("Task", "LoginAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("LoginRequestDto", "login");
                        m.AddParameter("bool?", "useCookies");
                        m.AddParameter("bool?", "useSessionCookies");

                        m.AddStatement("var useCookieScheme = (useCookies == true) || (useSessionCookies == true);");
                        m.AddStatement("var isPersistent = (useCookies == true) && (useSessionCookies != true);");
                        m.AddStatement("_signInManager.AuthenticationScheme = useCookieScheme ? IdentityConstants.ApplicationScheme : IdentityConstants.BearerScheme;");

                        m.AddStatement("var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, lockoutOnFailure: true);");

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
                    });

                    @class.AddMethod("ClaimsPrincipal", "RefreshAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("RefreshRequestDto", "refreshRequest");

                        m.AddStatement("var refreshTokenProtector = _bearerTokenOptions.Get(IdentityConstants.BearerScheme).RefreshTokenProtector;");
                        m.AddStatement("var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);");

                        m.AddIfStatement("refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc || _timeProvider.GetUtcNow() >= expiresUtc || await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not TUser user", i =>
                        {
                            i.AddStatement("throw new ChallangeException();");
                        });

                        m.AddReturn("await _signInManager.CreateUserPrincipalAsync(user);");
                    });

                    @class.AddMethod("Task", "RegisterAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("RegisterRequestDto", "registration");

                        m.AddIfStatement("!_userManager.SupportsUserEmail", i => i.AddStatement("throw new NotSupportedException($\"{nameof(IdentityServiceManager<TUser>)} requires a user store with email support.\");"));

                        m.AddStatement("var emailStore = (IUserEmailStore<TUser>)_userStore;");
                        m.AddStatement("var email = registration.Email;");

                        m.AddStatement("var user = new TUser();");
                        m.AddStatement("await _userStore.SetUserNameAsync(user, email, CancellationToken.None);");
                        m.AddStatement("await emailStore.SetEmailAsync(user, email, CancellationToken.None);");
                        m.AddStatement("var result = await _userManager.CreateAsync(user, registration.Password);");

                        m.AddIfStatement("!result.Succeeded", i => i.AddStatement("throw new Exception();"));

                        m.AddStatement("await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, email);");
                    });

                    @class.AddMethod("bool", "ResendConfirmationEmailAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("ResendConfirmationEmailRequestDto", "resendRequest");

                        m.AddIfStatement("await _userManager.FindByEmailAsync(resendRequest.Email) is not { } user", i => i.AddReturn("false"));

                        m.AddStatement("await SendConfirmationEmailAsync(user, _userManager, _httpContextAccessor.HttpContext, resendRequest.Email);");
                        m.AddReturn("true");
                    });

                    @class.AddMethod("Task", "ResetPasswordAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("ResetPasswordRequestDto", "resetRequest");

                        m.AddStatement("var user = await _userManager.FindByEmailAsync(resetRequest.Email);");

                        m.AddIfStatement("user is null || !(await _userManager.IsEmailConfirmedAsync(user))", i => i.AddStatement("throw new Exception(IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken()));"));

                        m.AddStatement("IdentityResult result;");

                        m.AddTryBlock(t =>
                        {
                            t.AddStatement("var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));");
                            t.AddStatement("result = await _userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);");
                        }).AddCatchBlock(c =>
                        {
                            c.AddStatement("result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());");
                        });

                        m.AddIfStatement("!result.Succeeded", i => i.AddStatement("throw new Exception(result);"));
                    });

                    @class.AddMethod("InfoResponseDto", "UpdateInfoAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("InfoRequestDto", "infoRequest");

                        m.AddIfStatement("await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User) is not { } user", c => c.AddStatement("throw new NotFoundException();"));

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

                        m.AddReturn("await CreateInfoResponseAsync(user, _userManager);");
                    });

                    @class.AddMethod("TwoFactorResponseDto", "UpdateTwoFactorAsync", m =>
                    {
                        m.Async();
                        m.AddParameter("TwoFactorRequestDto", "tfaRequest");

                        m.AddIfStatement("await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User) is not { } user", c => c.AddStatement("throw new NotFoundException();"));

                        m.AddIfStatement("tfaRequest.Enable == true", i =>
                        {
                            i.AddIfStatement("tfaRequest.ResetSharedKey", iIf => iIf.AddStatement("CreateValidationProblem(\"RequiresTwoFactor\", \"No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.\");"))
                            .AddElseIfStatement("string.IsNullOrEmpty(tfaRequest.TwoFactorCode)", ieIf => ieIf.AddStatement("return CreateValidationProblem(\"RequiresTwoFactor\",\"No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa.\");"))
                            .AddElseIfStatement("!await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, tfaRequest.TwoFactorCode)", ieIf => ieIf.AddStatement("return CreateValidationProblem(\"InvalidTwoFactorCode\",\"The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa.\");"));
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
                            c.AddInitStatement("RecoveryCodes", "recoveryCodes");
                            c.AddInitStatement("RecoveryCodesLeft", "recoveryCodes?.Length ?? await _userManager.CountRecoveryCodesAsync(user)");
                            c.AddInitStatement("IsTwoFactorEnabled ", "await _userManager.GetTwoFactorEnabledAsync(user)");
                            c.AddInitStatement("IsMachineRemembered ", "await _signInManager.IsTwoFactorClientRememberedAsync(user)");
                            c.WithSemicolon();
                        });

                        m.AddReturn("response");
                    });

                    @class.AddMethod("InfoResponseDto", "CreateInfoResponseAsync", c =>
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

                    @class.AddMethod("Dictionary<string, string[]>", "CreateValidationProblem", c =>
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

                        c.AddReturn("errorDictionary;");
                    });

                    @class.AddMethod("Dictionary<string, string[]>", "CreateValidationProblem", c =>
                    {
                        c.Private()
                        .Static();

                        c.AddParameter("string", "errorCode");
                        c.AddParameter("string", "errorDescription");

                        c.AddStatement("var errorDictionary = new Dictionary<string, string[]>(1);");

                        c.AddStatement("errorDictionary.Add(errorCode, [errorDescription]);");

                        c.AddReturn("errorDictionary;");
                    });

                    @class.AddMethod("Task", "SendConfirmationEmailAsync", c =>
                    {
                        c.Private()
                        .Async();
                       
                        c.AddParameter("TUser", "user");
                        c.AddParameter("UserManager<TUser>", "userManager");
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