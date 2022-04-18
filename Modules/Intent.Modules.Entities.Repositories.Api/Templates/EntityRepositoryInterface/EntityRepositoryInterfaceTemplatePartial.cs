using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class EntityRepositoryInterfaceTemplate : CSharpTemplateBase<ClassModel>
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.EntityRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityRepositoryInterfaceTemplate(IOutputTarget outputTarget, ClassModel model)
            : base(TemplateId, outputTarget, model)
        {
        }

        public string RepositoryInterfaceName => GetTypeName(RepositoryInterfaceTemplate.TemplateId);

        public string EntityStateName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public string PrimaryKeyType => GetTemplate<ITemplate>("Domain.Entity", Model).GetMetadata().CustomMetadata.TryGetValue("Surrogate Key Type", out var type) ? UseType(type) : UseType("System.Guid");

        public string PrimaryKeyName => Model.Attributes.FirstOrDefault(x => x.HasStereotype("Primary Key"))?.Name.ToPascalCase() ?? "Id";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}Repository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}
