using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}TableStorageRepository", @class =>
                {
                    @class.Internal();

                    var entityStateGenericTypeArgument = createEntityInterfaces
                        ? $", {EntityStateTypeName}"
                        : string.Empty;


                    @class.ExtendsClass($"{this.GetTableStorageRepositoryBaseName()}<{EntityTypeName}{entityStateGenericTypeArgument}, {this.GetTableStorageTableEntityName()}, {this.GetTableStorageTableEntityInterfaceName()}>");
                    @class.ImplementsInterface($"{this.GetEntityRepositoryInterfaceName()}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetTableStorageUnitOfWorkName(), "unitOfWork");
                        ctor.AddParameter(this.UseType("Azure.Data.Tables.TableServiceClient"), "tableServiceClient");
                        ctor.AddParameter("string", "tableName", p => p.WithDefaultValue($"nameof({EntityStateTypeName})"));
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("unitOfWork")
                            .AddArgument("tableServiceClient")
                            .AddArgument("tableName")
                        );
                    });
                });
        }

        //Because this is a Role its adaptive it will be the Entity.Interface or the Entity.Primary
        public string EntityTypeName => GetTypeName(TemplateRoles.Domain.Entity.Interface, Model);
        public string EntityStateTypeName => GetTypeName(TemplateRoles.Domain.Entity.Primary, Model);

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