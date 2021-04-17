using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Registrations;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class SqlProjectTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public SqlProjectTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => SqlProjectTemplate.TemplateId;


        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetSQLServerDatabaseProjectModels();

            foreach (var model in models)
            {
                var project = application.OutputTargets.Single(x => x.Id == model.Id);
                registry.RegisterTemplate(TemplateId, project, p => new SqlProjectTemplate(p, model));
            }
        }
    }
}