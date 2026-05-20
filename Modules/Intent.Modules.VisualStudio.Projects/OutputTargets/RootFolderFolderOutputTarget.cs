using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.Types.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.VisualStudio.Projects.OutputTargets
{
    internal class RootFolderFolderOutputTarget : IOutputTargetConfig
    {
        private readonly FolderExtensionModel _model;

        public RootFolderFolderOutputTarget(FolderModel model)
        {
            _model = new Intent.Modelers.CodebaseStructure.Api.FolderExtensionModel(model.InternalElement);
            Metadata = new Dictionary<string, object>
            {
                { "Namespace Provider", model.GetFolderOptions()?.NamespaceProvider() ?? true }
            };
        }
        public IEnumerable<IStereotype> Stereotypes => _model.Stereotypes;
        public string Id => _model.Id;
        public string Type => _model.InternalElement.SpecializationType; // Folder
        public string Name => _model.Name;
        public string RelativeLocation => _model.Name;
        public string ParentId => _model.InternalElement.ParentId;

        public IEnumerable<string> SupportedFrameworks => [];
        public IEnumerable<IOutputTargetRole> Roles => _model.OutputAnchors;
        public IEnumerable<IOutputTargetTemplate> Templates => _model.TemplateOutputs.DetectDuplicates();
        public IDictionary<string, object> Metadata { get; }
    }
}

