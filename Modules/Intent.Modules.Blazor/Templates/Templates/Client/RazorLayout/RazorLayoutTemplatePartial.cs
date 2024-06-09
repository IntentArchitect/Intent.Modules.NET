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
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayout
{
    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public class RazorLayoutTemplate : RazorTemplateBase<LayoutModel>, IRazorComponentTemplate
    {
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorLayoutTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorLayoutTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public RazorLayoutTemplate(IOutputTarget outputTarget, LayoutModel model) : base(TemplateId, outputTarget, model)
        {
            RazorFile = new RazorFile(this);
            BindingManager = new BindingManager(this, Model.InternalElement.Mappings.FirstOrDefault());
            ComponentBuilderProvider = DefaultRazorComponentBuilderProvider.Create(this);

            RazorFile.Configure(file =>
            {
                file.AddInheritsDirective("LayoutComponentBase");

                ComponentBuilderProvider.ResolveFor(Model.InternalElement)
                    .BuildComponent(Model.InternalElement, RazorFile);

                if (file.ChildNodes.All(x => x is not HtmlElement))
                {
                    file.AddHtmlElement("div", div => div.WithText("@Body"));
                }

                var block = GetCodeBlock();
                block.AddCodeBlockMembers(this, Model.InternalElement);
            });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Fully)]
        public RazorFile RazorFile { get; }

        public IRazorComponentBuilderProvider ComponentBuilderProvider { get; }

        public BindingManager BindingManager { get; }

        private IBuildsCSharpMembers _codeBlock;
        public IBuildsCSharpMembers GetCodeBlock()
        {
            if (_codeBlock == null)
            {
                RazorFile.AddCodeBlock(x => _codeBlock = x);
            }
            return _codeBlock;
        }

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