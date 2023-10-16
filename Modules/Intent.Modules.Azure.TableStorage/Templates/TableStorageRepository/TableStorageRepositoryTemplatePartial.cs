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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}TableStorageRepository", @class =>
                {
                    @class.Internal();
                    @class.ExtendsClass($"{this.GetTableStorageRepositoryBaseName()}<{EntityInterfaceName}, {EntityStateName}, {this.GetTableStorageTableEntityName()}>");
                    @class.ImplementsInterface($"{this.GetEntityRepositoryInterfaceName()}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetTableStorageUnitOfWorkName(), "unitOfWork");
                        ctor.AddParameter(this.UseType("Azure.Data.Tables.TableServiceClient"), "tableServiceClient");
                        ctor.AddParameter("string", "tableName", p => p.WithDefaultValue($"nameof({EntityStateName})"));
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("unitOfWork")
                            .AddArgument("tableServiceClient")
                            .AddArgument("tableName")
                        );
                    });
                });
        }

        public string GenericTypeParameters => Model.GenericTypes.Any()
    ? $"<{string.Join(", ", Model.GenericTypes)}>"
    : string.Empty;

        public string EntityInterfaceName => $"{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model)}{GenericTypeParameters}";
        public string EntityStateName => $"{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, Model)}{GenericTypeParameters}";

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