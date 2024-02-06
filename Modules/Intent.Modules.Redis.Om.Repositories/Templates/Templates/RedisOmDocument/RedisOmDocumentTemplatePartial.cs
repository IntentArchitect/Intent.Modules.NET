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
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmValueObjectDocument;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocument
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmDocumentTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmDocument";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmDocumentTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateId);
            AddTypeSource(RedisOmValueObjectDocumentTemplate.TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.ValueObject);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Document", @class =>
                {
                    @class.Internal();

                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (Model.ParentClass != null)
                    {
                        var genericTypeArguments = Model.ParentClass.GenericTypes.Any()
                            ? $"<{string.Join(", ", Model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName))}>"
                            : string.Empty;

                        @class.WithBaseType($"{this.GetRedisOmDocumentName(Model.ParentClass)}{genericTypeArguments}");
                    }

                    {
                        var genericTypeArguments = Model.GenericTypes.Any()
                            ? $"<{string.Join(", ", Model.GenericTypes)}>"
                            : string.Empty;
                        @class.ImplementsInterface($"{this.GetRedisOmDocumentInterfaceName()}{genericTypeArguments}");
                    }
                })
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var entityPropertyIds = EntityStateFileBuilder.CSharpFile.Classes.First().Properties
                        .Select(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel
                                ? metadataModel.Id
                                : null)
                        .Where(x => x != null)
                        .ToHashSet();

                    var attributes = Model.Attributes
                        .Where(x => entityPropertyIds.Contains(x.Id))
                        .ToList();
                    var associationEnds = Model.AssociatedClasses
                        .Where(x => entityPropertyIds.Contains(x.Id) && x.IsNavigable)
                        .ToList();

                    // if (Model.IsAggregateRoot())
                    // {
                    //     AddPropertiesForAggregate(@class);
                    // }
                    // else
                    // {
                    //     this.AddCosmosDBDocumentProperties(
                    //         @class: @class,
                    //         attributes: attributes,
                    //         associationEnds: associationEnds,
                    //         documentInterfaceTemplateId: CosmosDBDocumentInterfaceTemplate.TemplateId);
                    // }
                    //
                    // var pk = Model.GetPrimaryKeyAttribute();
                    // Model.TryGetPartitionAttribute(out var partitionAttribute);
                    // this.AddCosmosDBMappingMethods(
                    //     @class: @class,
                    //     attributes: attributes,
                    //     associationEnds: associationEnds,
                    //     partitionKeyAttribute: partitionAttribute,
                    //     entityInterfaceTypeName: EntityTypeName,
                    //     entityImplementationTypeName: EntityStateTypeName,
                    //     entityRequiresReflectionConstruction: Model.Constructors.Any() &&
                    //                                           Model.Constructors.All(x => x.Parameters.Count != 0),
                    //     entityRequiresReflectionPropertySetting: ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters(),
                    //     isAggregate: Model.IsAggregateRoot(),
                    //     hasBaseType: Model.ParentClass != null
                    //     );
                }, 1000);
        }

        public ICSharpFileBuilderTemplate EntityStateFileBuilder => GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model);

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