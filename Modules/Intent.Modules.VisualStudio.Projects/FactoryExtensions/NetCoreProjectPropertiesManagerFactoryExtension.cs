using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Plugins;
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

                        var hasChange = false;

                        hasChange |= SyncFrameworks(doc, template);

                        var settings = template.Project.GetNETCoreSettings();
                        if (settings != null)
                        {
                            hasChange |= SyncProperty(doc, "Configurations", settings.Configurations());
                            hasChange |= SyncProperty(doc, "RuntimeIdentifiers", settings.RuntimeIdentifiers());
                            hasChange |= SyncProperty(doc, "UserSecretsId", settings.UserSecretsId());
                            hasChange |= SyncProperty(doc, "RootNamespace", settings.RootNamespace());
                            hasChange |= SyncProperty(doc, "GenerateRuntimeConfigurationFiles", settings.GenerateRuntimeConfigurationFiles().Value);
                            hasChange |= SyncProperty(doc, "GenerateDocumentationFile", settings.GenerateDocumentationFile().Value);
                        }

                        if (!hasChange)
                        {
                            continue;
                        }

                        template.UpdateContent(doc.ToFormattedProjectString(), _sfEventDispatcher);
                    }

                    break;
            }
        }

        /// <returns>True if there was a change.</returns>
        private static bool SyncProperty(XDocument doc, string propertyName, string value)
        {
            var element = GetPropertyGroupElement(doc, propertyName);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (element == null)
                {
                    return false;
                }

                element.Remove();
                return true;
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

            if (element.Value == value)
            {
                return false;
            }

            element.Value = value;
            return true;
        }

        /// <returns>True if there was a change.</returns>
        private static bool SyncFrameworks(XDocument doc, IVisualStudioProjectTemplate template)
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
                return false;
            }

            var elementName = template.GetTargetFrameworks().Count() == 1
                ? "TargetFramework"
                : "TargetFrameworks";

            element.ReplaceWith(XElement.Parse($"<{elementName}>{targetFrameworks}</{elementName}>"));

            return true;
        }

        private static XElement GetPropertyGroupElement(XDocument doc, string name)
        {
            var (prefix, namespaceManager, _) = doc.GetNamespaceManager();

            return doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:{name}", namespaceManager);
        }
    }
}