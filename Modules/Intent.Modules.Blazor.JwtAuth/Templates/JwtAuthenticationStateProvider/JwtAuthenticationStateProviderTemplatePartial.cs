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

namespace Intent.Modules.Blazor.JwtAuth.Templates.JwtAuthenticationStateProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JwtAuthenticationStateProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.JwtAuth.JwtAuthenticationStateProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public JwtAuthenticationStateProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"JwtAuthenticationStateProvider", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider"));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("Microsoft.JSInterop.IJSRuntime"), "jsRuntime", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("AuthenticationState", "GetAuthenticationStateAsync", method =>
                    {
                        method.Override().Async();
                        method.AddStatements(@"
        var token = await _jsRuntime.InvokeAsync<string>(""localStorage.getItem"", ""authToken"");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), ""jwt"");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }".ConvertToStatements());
                    });

                    @class.AddMethod("void", "NotifyUserAuthentication", method =>
                    {
                        method.AddParameter("string", "token");
                        method.AddStatements(@"
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), ""jwt"");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));".ConvertToStatements());
                    });

                    @class.AddMethod("void", "NotifyUserLogout", method =>
                    {
                        method.AddStatements(@"
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));".ConvertToStatements());
                    });

                    @class.AddMethod("IEnumerable<Claim>", "ParseClaimsFromJwt", method =>
                    {
                        method.Private();
                        method.AddParameter("string", "jwt");
                        method.AddStatements(@"
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(FixBase64String(payload));
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));".ConvertToStatements());
                    });

                    @class.AddMethod("string", "FixBase64String", method =>
                    {
                        method.Private();
                        method.AddParameter("string", "base64");
                        method.AddStatements(@"
        switch (base64.Length % 4)
        {
            case 2: return base64 + ""=="";
            case 3: return base64 + ""="";
            default: return base64;
        }".ConvertToStatements());
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