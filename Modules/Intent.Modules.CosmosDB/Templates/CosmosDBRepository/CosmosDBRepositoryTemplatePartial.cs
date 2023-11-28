using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}CosmosDBRepository", @class =>
                {
                    var pkAttribute = Model.GetPrimaryKeyAttribute();
                    var pkFieldName = pkAttribute.Name.ToCamelCase();
                    var genericTypeParameters = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;
                    var entityDocumentName = $"{this.GetCosmosDBDocumentName()}{genericTypeParameters}";
                    var entityDocumentInterfaceName = $"{this.GetCosmosDBDocumentInterfaceName()}{genericTypeParameters}";

                    @class.Internal();
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    var entityStateGenericTypeArgument = createEntityInterfaces
                        ? $", {EntityStateTypeName}"
                        : string.Empty;
                    @class.ExtendsClass($"{this.GetCosmosDBRepositoryBaseName()}<{EntityTypeName}{entityStateGenericTypeArgument}, {entityDocumentName}, {entityDocumentInterfaceName}>");
                    @class.ImplementsInterface($"{this.GetEntityRepositoryInterfaceName()}{genericTypeParameters}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetCosmosDBUnitOfWorkName(), "unitOfWork");
                        ctor.AddParameter(UseType($"Microsoft.Azure.CosmosRepository.IRepository<{entityDocumentName}>"), "cosmosRepository");
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("unitOfWork")
                            .AddArgument("cosmosRepository")
                            .AddArgument($"\"{pkFieldName}\"")
                        );
                    });

                    if (pkAttribute.TypeReference?.Element.Name != "string")
                    {
                        @class.AddMethod($"{UseType("System.Threading.Tasks.Task")}<{EntityStateTypeName}>", "FindByIdAsync", method =>
                        {
                            method
                                .Async()
                                .AddParameter(GetTypeName(pkAttribute), "id")
                                .AddOptionalCancellationTokenParameter(this)
                                .WithExpressionBody($"await FindByIdAsync(id{pkAttribute.GetToString(this)}, cancellationToken)");
                        });

                        @class.AddMethod($"{UseType("System.Threading.Tasks.Task")}<{UseType("System.Collections.Generic.List")}<{EntityStateTypeName}>>", "FindByIdsAsync", method =>
                        {
                            AddUsing("System.Linq");
                            method
                                .Async()
                                .AddParameter($"{GetTypeName(pkAttribute)}[]", "ids")
                                .AddOptionalCancellationTokenParameter(this)
                                .WithExpressionBody($"await FindByIdsAsync(ids.Select(id => id{pkAttribute.GetToString(this)}).ToArray(), cancellationToken)");
                        });
                    }
                });
        }

        public string GenericTypeParameters => Model.GenericTypes.Any()
            ? $"<{string.Join(", ", Model.GenericTypes)}>"
            : string.Empty;

        public string EntityTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{GenericTypeParameters}";
        public string EntityStateTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Primary, Model)}{GenericTypeParameters}";

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
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