using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.OutputTargets;

namespace Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject
{
    internal class JavaScriptProjectConfig : IOutputTargetConfig
    {
        public JavaScriptProjectModel Model { get; }

        public JavaScriptProjectConfig(JavaScriptProjectModel model) : this(model, outputLocationOptions: null)
        {
        }

        internal JavaScriptProjectConfig(JavaScriptProjectModel model, OutputLocationOptions outputLocationOptions)
        {
            Model = model;
            var relativeLocation = model.GetJavaScriptProjectOptions()?.RelativeLocation();
            RelativeLocation = (outputLocationOptions ?? OutputLocationOptions.None).GetEffectiveRelativeLocation(relativeLocation, model.Name);
        }

        public IEnumerable<IStereotype> Stereotypes => Model.Stereotypes;
        public string Id => Model.Id;
        public string Type => "JavaScriptProject";
        public string Name => Model.Name;
        public string RelativeLocation { get; }
        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks { get; } = [];
        public IEnumerable<IOutputTargetRole> Roles => Model.OutputAnchors;
        public IEnumerable<IOutputTargetTemplate> Templates => Model.TemplateOutputs.DetectDuplicates();
        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    }
}
