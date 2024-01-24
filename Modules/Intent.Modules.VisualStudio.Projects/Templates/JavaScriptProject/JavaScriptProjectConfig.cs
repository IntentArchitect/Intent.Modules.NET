using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject
{
    internal class JavaScriptProjectConfig : IOutputTargetConfig
    {
        public JavaScriptProjectModel Model { get; }

        public JavaScriptProjectConfig(JavaScriptProjectModel model)
        {
            Model = model;
            var relativeLocation = model.GetJavaScriptProjectOptions()?.RelativeLocation();
            RelativeLocation = string.IsNullOrWhiteSpace(relativeLocation) ? model.Name : relativeLocation;
        }

        public IEnumerable<IStereotype> Stereotypes => Model.Stereotypes;
        public string Id => Model.Id;
        public string Type => "JavaScriptProject";
        public string Name => Model.Name;
        public string RelativeLocation { get; }
        public string ParentId => null;
        public IEnumerable<string> SupportedFrameworks { get; } = [];
        public IEnumerable<IOutputTargetRole> Roles => Model.Roles;
        public IEnumerable<IOutputTargetTemplate> Templates => Model.TemplateOutputs.DetectDuplicates();
        public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
    }
}
