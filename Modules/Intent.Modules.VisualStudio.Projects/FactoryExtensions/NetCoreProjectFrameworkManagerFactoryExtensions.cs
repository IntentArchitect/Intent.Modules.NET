using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    public class NetCoreProjectFrameworkManagerFactoryExtensions : FactoryExtensionBase, IExecutionLifeCycle
    {
        private readonly IList<IVisualStudioProjectTemplate> _projects = new List<IVisualStudioProjectTemplate>();
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;

        public override string Id => "Intent.VSProjects.NetCoreProjectFrameworkManager";

        public override int Order => 1000;

        public NetCoreProjectFrameworkManagerFactoryExtensions(ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            _sfEventDispatcher = sfEventDispatcher;
        }

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateRegistrations)
            {
                application.EventDispatcher.Subscribe<VisualStudioProjectCreatedEvent>(HandleEvent);
            }
            else if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                foreach (var project in _projects)
                {
                    var projectContent = project.LoadContent();
                    var doc = XDocument.Parse(projectContent);
                    if (doc.ResolveProjectScheme() == VisualStudioProjectScheme.Lean)
                    {
                        var (prefix, namespaceManager, namespaceName) = doc.GetNamespaceManager();
                        var framework = doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:TargetFramework", namespaceManager) ?? doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:TargetFrameworks", namespaceManager);
                        if (framework == null)
                        {
                            throw new Exception("Could not determine framework element for project: " + project.FilePath);
                        }

                        var targetFrameworks = string.Join(";", project.GetTargetFrameworks().OrderBy(x => x));
                        if (framework.Value == targetFrameworks)
                        {
                            continue;
                        }
                        if (project.GetTargetFrameworks().Count() == 1)
                        {
                            framework.ReplaceWith(XElement.Parse($"<TargetFramework>{targetFrameworks}</TargetFramework>"));
                        }
                        else
                        {
                            framework.ReplaceWith(XElement.Parse($"<TargetFrameworks>{targetFrameworks}</TargetFrameworks>"));
                        }
                        project.UpdateContent(doc.ToFormattedProjectString(), _sfEventDispatcher);
                    }
                }
            }
        }

        private void HandleEvent(VisualStudioProjectCreatedEvent @event)
        {
            _projects.Add(@event.TemplateInstance);
        }

        public void UpdateProjectFrameworks(IApplication application)
        {
            // Resolve all dependencies and events
            Logging.Log.Info($"Updating project frameworks");

            foreach (var project in application.OutputTargets.Where(x => VisualStudioProjectExtensions.IsVSProject(x)))
            {

            }
            
        }
    }
}