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

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.TokenServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class TokenServiceInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.TokenServiceInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TokenServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Security.Claims")
                .AddUsing("System")
                .AddInterface("ITokenService", inter =>
                {
                    inter.AddMethod("string", "GenerateAccessToken", method =>
                    {
                        method.AddParameter("string", "username");
                        method.AddParameter("IEnumerable<Claim>", "claims");
                    });
                    inter.AddMethod("(string Token, DateTime Expiry)", "GenerateRefreshToken");
                    inter.AddMethod("ClaimsPrincipal", "GetPrincipalFromExpiredToken", method =>
                    {
                        method.AddParameter("string", "token");
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