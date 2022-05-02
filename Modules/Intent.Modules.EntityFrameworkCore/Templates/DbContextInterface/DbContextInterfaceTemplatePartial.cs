using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DbContextInterfaceTemplate : CSharpTemplateBase<IList<ClassModel>>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DbContextInterface";
        private readonly IList<EntityTypeConfigurationCreatedEvent> _entityTypeConfigurations = new List<EntityTypeConfigurationCreatedEvent>();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbContextInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.EntityFrameworkCore(Project));
            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(evt =>
            {
                _entityTypeConfigurations.Add(evt);
            });
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName("Domain.Entity", model);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IApplicationDbContext",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
