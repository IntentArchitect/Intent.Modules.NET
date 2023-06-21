using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.CosmosDB.CosmosDBRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("CosmosRepository = Microsoft.Azure.CosmosRepository")
                .AddClass($"{Model.Name}CosmosDBRepository", @class =>
                {
                    var pkAttribute = Model.Attributes.Single(x => x.HasPrimaryKey());
                    var pkPropertyName = pkAttribute.Name.ToPascalCase();
                    var pkFieldName = pkAttribute.Name.ToCamelCase();
                    var entityDocumentName = this.GetCosmosDBDocumentName();

                    @class.Internal();
                    @class.ExtendsClass($"{this.GetCosmosDBRepositoryBaseName()}<{EntityInterfaceName}, {EntityStateName}, {entityDocumentName}>");
                    @class.ImplementsInterface(this.GetEntityRepositoryInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetCosmosDBUnitOfWorkName(), "unitOfWork");
                        ctor.AddParameter(UseType($"Microsoft.Azure.CosmosRepository.IRepository<{entityDocumentName}>"), "cosmosRepository");
                        ctor.AddParameter(UseType("AutoMapper.IMapper"), "mapper");
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("unitOfWork")
                            .AddArgument("cosmosRepository")
                            .AddArgument("mapper")
                            .AddArgument($"\"{pkFieldName}\"")
                        );
                    });

                    @class.AddMethod("void", "EnsureHasId", m => m
                        .Protected()
                        .Override()
                        .AddParameter(EntityInterfaceName, "entity")
                        .AddStatement($"entity.{pkPropertyName} ??= Guid.NewGuid().ToString();")
                    );
                });
        }

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);
        public string EntityStateName => GetTypeName("Domain.Entity", Model);

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