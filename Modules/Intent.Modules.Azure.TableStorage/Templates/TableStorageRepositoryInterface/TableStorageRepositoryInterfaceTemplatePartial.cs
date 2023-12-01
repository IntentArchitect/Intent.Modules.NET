using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntityInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageRepositoryInterfaceTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageRepositoryInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"ITableStorageRepository", @interface => @interface
                    .AddGenericParameter("TDomain", out var tDomain)
                    .AddGenericParameter("TTableInterface", out var tTableInterface)
                    .ImplementsInterfaces(new[] { $"{this.GetRepositoryInterfaceName()}<{tDomain}>" })
                    .AddProperty(this.GetTableStorageUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                        .WithoutSetter()
                    )
                    .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                    )
                    .AddMethod($"Task<{tDomain}?>", "FindByIdAsync", method => method
                        .AddParameter("(string partitionKey, string rowKey)", "id")
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                    )
                    .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                        .AddParameter($"Expression<Func<{tTableInterface}, bool>>", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                    )
                );
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            foreach (var model in Model)
            {
                var template = GetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, model.Id);
                template.CSharpFile.AfterBuild(file =>
                {
                    var @interface = file.Interfaces.Single();

                    @interface.Interfaces.Clear();
                    var tDomainGenericArgument = template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model);
                    var tDocumentInterfaceGenericArgument = template.GetTypeName(TableStorageTableEntityInterfaceTemplate.TemplateId, model);
                    @interface.ImplementsInterfaces($"{this.GetTableStorageRepositoryInterfaceName()}<{tDomainGenericArgument}, {tDocumentInterfaceGenericArgument}>");

                }, 1000);
            }
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