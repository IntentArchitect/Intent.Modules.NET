using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.Identity.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates.CurrentUserInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CurrentUserInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Identity.CurrentUserInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CurrentUserInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Security.Claims")
                .AddInterface($"ICurrentUser", @interface =>
                {
                    string userIdType = ExecutionContext.Settings.GetIdentitySettings().UserIdType().ToCSharpType();

                    @interface.AddProperty(this.UseType(userIdType) + "?", "Id", p => p.ReadOnly());
                    @interface.AddProperty($"string?", "Name", p => p.ReadOnly());
                    @interface.AddProperty($"ClaimsPrincipal", "Principal", p => p.ReadOnly());
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