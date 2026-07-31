using System;
using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.VisualStudio.Projects.OutputTargets;

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
        private readonly OutputLocationOptions _outputLocationOptions;

        public FolderConfig(SolutionFolderModel model) : this(model, outputLocationOptions: null)
        {
        }

        internal FolderConfig(SolutionFolderModel model, OutputLocationOptions outputLocationOptions)
        {
            _model = model;
            _outputLocationOptions = outputLocationOptions ?? OutputLocationOptions.None;
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

        // Unlike a project, a Solution Folder has no "explicit override vs Name fallback" concept -
        // an empty RelativeLocation means "not materialized, no physical folder", which the shift
        // must leave alone; any non-empty value already denotes a real folder that needs shifting.
        public string RelativeLocation => string.IsNullOrEmpty(_model.RelativeLocation)
            ? _model.RelativeLocation
            : _outputLocationOptions.Combine(_model.RelativeLocation);

        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks => Array.Empty<string>();
        public IEnumerable<IOutputTargetRole> Roles => _model.OutputAnchors;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs.DetectDuplicates();
        public IDictionary<string, object> Metadata { get; }
    }
}
