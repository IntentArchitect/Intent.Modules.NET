#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.StaticContentTemplateRegistrations
{
    public class ServiceFabricStaticStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        public new const string TemplateId = "Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.StaticContentTemplateRegistrations.ServiceFabricStaticStaticContentTemplateRegistration";

        public ServiceFabricStaticStaticContentTemplateRegistration(IMetadataManager metadataManager) : base(TemplateId)
        {
            _metadataManager = metadataManager;
        }

        public override string ContentSubFolder => "ServiceFabricStaticContent";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };

        private IReadOnlyCollection<IOutputTarget>? _outputTargets;

        protected override void RegisterTemplate(ITemplateInstanceRegistry registry, IApplication application, Func<IOutputTarget, ITemplate> createTemplateInstance)
        {
            _outputTargets ??= _metadataManager.VisualStudio(application).GetServiceFabricProjectModels()
                .Select(x => application.OutputTargets.Single(y => y.Id == x.Id))
                .ToArray();

            foreach (var outputTarget in _outputTargets)
            {
                registry.RegisterTemplate(TemplateId, outputTarget, createTemplateInstance);
            }
        }

        protected override ITemplateFileConfig UpdateTemplateFileConfig(ITemplateFileConfig fileConfig, StaticContentTemplate template)
        {
            fileConfig.CustomMetadata["ItemType"] = "None";

            return base.UpdateTemplateFileConfig(fileConfig, template);
        }
    }
}