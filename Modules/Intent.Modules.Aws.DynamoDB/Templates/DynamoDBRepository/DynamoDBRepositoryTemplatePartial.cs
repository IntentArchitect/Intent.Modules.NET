using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

            FulfillsRole(TemplateRoles.Repository.Implementation.Entity);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}DynamoDBRepository", @class =>
                {
                    // this will force a load of the Domain.Entities type which in turn means 
                    // entityDocumentName resolves with the full namespace if conflicts occur
                    var entityTypeName = EntityTypeName;

                    var genericTypeParameters = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;
                    var entityDocumentName = $"{this.GetDynamoDBDocumentName()}{genericTypeParameters}";

                    @class.Internal();
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    // Base class generic type arguments:
                    {
                        var primaryKeys = Model.GetPrimaryKeyData();
                        if (primaryKeys == null)
                        {
                            throw new Exception("Could not get primary key data");
                        }

                        var baseClassTypeArgs = new List<string>
                        {
                            entityTypeName
                        };

                        if (createEntityInterfaces)
                        {
                            baseClassTypeArgs.Add(EntityStateTypeName);
                        }

                        baseClassTypeArgs.Add(entityDocumentName);

                        baseClassTypeArgs.Add(GetTypeName(primaryKeys.PartitionKeyAttribute));

                        baseClassTypeArgs.Add(primaryKeys.SortKeyAttribute != null
                            ? GetTypeName(primaryKeys.SortKeyAttribute)
                            : "object");

                        @class.ExtendsClass($"{this.GetDynamoDBRepositoryBaseName()}<{string.Join(", ", baseClassTypeArgs)}>");
                    }

                    @class.ImplementsInterface($"{this.GetEntityRepositoryInterfaceName()}{genericTypeParameters}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(UseType("Amazon.DynamoDBv2.DataModel.IDynamoDBContext"), "context");
                        ctor.AddParameter(this.GetDynamoDBUnitOfWorkName(), "unitOfWork");
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("context")
                            .AddArgument("unitOfWork")
                        );
                    });
                });
        }

        internal string GetPkType(AttributeModelExtensionMethods.PrimaryKeyData pkAttribute)
        {
            return pkAttribute.SortKeyAttribute != null
                ? $"({GetTypeName(pkAttribute.PartitionKeyAttribute)} {pkAttribute.PartitionKeyAttribute.Name.ToPascalCase()}, {GetTypeName(pkAttribute.SortKeyAttribute)} {pkAttribute.SortKeyAttribute.Name.ToPascalCase()})"
                : GetTypeName(pkAttribute.PartitionKeyAttribute);
        }

        public string GenericTypeParameters => Model.GenericTypes.Any()
            ? $"<{string.Join(", ", Model.GenericTypes)}>"
            : string.Empty;

        public string EntityTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{GenericTypeParameters}";
        public string EntityStateTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Primary, Model)}{GenericTypeParameters}";

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model, accessibleTo: null);
            if (contractTemplate == null)
            {
                return;
            }

            ((ICSharpFileBuilderTemplate)contractTemplate).CSharpFile.Interfaces[0].AddMetadata("requires-explicit-update", true);
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate)
                .WithPerServiceCallLifeTime()
            );
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