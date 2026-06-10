using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.ForgotPasswordCodeBehind
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ForgotPasswordCodeBehindTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.ForgotPasswordCodeBehindTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ForgotPasswordCodeBehindTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // Emits a partial class that pairs (by name + namespace + folder) with the
            // ForgotPassword.razor component. ForgotPasswordTemplate contributes the members
            // via its GetCodeBehind(), so the page's C# lives here instead of an inline @code block.
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .WithFileExtension("razor.cs")
                .IntentManagedMerge()
                .AddClass($"ForgotPassword", @class =>
                {
                    @class.Partial();
                });
        }

        public override ICSharpCodeContext RootCodeContext => CSharpFile.Classes.Single();

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

        // Generate the code-behind only when the matching ForgotPassword page is generated
        // (Identity + JWT; not OIDC), keeping this paired with ForgotPasswordTemplate.
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && !ExecutionContext.GetSettings().GetBlazor().Authentication().IsOidc();
        }
    }
}
