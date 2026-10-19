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
using Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocumentInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBValueObjectDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBValueObjectDocumentTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBValueObjectDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBValueObjectDocumentTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    var genericTypeArguments = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;

                    @class.ImplementsInterface($"{this.GetCosmosDBValueObjectDocumentInterfaceName()}{genericTypeArguments}");
                })
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();

                    var attributes = Model.ChildElements
                        .Where(x => x.IsAttributeModel())
                        .Select(x => x.AsAttributeModel())
                        .ToArray();
                    var associationEnds = this.GetValueObjectAssociationEnds(Model);

                    this.AddCosmosDBDocumentProperties(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: associationEnds,
                        documentInterfaceTemplateId: CosmosDBValueObjectDocumentInterfaceTemplate.TemplateId);

                    var valueObjectTypeName = GetTypeName(TemplateRoles.Domain.ValueObject, Model);

                    this.AddCosmosDBMappingMethods(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: associationEnds,
                        partitionKeyAttribute: null,
                        entityInterfaceTypeName: valueObjectTypeName,
                        entityImplementationTypeName: valueObjectTypeName,
                        entityRequiresReflectionConstruction: true,
                        entityRequiresReflectionPropertySetting: true,
                        isAggregate: false,
                        hasBaseType: false);
                }, 1000);
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