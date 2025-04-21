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
using Intent.SqlDatabaseProject.Api;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.SqlDatabaseProject.Templates.Schema
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class SchemaTemplateRegistration : FilePerModelTemplateRegistration<SchemaModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SchemaTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => SchemaTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, SchemaModel model)
        {
            return new SchemaTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<SchemaModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application)
                .Elements
                .Select(s =>
                {
                    var schema = s.FindSchema();
                    return schema is null ? new SchemaModel("dbo") : new SchemaModel(schema);
                })
                .Where(p => p.Name != "dbo")
                .DistinctBy(x => x.Name)
                .ToArray();
        }
    }
}