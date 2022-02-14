using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Templates.DomainEntityState;
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
        public const string Identifier = "Intent.EntityFrameworkCore.DbContextInterface";
        private readonly IList<EntityTypeConfigurationCreatedEvent> _entityTypeConfigurations = new List<EntityTypeConfigurationCreatedEvent>();

        [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
        public DbContextInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(Identifier, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.EntityFrameworkCore(Project));
            ExecutionContext.EventDispatcher.Subscribe<EntityTypeConfigurationCreatedEvent>(evt =>
            {
                _entityTypeConfigurations.Add(evt);
            });
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName(DomainEntityStateTemplate.TemplateId, model);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IApplicationDbContext",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DbContextInterface";
    }
}
