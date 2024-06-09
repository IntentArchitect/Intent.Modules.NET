using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Api.Mappings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
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
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public class RazorComponentTemplate : RazorTemplateBase<ComponentModel>, IRazorComponentTemplate
    {
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
            //AddTypeSource("Intent.Blazor.HttpClients.PagedResult");
            AddTypeSource("Blazor.HttpClient.Contracts.Dto");
            AddTypeSource("Blazor.HttpClient.ServiceContract");
            AddTypeSource(TemplateId);
            RazorFile = new RazorFile(this);
            BindingManager = new BindingManager(this, Model.InternalElement.Mappings.FirstOrDefault());
            ComponentBuilderProvider = DefaultRazorComponentBuilderProvider.Create(this);// new RazorComponentBuilderProvider(this);

            RazorFile.Configure(file =>
            {
                if (Model.HasPage())
                {
                    RazorFile.AddPageDirective(Model.GetPage().Route());
                    if (!string.IsNullOrWhiteSpace(Model.GetPage().Title()))
                    {
                        RazorFile.AddHtmlElement("PageTitle", x => x.WithText(Model.GetPage().Title()));
                    }
                }

                ComponentBuilderProvider.ResolveFor(Model.View.InternalElement).BuildComponent(Model.View.InternalElement, RazorFile);

                var block = GetCodeBlock();
                block.AddCodeBlockMembers(this, Model.InternalElement);
                if (Model.HasPage())
                {
                    foreach (var declaration in block.Declarations)
                    {
                        if (declaration is CSharpProperty property && new RouteManager(Model.GetPage().Route()).HasParameterExpression(property.Name)
                            && property.Attributes.All(x => x.Name != "Parameter"))
                        {
                            property.AddAttribute("Parameter");
                        }
                    }
                }
            });
        }

        public BindingManager BindingManager { get; }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public RazorFile RazorFile { get; }

        public IRazorComponentBuilderProvider ComponentBuilderProvider { get; }

        public void AddInjectDirective(string fullyQualifiedTypeName, string propertyName = null)
        {
            RazorFile.AddInjectDirective(fullyQualifiedTypeName, propertyName);
        }

        public CSharpClassMappingManager CreateMappingManager()
        {
            var mappingManager = new CSharpClassMappingManager(this);
            mappingManager.AddMappingResolver(new CallServiceOperationMappingResolver(this));
            mappingManager.AddMappingResolver(new RazorBindingMappingResolver(this));
            mappingManager.SetFromReplacement(Model, null);
            mappingManager.SetToReplacement(Model, null);
            return mappingManager;
        }

        private IBuildsCSharpMembers _codeBlock;
        public IBuildsCSharpMembers GetCodeBlock()
        {
            if (_codeBlock == null)
            {
                RazorFile.AddCodeBlock(x => _codeBlock = x);
            }
            return _codeBlock;
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            return new RazorFileConfig(
                className: $"{Model.Name}",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath()
            );
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public override string TransformText() => RazorFile.ToString();
    }
}