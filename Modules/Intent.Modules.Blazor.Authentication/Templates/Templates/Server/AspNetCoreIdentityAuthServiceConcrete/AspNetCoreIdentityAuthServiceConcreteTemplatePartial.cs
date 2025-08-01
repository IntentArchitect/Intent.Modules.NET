using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AuthServiceInterface;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AspNetCoreIdentityAuthServiceConcrete
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AspNetCoreIdentityAuthServiceConcreteTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.AspNetCoreIdentityAuthServiceConcreteTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AspNetCoreIdentityAuthServiceConcreteTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.WebUtilities")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Threading")
                .AddUsing("System.Text")
                .AddUsing("System.Text.Encodings.Web")
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddClass($"AspNetCoreIdentityAuthServiceConcrete", @class =>
                {
                    @class.Internal();
                    @class.ImplementsInterface(GetTypeName(AuthServiceInterfaceTemplate.TemplateId));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"SignInManager<{GetTypeName(ApplicationUserTemplate.TemplateId)}>", "signInManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"UserManager<{GetTypeName(ApplicationUserTemplate.TemplateId)}>", "userManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"IUserStore<{GetTypeName(ApplicationUserTemplate.TemplateId)}>", "userStore", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"NavigationManager", "navigationManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"{GetTypeName(IdentityRedirectManagerTemplate.TemplateId)}", "redirectManager", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter($"IEmailSender<{GetTypeName(ApplicationUserTemplate.TemplateId)}>", "emailSender", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task<string>", "ConfirmEmail", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "userId");
                        method.AddParameter("string", "code");

                        method.AddAssignmentStatement("var user", new CSharpStatement("await _userManager.FindByIdAsync(userId);"));

                        method.AddIfStatement("user is null", @if =>
                        {
                            @if.AddReturn("$\"Error loading user with ID {userId}\"");
                        }).AddElseStatement(@else =>
                        {
                            @else.AddAssignmentStatement("var result", new CSharpStatement("await _userManager.ConfirmEmailAsync(user, code);"));
                            @else.AddReturn("result.Succeeded ? \"Thank you for confirming your email.\" : \"Error confirming your email.\"");
                        });
                    });

                    @class.AddMethod("Task", "ForgotPassword", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");

                        method.AddAssignmentStatement("var user", new CSharpStatement("await _userManager.FindByEmailAsync(email);"));

                        method.AddIfStatement("user is null || !(await _userManager.IsEmailConfirmedAsync(user))", @if =>
                        {
                            @if.AddReturn("");
                        });

                        method.AddAssignmentStatement("var code", new CSharpStatement("await _userManager.GeneratePasswordResetTokenAsync(user);"));
                        method.AddAssignmentStatement("code", new CSharpStatement("WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));"));
                        method.AddAssignmentStatement("var callbackUrl", new CSharpStatement("_navigationManager.GetUriWithQueryParameters(" +
                            "_navigationManager.ToAbsoluteUri(\"Account/ResetPassword\").AbsoluteUri," +
                            "new Dictionary<string, object?> { [\"code\"] = code });"));

                        method.AddStatement("await _emailSender.SendPasswordResetLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));");
                    });

                    @class.AddMethod("Task", "Login", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "username");
                        method.AddParameter("string", "password");
                        method.AddParameter("bool", "rememberMe");
                        method.AddParameter("string", "returnUrl");

                        method.AddAssignmentStatement("var result", new CSharpStatement("await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);"));

                        method.AddIfStatement("result.Succeeded", @if =>
                        {
                            @if.AddStatement("_redirectManager.RedirectTo(returnUrl);");
                        }).AddElseIfStatement("result.RequiresTwoFactor", eIf =>
                        {
                            eIf.AddStatement("_redirectManager.RedirectTo(\"Account/LoginWith2fa\", new() { [\"returnUrl\"] = returnUrl, [\"rememberMe\"] = rememberMe });");
                        }).AddElseIfStatement("result.IsLockedOut", eIf =>
                        {
                            eIf.AddStatement("_redirectManager.RedirectTo(\"Account/Lockout\");");
                        }).AddElseStatement(@else =>
                        {
                            @else.AddStatement("throw new Exception(\"Error: Invalid login attempt.\");");
                        });
                    });

                    @class.AddMethod("Task", "Register", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "password");
                        method.AddParameter("string", "returnUrl");

                        method.AddAssignmentStatement("var user", new CSharpStatement("CreateUser();"));

                        method.AddStatement("await _userStore.SetUserNameAsync(user, email, CancellationToken.None);");
                        method.AddAssignmentStatement("var emailStore", new CSharpStatement("GetEmailStore();"));
                        method.AddStatement("await emailStore.SetEmailAsync(user, email, CancellationToken.None);");
                        method.AddAssignmentStatement("var result", new CSharpStatement("await _userManager.CreateAsync(user, password);"));

                        method.AddIfStatement("!result.Succeeded", @if =>
                        {
                            @if.AddStatement("throw new Exception(\"Could not register user\");");
                        });

                        method.AddAssignmentStatement("var userId", new CSharpStatement("await _userManager.GetUserIdAsync(user);"));
                        method.AddAssignmentStatement("var code", new CSharpStatement("await _userManager.GenerateEmailConfirmationTokenAsync(user);"));
                        method.AddAssignmentStatement("code", new CSharpStatement("WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));"));
                        method.AddAssignmentStatement("var callbackUrl", new CSharpStatement("_navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri(\"Account/ConfirmEmail\").AbsoluteUri,new Dictionary<string, object?> { [\"userId\"] = userId, [\"code\"] = code, [\"returnUrl\"] = returnUrl });"));

                        method.AddStatement("await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));");

                        method.AddIfStatement("_userManager.Options.SignIn.RequireConfirmedAccount", @if =>
                        {
                            @if.AddStatement("_redirectManager.RedirectTo(\"Account/RegisterConfirmation\", new() { [\"email\"] = email, [\"returnUrl\"] = returnUrl });");
                        }).AddElseStatement(@else =>
                        {
                            @else.AddStatement("await _signInManager.SignInAsync(user, isPersistent: false);");
                            @else.AddStatement("_redirectManager.RedirectTo(returnUrl);");
                        });
                    });

                    @class.AddMethod(GetTypeName(ApplicationUserTemplate.TemplateId), "CreateUser", method =>
                    {
                        method.Private();
                        method.AddTryBlock(@try =>
                        {
                            @try.AddReturn($"Activator.CreateInstance<{GetTypeName(ApplicationUserTemplate.TemplateId)}>();");
                        }).AddCatchBlock(@catch =>
                        {
                            @catch.AddStatement($"throw new InvalidOperationException($\"Can't create an instance of '{{nameof({GetTypeName(ApplicationUserTemplate.TemplateId)})}}'. \" + $\"Ensure that '{{nameof({GetTypeName(ApplicationUserTemplate.TemplateId)})}}' is not an abstract class and has a parameterless constructor.\");");
                        });
                    });

                    @class.AddMethod($"IUserEmailStore<{GetTypeName(ApplicationUserTemplate.TemplateId)}>", "GetEmailStore", method =>
                    {
                        method.Private();
                        method.AddIfStatement("!_userManager.SupportsUserEmail", @if =>
                        {
                            @if.AddStatement("throw new NotSupportedException(\"The default UI requires a user store with email support.\");");
                        });

                        method.AddReturn($"(IUserEmailStore<{GetTypeName(ApplicationUserTemplate.TemplateId)}>)_userStore");
                    });

                    @class.AddMethod("Task", "ResendEmailConfirmation", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");

                        method.AddAssignmentStatement("var user", new CSharpStatement("await _userManager.FindByEmailAsync(email);"));

                        method.AddIfStatement("user is null", @if =>
                        {
                            @if.AddReturn("");
                        });

                        method.AddAssignmentStatement("var userId", new CSharpStatement("await _userManager.GetUserIdAsync(user);"));
                        method.AddAssignmentStatement("var code", new CSharpStatement("await _userManager.GenerateEmailConfirmationTokenAsync(user);"));
                        method.AddAssignmentStatement("code", new CSharpStatement("WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));"));
                        method.AddAssignmentStatement("var callbackUrl", new CSharpStatement("_navigationManager.GetUriWithQueryParameters(_navigationManager.ToAbsoluteUri(\"Account/ConfirmEmail\").AbsoluteUri,new Dictionary<string, object?> { [\"userId\"] = userId, [\"code\"] = code });"));

                        method.AddStatement("await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));");
                    });

                    @class.AddMethod("Task", "ResetPassword", method =>
                    {
                        method.Async();
                        method.AddParameter("string", "email");
                        method.AddParameter("string", "code");
                        method.AddParameter("string", "password");

                        method.AddAssignmentStatement("var user", new CSharpStatement("await _userManager.FindByEmailAsync(email);"));

                        method.AddIfStatement("user is null", @if =>
                        {
                            @if.AddStatement("_redirectManager.RedirectTo(\"Account/ResetPasswordConfirmation\");");
                        });

                        method.AddAssignmentStatement("var result", new CSharpStatement("await _userManager.ResetPasswordAsync(user, code, password);"));

                        method.AddIfStatement("result.Succeeded", @if =>
                        {
                            @if.AddStatement("_redirectManager.RedirectTo(\"Account/ResetPasswordConfirmation\");");
                        });
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

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity();
        }
    }
}