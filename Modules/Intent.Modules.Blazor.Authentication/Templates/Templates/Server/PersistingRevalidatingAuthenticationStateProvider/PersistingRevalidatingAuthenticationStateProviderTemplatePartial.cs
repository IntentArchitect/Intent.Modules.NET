using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.FactoryExtensions;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.UserInfo;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ApplicationUser;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.PersistingRevalidatingAuthenticationStateProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PersistingRevalidatingAuthenticationStateProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.PersistingRevalidatingAuthenticationStateProviderTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PersistingRevalidatingAuthenticationStateProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.Components.Authorization")
                .AddUsing("Microsoft.AspNetCore.Components.Server")
                .AddUsing("Microsoft.AspNetCore.Components.Web")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Logging")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Threading")
                .AddUsing("System.Security.Claims")
                .AddUsing("System")
                .AddUsing("System.Diagnostics")
                .AddClass($"PersistingRevalidatingAuthenticationStateProvider", @class =>
                {
                    @class.WithBaseType("RevalidatingServerAuthenticationStateProvider");
                    @class.AddField("IdentityOptions", "options", f => f.PrivateReadOnly());
                    @class.AddField("PersistingComponentStateSubscription", "subscription", f => f.PrivateReadOnly());
                    @class.AddField("Task<AuthenticationState>?", "authenticationStateTask");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ILoggerFactory", "loggerFactory");
                        ctor.CallsBase(b => b.AddArgument("loggerFactory"));
                        ctor.AddParameter("IServiceScopeFactory", "serviceScopeFactory", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("PersistentComponentState", "persistentComponentState", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IOptions<IdentityOptions>", "optionsAccessor");

                        ctor.AddStatement("options = optionsAccessor.Value;");
                        ctor.AddStatement("AuthenticationStateChanged += OnAuthenticationStateChanged;");
                        ctor.AddStatement("subscription = _persistentComponentState.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);");
                    });

                    @class.AddProperty("TimeSpan", "RevalidationInterval", p => p.Override().Protected().WithoutSetter().Getter.WithExpressionImplementation("TimeSpan.FromMinutes(30)"));

                    @class.AddMethod("Task<bool>", "ValidateAuthenticationStateAsync", method =>
                    {
                        method.Async().Protected().Override();
                        method.AddParameter("AuthenticationState", "authenticationState");
                        method.AddParameter("CancellationToken", "cancellationToken");

                        if (ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                        {
                            method.AddStatement("await using var scope = _serviceScopeFactory.CreateAsyncScope();");
                            var identityUserName = string.Empty;
                            if (ExecutionContext.InstalledModules.Any(im => im.ModuleId == "Intent.AspNetCore.Identity"))
                            {
                                identityUserName = IdentityHelperExtensions.GetIdentityUserClass(this);
                            }
                            else
                            {
                                identityUserName = GetTypeName(ApplicationUserTemplate.TemplateId);
                            }
                            method.AddStatement($"var userManager = scope.ServiceProvider.GetRequiredService<UserManager<{identityUserName}>>();");
                            method.AddReturn("await ValidateSecurityStampAsync(userManager, authenticationState.User);");
                        }
                        else
                        {
                            method.AddReturn("await Task.FromResult(authenticationState.User.Identity.IsAuthenticated);");
                        }
                    });

                    if (ExecutionContext.GetSettings().GetBlazor().Authentication().IsAspnetcoreIdentity())
                    {
                        @class.AddMethod("Task<bool>", "ValidateSecurityStampAsync", method =>
                        {
                            method.Async().Private();
                            method.AddParameter("UserManager<ApplicationUser>", "userManager");
                            method.AddParameter("ClaimsPrincipal", "principal");

                            method.AddStatement("var user = await userManager.GetUserAsync(principal);");
                            method.AddIfStatement("user is null", @if =>
                            {
                                @if.AddReturn("false");
                            })
                            .AddElseIfStatement("!userManager.SupportsUserSecurityStamp", @elseif =>
                            {
                                @elseif.AddReturn("true");
                            }).AddElseStatement(@else =>
                            {
                                @else.AddStatement("var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);");
                                @else.AddStatement("var userStamp = await userManager.GetSecurityStampAsync(user);");
                                @else.AddReturn("principalStamp == userStamp");
                            });
                        });
                    }

                    @class.AddMethod("void", "OnAuthenticationStateChanged", method =>
                    {
                        method.Private();
                        method.AddParameter("Task<AuthenticationState>", "task");
                        method.AddStatement("authenticationStateTask = task;");
                    });

                    @class.AddMethod("Task", "OnPersistingAsync", method =>
                    {
                        method.Async().Private();

                        method.AddIfStatement("authenticationStateTask is null", @if =>
                        {
                            @if.AddStatement("throw new UnreachableException($\"Authentication state not set in {nameof(OnPersistingAsync)}().\");");
                        });

                        method.AddStatement("var authenticationState = await authenticationStateTask;");
                        method.AddStatement("var principal = authenticationState.User;");

                        method.AddIfStatement("principal.Identity?.IsAuthenticated == true", @if =>
                        {
                            @if.AddStatement("var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;");
                            @if.AddStatement("var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;");
                            @if.AddStatement("var accessToken = principal.FindFirst(\"access_token\")?.Value;");

                            @if.AddIfStatement("userId != null && email != null", @iif =>
                            {
                                @iif.AddStatement("var userInfo = new UserInfo {UserId = userId, Email = email, AccessToken = accessToken};");
                                @iif.AddStatement($"_persistentComponentState.PersistAsJson(nameof({GetTypeName(UserInfoTemplate.TemplateId)}), userInfo);");
                            });
                        });
                    });

                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.Protected().Override();
                        method.AddParameter("bool", "disposing");
                        method.AddStatement("subscription.Dispose();");
                        method.AddStatement("AuthenticationStateChanged -= OnAuthenticationStateChanged;");
                        method.AddStatement("base.Dispose(disposing);");
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
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetGroup("489a67db-31b2-4d51-96d7-52637c3795be").GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa").Value == "interactive-auto";
        }
    }
}