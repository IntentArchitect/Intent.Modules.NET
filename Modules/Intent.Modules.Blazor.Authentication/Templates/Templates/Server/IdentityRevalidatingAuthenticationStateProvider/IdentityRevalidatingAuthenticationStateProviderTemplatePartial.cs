using Intent.Engine;
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
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRevalidatingAuthenticationStateProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityRevalidatingAuthenticationStateProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.IdentityRevalidatingAuthenticationStateProviderTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityRevalidatingAuthenticationStateProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
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
                .AddClass($"IdentityRevalidatingAuthenticationStateProvider", @class =>
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

                        ctor.AddParameter("IOptions<IdentityOptions>", "optionsAccessor");

                        ctor.AddStatement("options = optionsAccessor.Value;");
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
                            method.AddStatement($"var userManager = scope.ServiceProvider.GetRequiredService<UserManager<{GetTypeName(ApplicationUserTemplate.TemplateId)}>>();");
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
            return base.CanRunTemplate() && ExecutionContext.GetSettings().GetGroup("489a67db-31b2-4d51-96d7-52637c3795be").GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa").Value == "interactive-server";
        }
    }
}