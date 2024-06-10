using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates.Repository
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class RepositoryTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RepositoryTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RepositoryTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new RepositoryTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels()
                .Where(x => (x.InternalElement.Package.AsDomainPackageModel()?.HasRelationalDatabase() == true &&
                             x.IsAggregateRoot() && (!x.IsAbstract || x.HasTable())) || HasRepository(x))
                .ToArray();
        }

        public bool HasRepository(ClassModel model)
        {
            return model.HasStereotype("cdc3d50d-0edd-4ffd-996f-08cbdf39595c");
        }

    }
}