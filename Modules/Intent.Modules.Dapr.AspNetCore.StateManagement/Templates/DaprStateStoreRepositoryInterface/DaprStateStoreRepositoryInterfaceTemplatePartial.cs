using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprStateStoreRepositoryInterfaceTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprStateStoreRepositoryInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IDaprStateStoreRepository", @interface => @interface
                    .AddGenericParameter("TDomain", out var tDomain)
                    .AddGenericParameter("TPersistence")
                    .AddGenericParameter("TIdentifier", out var tIdentifier)
                    .ImplementsInterfaces(new[] { $"{this.GetRepositoryInterfaceName()}<{tDomain}>" })
                    .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                    )
                    .AddMethod($"Task<{tDomain}>", "FindByIdAsync", method => method
                        .AddParameter(tIdentifier, "id")
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                    )
                    .AddProperty(this.GetDaprStateStoreUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                        .WithoutSetter()
                    )
                );
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            foreach (var model in Model)
            {
                var template = GetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, model.Id);
                template.CSharpFile.OnBuild(file =>
                {
                    var primaryKeyAttribute = model.Attributes.Single(x => x.HasPrimaryKey());
                    var primaryKeyTypeName = template.GetTypeName((IElement)primaryKeyAttribute.TypeReference.Element);
                    var @interface = file.Interfaces.Single();

                    @interface.Interfaces[0] = @interface.Interfaces.Single()
                        .Replace("IRepository", this.GetDaprStateStoreRepositoryInterfaceName())
                        .Replace(">", $", {primaryKeyTypeName}>");
                });
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