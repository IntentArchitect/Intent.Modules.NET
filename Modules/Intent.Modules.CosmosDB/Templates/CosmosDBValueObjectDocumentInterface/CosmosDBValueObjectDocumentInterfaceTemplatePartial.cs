using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocumentInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBValueObjectDocumentInterfaceTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBValueObjectDocumentInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBValueObjectDocumentInterfaceTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("System.Collections.Generic.IReadOnlyList<{0}>"));
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name}Document", @interface =>
                {
                    var attributes = Model.ChildElements
                        .Where(x => x.IsAttributeModel())
                        .Select(x => x.AsAttributeModel())
                        .ToArray();

                    foreach (var attribute in attributes)
                    {
                        @interface.AddProperty(GetTypeName(attribute.TypeReference), attribute.Name.ToPascalCase(), p => p.WithoutSetter());
                    }
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