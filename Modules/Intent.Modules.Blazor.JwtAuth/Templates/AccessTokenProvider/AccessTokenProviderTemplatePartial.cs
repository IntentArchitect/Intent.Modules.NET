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

namespace Intent.Modules.Blazor.JwtAuth.Templates.AccessTokenProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AccessTokenProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.JwtAuth.AccessTokenProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccessTokenProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"AccessTokenProvider", @class =>
                {
                    @class.Internal();
                    @class.WithBaseType(UseType("Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider"));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetAuthServiceInterfaceName(), "authService", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("AccessTokenResult", "RequestAccessToken", method =>
                    {
                        method.Async(true);
                        method.AddStatements(@"
        var token = await _authService.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, ""auth/login"");
        }
        var accessToken = new AccessToken
        {
            Expires = DateTimeOffset.MaxValue,
            Value = token
        };

        var result = new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null);

        return result;".ConvertToStatements());
                    });

                    @class.AddMethod("AccessTokenResult", "RequestAccessToken", method =>
                    {
                        method.Async(true);
                        method.AddParameter("AccessTokenRequestOptions", "options");
                        method.AddStatement("return await RequestAccessToken();");
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