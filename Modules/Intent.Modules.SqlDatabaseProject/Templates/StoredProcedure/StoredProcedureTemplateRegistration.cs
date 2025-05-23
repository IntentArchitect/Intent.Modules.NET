using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.SqlDatabaseProject.Api;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.SqlDatabaseProject.Templates.StoredProcedure
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class StoredProcedureTemplateRegistration : FilePerModelTemplateRegistration<SqlStoredProcedureModel>
    {
        private readonly IMetadataManager _metadataManager;

        public StoredProcedureTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => StoredProcedureTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, SqlStoredProcedureModel model)
        {
            return new StoredProcedureTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<SqlStoredProcedureModel> GetModels(IApplication application)
        {
            var domain = _metadataManager.Domain(application);

            return domain.GetElementsOfType(StoredProcedureModel.SpecializationTypeId)
                .Select(s => s.AsStoredProcedureModel())
                .Select(storedProc => new SqlStoredProcedureModel(storedProc))
                .ToList();
        }
    }
}