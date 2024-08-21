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

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.ApplicationIdentityUserConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationIdentityUserConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Identity.AccountController.ApplicationIdentityUserConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationIdentityUserConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ApplicationIdentityUserConfiguration", @class =>
                {
                    @class.WithBaseType($"{UseType("Microsoft.EntityFrameworkCore.IEntityTypeConfiguration")}<{this.GetApplicationIdentityUserName()}>");
                    @class.AddMethod("void", "Configure", method =>
                    {
                        method.AddParameter($"{UseType("Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder")}<{this.GetApplicationIdentityUserName()}>", "builder");
                        method.AddStatement($"builder.Property(x => x.RefreshToken);");
                        method.AddStatement($"builder.Property(x => x.RefreshTokenExpired);");
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