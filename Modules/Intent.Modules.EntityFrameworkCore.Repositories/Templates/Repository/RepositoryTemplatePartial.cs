using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Keys.Settings;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.Repositories.Repository";
        private ITemplateDependency _entityStateTemplateDependency;
        private ITemplateDependency _entityInterfaceTemplateDependency;
        private ITemplateDependency _repositoryInterfaceTemplateDependency;
        private ITemplateDependency _dbContextTemplateDependency;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryTemplate(IOutputTarget outputTarget, ClassModel model)
            : base(TemplateId, outputTarget, model)
        {
        }

        public override void OnCreated()
        {
            base.OnCreated();
            _entityStateTemplateDependency = TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == Model.Id);
            _entityInterfaceTemplateDependency = TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Interface Template Id"], (to) => to.Id == Model.Id);
            _repositoryInterfaceTemplateDependency = TemplateDependency.OnModel(EntityRepositoryInterfaceTemplate.Identifier, Model);
            _dbContextTemplateDependency = TemplateDependency.OnTemplate(DbContextTemplate.TemplateId);
        }

        public string EntityInterfaceName
        {
            get
            {
                var template = Project.FindTemplateInstance<IClassProvider>(_entityInterfaceTemplateDependency);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string EntityName
        {
            get
            {
                var template = Project.FindTemplateInstance<IClassProvider>(_entityStateTemplateDependency);
                return template != null ? NormalizeNamespace($"{template.Namespace}.{template.ClassName}") : $"{Model.Name}";
            }
        }

        public string RepositoryContractName => Project.FindTemplateInstance<IClassProvider>(_repositoryInterfaceTemplateDependency)?.ClassName ?? $"I{ClassName}";

        public string DbContextName => Project.FindTemplateInstance<IClassProvider>(_dbContextTemplateDependency)?.ClassName ?? $"{Model.Application.Name}DbContext";

        public string PrimaryKeyType => Model.Attributes.Any(x => x.HasStereotype("Primary Key")) ? GetTypeName(Model.Attributes.First(x => x.HasStereotype("Primary Key")).Type) : UseType(ExecutionContext.Settings.GetEntityKeySettings()?.KeyType().Value ?? "System.Guid");

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Name.ToPascalCase() ?? "Id";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Repository",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                _entityStateTemplateDependency,
                _entityInterfaceTemplateDependency,
                _repositoryInterfaceTemplateDependency,
                _dbContextTemplateDependency,
            };
        }

        public override void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(_repositoryInterfaceTemplateDependency);
            if (contractTemplate == null)
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
            //Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            //{
            //    { "InterfaceType", $"{contractTemplate.Namespace}.{contractTemplate.ClassName}"},
            //    { "ConcreteType", $"{Namespace}.{ClassName}" },
            //    { "InterfaceTypeTemplateId", _repositoryInterfaceTemplateDependency.TemplateId },
            //    { "ConcreteTypeTemplateId", Identifier }
            //});
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
            return !(ExecutionContext.Settings.GetEntityFrameworkCoreSettings().InheritanceStrategy().IsTablePerConcreteType() &&
                   Model.IsAbstract && OutputTarget.GetProject().TargetDotNetFrameworks.First().Major <= 6);
        }
    }
}
