using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Plugins;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    public class NetCoreProjectPropertiesManagerFactoryExtension : FactoryExtensionBase, IExecutionLifeCycle
    {
        private readonly IList<IVisualStudioProjectTemplate> _projectTemplates = new List<IVisualStudioProjectTemplate>();
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;

        public override string Id => "Intent.VSProjects.NetCoreProjectPropertiesManager";

        public override int Order => 1000;

        public NetCoreProjectPropertiesManagerFactoryExtension(ISoftwareFactoryEventDispatcher sfEventDispatcher)
        {
            _sfEventDispatcher = sfEventDispatcher;
        }

        public void OnStep(IApplication application, string step)
        {
            switch (step)
            {
                case ExecutionLifeCycleSteps.BeforeTemplateRegistrations:
                    application.EventDispatcher.Subscribe<VisualStudioProjectCreatedEvent>(@event => _projectTemplates.Add(@event.TemplateInstance));
                    break;
                case ExecutionLifeCycleSteps.AfterTemplateExecution:
                    foreach (var template in _projectTemplates)
                    {
                        var doc = XDocument.Parse(template.LoadContent());
                        if (doc.ResolveProjectScheme() != VisualStudioProjectScheme.Lean)
                        {
                            continue;
                        }

                        SyncFrameworks(doc, template);

                        var settings = template.Project.GetNETCoreSettings();
                        if (settings != null)
                        {
                            SyncProperty(doc, "Configurations", settings.Configurations());
                            SyncProperty(doc, "RuntimeIdentifiers", settings.RuntimeIdentifiers());
                            SyncProperty(doc, "UserSecretsId", settings.UserSecretsId());
                            SyncProperty(doc, "RootNamespace", settings.RootNamespace());
                            SyncProperty(doc, "GenerateRuntimeConfigurationFiles", settings.GenerateRuntimeConfigurationFiles().Value);
                        }

                        template.UpdateContent(doc.ToFormattedProjectString(), _sfEventDispatcher);
                    }

                    break;
            }
        }

        private static void SyncProperty(XDocument doc, string propertyName, string value)
        {
            var element = GetPropertyGroupElement(doc, propertyName);
            if (string.IsNullOrWhiteSpace(value))
            {
                element?.Remove();
                return;
            }


            if (element == null)
            {
                var propertyGroupElement = GetPropertyGroupElement(doc, "TargetFramework")?.Parent ??
                                           GetPropertyGroupElement(doc, "TargetFrameworks")?.Parent;
                if (propertyGroupElement == null)
                {
                    throw new Exception("Could not determine target property group element.");
                }

                element = new XElement(propertyName);
                propertyGroupElement.Add(element);
            }

            element.Value = value;
        }

        private static void SyncFrameworks(XDocument doc, IVisualStudioProjectTemplate template)
        {
            var element = GetPropertyGroupElement(doc, "TargetFramework") ??
                          GetPropertyGroupElement(doc, "TargetFrameworks");
            if (element == null)
            {
                throw new Exception("Could not determine framework element for project: " + template.FilePath);
            }

            var targetFrameworks = string.Join(";", template.GetTargetFrameworks().OrderBy(x => x));
            if (element.Value == targetFrameworks)
            {
                return;
            }

            var elementName = template.GetTargetFrameworks().Count() == 1
                ? "TargetFramework"
                : "TargetFrameworks";

            element.ReplaceWith(XElement.Parse($"<{elementName}>{targetFrameworks}</{elementName}>"));
        }

        private static XElement GetPropertyGroupElement(XDocument doc, string name)
        {
            var (prefix, namespaceManager, _) = doc.GetNamespaceManager();

            return doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:{name}", namespaceManager);
        }
    }
}