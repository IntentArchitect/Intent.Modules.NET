using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBValueObjectDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBValueObjectDocumentTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBValueObjectDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBValueObjectDocumentTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    var attributes = Model.ChildElements
                        .Where(x => x.IsAttributeModel())
                        .Select(x => x.AsAttributeModel())
                        .ToArray();

                    var valueObjectTypeName = GetTypeName(TemplateRoles.Domain.ValueObject, Model);

                    this.AddDynamoDBDocumentProperties(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: [],
                        entityTypeName: valueObjectTypeName);

                    this.AddDynamoDBMappingMethods(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: [],
                        partitionKeyAttribute: null,
                        entityInterfaceTypeName: valueObjectTypeName,
                        entityImplementationTypeName: valueObjectTypeName,
                        entityRequiresReflectionConstruction: true,
                        entityRequiresReflectionPropertySetting: true,
                        isAggregate: false,
                        hasBaseType: false);
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