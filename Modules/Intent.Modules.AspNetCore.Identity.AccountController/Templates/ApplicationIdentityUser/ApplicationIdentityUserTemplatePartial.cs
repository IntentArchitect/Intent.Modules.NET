using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.ApplicationIdentityUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationIdentityUserTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.ApplicationIdentityUser";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationIdentityUserTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            FulfillsRole("Domain.IdentityUser");
            AddNugetDependency(Intent.Modules.AspNetCore.Identity.NugetPackages.MicrosoftExtensionsIdentityStores(OutputTarget.GetProject()));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ApplicationIdentityUser", @class =>
                {
                    @class.WithBaseType(UseType("Microsoft.AspNetCore.Identity.IdentityUser"));
                    @class.AddProperty(UseType("string?"), "RefreshToken");
                    @class.AddProperty(UseType("System.DateTime?"), "RefreshTokenExpired");
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