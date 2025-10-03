using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using ComponentModel = Intent.Modelers.UI.Api.ComponentModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent
{
    using Intent.Blazor.Api;

    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public class RazorComponentTemplate : RazorComponentTemplateBase<ComponentModel>
    {
        public const string SecuredStereotypeId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorComponentTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorComponentTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(ModelDefinitionTemplate.TemplateId);
            AddTypeSource("Blazor.HttpClient.Contracts.Dto");
            AddTypeSource("Blazor.HttpClient.ServiceContract");
            AddTypeSource("Intent.Application.Dtos.DtoModel");
            AddTypeSource(TemplateId);

            RazorFile = IRazorFile.Create(this, $"{Model.Name}")
                .Configure(file =>
                {
                    if (Model.HasPage())
                    {
                        file.AddPageDirective(Model.GetPage().Route());
                        if (!string.IsNullOrWhiteSpace(Model.GetPage().Title()))
                        {
                            file.AddHtmlElement("PageTitle", x => x.WithText(Model.GetPage().Title()));
                        }
                    }

                    if (Model.HasSecured())
                    {
                        foreach (var secure in Model.GetSecureds())
                        {
                            file.AddAttributeDirective(secure.AuthorizationAttribute(this));
                        }
                    }

                    var block = GetCodeBehind();
                    block.AddCodeBlockMembers(this, Model.InternalElement);

                    file.AfterBuild(_ =>
                    {
                        ComponentBuilderProvider.BuildComponent(Model.View.InternalElement, file);
                    });


                    if (Model.HasPage())
                    {
                        foreach (var declaration in block.Declarations)
                        {
                            if (declaration is CSharpProperty property && new RouteManager(Model.GetPage().Route()).HasParameterExpression(property.Name)
                                && property.Attributes.All(x => x.Name != "Parameter" && !x.Name.EndsWith(".Parameter")))
                            {
                                property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Parameter"));
                            }
                        }
                    }
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public sealed override IRazorFile RazorFile { get; }

        protected override string CodeBehindTemplateId => RazorComponentCodeBehindTemplate.TemplateId;

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