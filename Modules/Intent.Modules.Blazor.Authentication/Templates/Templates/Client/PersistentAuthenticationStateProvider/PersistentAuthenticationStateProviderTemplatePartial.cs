using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Templates.Templates.Client.UserInfo;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Client.PersistentAuthenticationStateProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PersistentAuthenticationStateProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Client.PersistentAuthenticationStateProviderTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PersistentAuthenticationStateProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.Components.Authorization")
                .AddUsing("System.Security.Claims")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.AspNetCore.Components.WebAssembly.Authentication")
                .AddUsing("System")
                .AddClass($"PersistentAuthenticationStateProvider", @class =>
                {
                    @class.WithBaseType("AuthenticationStateProvider");
                    @class.ImplementsInterface("IAccessTokenProvider");
                    @class.AddField("Task<AuthenticationState>", "defaultUnauthenticatedTask", f =>
                    {
                        f.PrivateReadOnly().Static();
                        f.WithAssignment(new CSharpStatement("Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))"));
                    });
                    @class.AddField("Task<AuthenticationState>", "authenticationStateTask", f =>
                    {
                        f.PrivateReadOnly();
                        f.WithAssignment(new CSharpStatement("defaultUnauthenticatedTask"));
                    });
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("PersistentComponentState", "state");

                        ctor.AddIfStatement($"!state.TryTakeFromJson<{GetTypeName(UserInfoTemplate.TemplateId)}>(nameof(UserInfo), out var userInfo) || userInfo is null", @if => @if.AddReturn(""));

                        ctor.AddStatements(@"Claim[] claims = [
                            new Claim(ClaimTypes.NameIdentifier, userInfo.UserId),
                            new Claim(ClaimTypes.Email, userInfo.Email),
                            new Claim(ClaimTypes.Email, userInfo.Email),
                            new Claim(""access_token"", userInfo.AccessToken == null ? """" : userInfo.AccessToken) ];".ConvertToStatements());

                        ctor.AddStatements(@"authenticationStateTask = Task.FromResult(
                            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                                authenticationType: nameof(PersistentAuthenticationStateProvider)))));".ConvertToStatements());
                    });

                    @class.AddMethod("Task<AuthenticationState>", "GetAuthenticationStateAsync", method => method.Override().AddReturn("authenticationStateTask"));

                    @class.AddMethod("ValueTask<AccessTokenResult>", "RequestAccessToken", method =>
                    {
                        method.Async().WithReturnType("ValueTask<AccessTokenResult>");
                        method.AddAssignmentStatement("var state", new CSharpStatement("await this.GetAuthenticationStateAsync();"));
                        method.AddAssignmentStatement("var token", new CSharpStatement("state.User.FindFirst(\"access_token\");"));

                        method.AddIfStatement($"token == null", @if => @if.AddReturn("new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, \"auth/login\", null)"));

                        method.AddAssignmentStatement("var accessToken", new CSharpStatement("new AccessToken { Expires = DateTimeOffset.MaxValue, Value = token.Value };"));

                        method.AddAssignmentStatement("var result", new CSharpStatement("new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null, null);"));

                        method.AddReturn("result");
                    });

                    @class.AddMethod("ValueTask<AccessTokenResult>", "RequestAccessToken", method =>
                    {
                        method.Async().WithReturnType("ValueTask<AccessTokenResult>");

                        method.AddParameter("AccessTokenRequestOptions", "options");

                        method.AddReturn("await RequestAccessToken()");
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
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer();
        }
    }
}