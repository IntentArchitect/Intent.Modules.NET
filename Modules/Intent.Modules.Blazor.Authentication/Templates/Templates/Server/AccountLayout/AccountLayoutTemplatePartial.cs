using System;
using Intent.Engine;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.AccountLayout
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AccountLayoutTemplate : RazorTemplateBase<object>, IRazorFileTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.AccountLayoutTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="AccountLayoutTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccountLayoutTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            RazorFile = IRazorFile.Create(this, $"AccountLayout")
                .Configure(file =>
                {
                    file.AddInheritsDirective("LayoutComponentBase");
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement($"@layout {outputTarget.ApplicationName()}.Client.Layout.MainLayout"), file));
                    file.AddInjectDirective("Microsoft.AspNetCore.Components.NavigationManager", "NavigationManager");
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement($"@if(HttpContext is null)"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("{"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("<p>Loading...</p>"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("}"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("else"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("{"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("@Body"), file));
                    file.AddEmptyLine();
                    file.AddChildNode(IRazorCodeDirective.Create(new CSharpStatement("}"), file));
                    file.AddEmptyLine();

                    file.AddCodeBlock(code =>
                    {
                        code.AddProperty("HttpContext?", "HttpContext", httpContext =>
                        {
                            httpContext.Private();
                            httpContext.AddAttribute("CascadingParameter");
                        });

                        code.AddMethod("void", "OnParametersSet", onParametersSet =>
                        {
                            onParametersSet.Protected().Override();

                            onParametersSet.AddIfStatement("HttpContext is null", @if =>
                            {
                                @if.AddStatement("NavigationManager.Refresh(forceReload: true);");
                            });
                        });
                    });
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public IRazorFile RazorFile { get; }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return RazorFile.GetConfig();
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();
    }
}