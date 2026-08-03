using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.VisualStudio.Projects.OutputTargets
{
    public class RootFolderOutputTarget : IOutputTargetConfig
    {
        private readonly RootFolderModel _model;
        private readonly OutputLocationOptions _outputLocationOptions;

        public RootFolderOutputTarget(RootFolderModel model) : this(model, outputLocationOptions: null)
        {
        }

        internal RootFolderOutputTarget(RootFolderModel model, OutputLocationOptions outputLocationOptions)
        {
            _model = model;
            _outputLocationOptions = outputLocationOptions ?? OutputLocationOptions.None;
        }

        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => "Folder";
        public string Name => _model.Name;

        // The Root Folder is the anchor everything else (Projects, Solution Folders) shifts relative to via
        // Combine/GetEffectiveRelativeLocation - so its own target must move by the shift itself, landing it
        // (and anything anchored to it, e.g. static file drops outside the VS Solution) at RootDirectory.
        public string RelativeLocation => _outputLocationOptions.RelativeLocation;
        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks => new string[0];
        public IEnumerable<IOutputTargetRole> Roles => _model.OutputAnchors;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs;
        public IDictionary<string, object> Metadata { get; }
    }
}
