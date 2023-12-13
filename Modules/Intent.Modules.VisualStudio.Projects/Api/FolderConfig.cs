using System;
using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public class FolderConfig : IOutputTargetConfig
    {
        public static class MetadataKey
        {
            internal const string IsMatch = "Intent.Modules.VisualStudio.Projects.FolderConfig.IsMatch";
            internal const string Model = "Intent.Modules.VisualStudio.Projects.FolderConfig.Model";
        }

        private readonly SolutionFolderModel _model;

        public FolderConfig(SolutionFolderModel model)
        {
            _model = model;
            Metadata = new Dictionary<string, object>
            {
                [MetadataKey.Model] = model,
                [MetadataKey.IsMatch] = true
            };
        }

        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => "Folder";
        public string Name => _model.Name;
        public string RelativeLocation => "";
        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks => Array.Empty<string>();
        public IEnumerable<IOutputTargetRole> Roles => _model.Roles;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs.DetectDuplicates();
        public IDictionary<string, object> Metadata { get; }
    }
}
