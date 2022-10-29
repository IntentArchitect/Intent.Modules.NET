using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.Repository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryTemplate(IOutputTarget outputTarget, ClassModel model)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddClass($"{Model.Name}Repository", @class =>
                {
                    @class.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                    @class.WithBaseType($"RepositoryBase<{EntityInterfaceName}, {EntityName}, {DbContextName}>");
                    @class.ImplementsInterface(RepositoryContractName);
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(DbContextName, "dbContext")
                            .CallsBase(b => b.AddArgument("dbContext"));
                        if (!string.IsNullOrWhiteSpace(ConstructorImplementation()))
                        {
                            ctor.AddStatement(ConstructorImplementation());
                        }
                    });

                    if (TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, Model, out var entityTemplate))
                    {
                        entityTemplate.CSharpFile.AfterBuild(file =>
                        {
                            var entityClass = file.Classes.First();
                            if (entityClass.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks)
                                && pks.Length == 1)
                            {
                                @class.AddMethod($"Task<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model)}>", "FindByIdAsync", method =>
                                {
                                    var pk = pks.First();
                                    method.Async();
                                    method.AddParameter(entityTemplate.UseType(pk.Type), pk.Name.ToCamelCase());
                                    method.AddParameter("CancellationToken", "cancellationToken",
                                        param => param.WithDefaultValue("default"));
                                    method.AddStatement($"return await FindAsync(x => x.{pk.Name} == {pk.Name.ToCamelCase()}, cancellationToken);");
                                });
                                @class.AddMethod($"Task<List<{GetTypeName(TemplateFulfillingRoles.Domain.Entity.Interface, Model)}>>", "FindByIdsAsync", method =>
                                {
                                    var pk = pks.First();
                                    method.Async();
                                    method.AddParameter($"{entityTemplate.UseType(pk.Type)}[]", pk.Name.ToCamelCase().Pluralize());
                                    method.AddParameter("CancellationToken", "cancellationToken",
                                        param => param.WithDefaultValue("default"));
                                    method.AddStatement($"return await FindAllAsync(x => {pk.Name.ToCamelCase().Pluralize()}.Contains(x.{pk.Name}), cancellationToken);");
                                });
                            }
                        }, 4000);
                    }
                });
        }

        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Repository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public string EntityName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public string RepositoryContractName => TryGetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, Model) ?? $"I{ClassName}";

        public string DbContextName => TryGetTypeName("Infrastructure.Data.DbContext") ?? $"{Model.Application.Name}DbContext";

        public string PrimaryKeyType => GetTemplate<ITemplate>("Domain.Entity", Model).GetMetadata().CustomMetadata.TryGetValue("Surrogate Key Type", out var type) ? UseType(type) : UseType("System.Guid");

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasPrimaryKey())?.Name.ToPascalCase() ?? "Id";

        public override void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
        }

        private string ConstructorImplementation()
        {
            if (!IsRepoSupported())
            {
                return $@"
            // The {Model.Name} type's ORM inheritance strategy is set to Table per Concrete Type (TPC).
            // Table per Concrete Type is not supported in the current version of Entity Framework Core.
            // Because of this, Intent.EntityFrameworkCore module is only creating tables for the concrete types in this hierarchy.
            // A repository on this abstract type is therefore not supported.
            throw new NotSupportedException($""Cannot create a repository for abstract type {Model.Name}."");";
            }

            return string.Empty;
        }

        private bool IsRepoSupported()
        {
            return !(ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC() &&
                   Model.IsAbstract && OutputTarget.GetProject().TargetDotNetFrameworks.First().Major <= 6);
        }

    }
}
