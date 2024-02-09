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

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmValueObjectDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmValueObjectDocumentTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmValueObjectDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmValueObjectDocumentTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    var genericTypeArguments = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;

                    @class.ImplementsInterface($"{this.GetRedisOmValueObjectDocumentInterfaceName()}{genericTypeArguments}");

                    var attributes = Model.ChildElements
                        .Where(x => x.IsAttributeModel())
                        .Select(x => x.AsAttributeModel())
                        .ToArray();

                    this.AddRedisOmDocumentProperties(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: Array.Empty<AssociationEndModel>());

                    var valueObjectTypeName = GetTypeName(TemplateRoles.Domain.ValueObject, Model);

                    this.AddRedisOmMappingMethods(
                        @class: @class,
                        attributes: attributes,
                        associationEnds: Array.Empty<AssociationEndModel>(),
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