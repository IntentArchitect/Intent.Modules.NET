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

namespace Intent.Modules.Blazor.Templates.Templates.Common.UserMenuCodeBehind
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class UserMenuCodeBehindTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Common.UserMenuCodeBehindTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public UserMenuCodeBehindTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            // The code-behind for the UserMenu.razor markup that ships as static content (from this module's
            // ThemeToggle content, or MudBlazor's Theme content). It is generated rather than shipped alongside
            // that markup so that the Roslyn Weaver owns it, and the application's C# style settings - Namespace
            // Declaration Style in particular - apply to it like every other generated .cs file. A hand-written
            // .cs file shipped as static content is emitted verbatim and never sees the weaver.
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .WithFileExtension("razor.cs")
                .IntentManagedMerge()
                .AddClass("UserMenu", @class =>
                {
                    @class.Partial();

                    @class.AddProperty("string", "Title", prop =>
                    {
                        prop.AddAttribute(UseType("Microsoft.AspNetCore.Components.Parameter"));
                        prop.WithInitialValue(@"""Account menu""");
                    });

                    @class.AddProperty($"{UseType("Microsoft.AspNetCore.Components.RenderFragment")}?", "Trigger", prop =>
                    {
                        prop.AddAttribute(UseType("Microsoft.AspNetCore.Components.Parameter"));
                        prop.WithComments("/// <summary>The trigger content shown in the closed menu (e.g. an icon).</summary>");
                    });

                    @class.AddProperty($"{UseType("Microsoft.AspNetCore.Components.RenderFragment")}?", "ChildContent", prop =>
                    {
                        prop.AddAttribute(UseType("Microsoft.AspNetCore.Components.Parameter"));
                        prop.WithComments("/// <summary>The menu items rendered inside the dropdown panel.</summary>");
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
