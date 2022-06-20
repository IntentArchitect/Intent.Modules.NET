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

                        var netCoreSettings = template.Project.GetNETCoreSettings();
                        if (netCoreSettings != null)
                        {
                            hasChange |= SyncProperty(doc, "Configurations", netCoreSettings.Configurations());
                            hasChange |= SyncProperty(doc, "RuntimeIdentifiers", netCoreSettings.RuntimeIdentifiers());
                            hasChange |= SyncProperty(doc, "UserSecretsId", netCoreSettings.UserSecretsId());
                            hasChange |= SyncProperty(doc, "RootNamespace", netCoreSettings.RootNamespace());
                            hasChange |= SyncProperty(doc, "AssemblyName", netCoreSettings.AssemblyName());
                            hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netCoreSettings.GenerateRuntimeConfigurationFiles().Value);
                            hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netCoreSettings.GenerateDocumentationFile().Value);
                        }

                        var projectOptions = template.Project.GetCSharpProjectOptions();
                        if (projectOptions != null)
                        {
                            hasChange |= SyncProperty(doc, "LangVersion", projectOptions.LanguageVersion().IsDefault()
                                ? null
                                : projectOptions.LanguageVersion().Value);
                            hasChange |= SyncProperty(doc, "Nullable", projectOptions.NullableEnabled() ? "enable" : null);
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

        /// <summary>
        /// For when <paramref name="value"/> is one of the following:
        /// <list type="table">
        /// <item>
        /// <term><see langword="null"/></term>
        /// <description>The property's value us "unmanaged" by Intent and should not be changed, added, or removed.</description>
        /// </item>
        /// <item>
        /// <term>"(unspecified)"</term>
        /// <description>The property should be removed from the <c>.csproj</c> file.</description>
        /// </item>
        /// <item>
        /// <term><see langword="false"/></term>
        /// <description>The property's value should be set to <c>false</c>.</description>
        /// </item>
        /// <item>
        /// <term><see langword="true"/></term>
        /// <description>The property's value should be set to <c>true</c>.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <returns>True if there was a change.</returns>
        private static bool SyncManageableBooleanProperty(XDocument doc, string propertyName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                value is not "(unspecified)" &&
                value is not "true" &&
                value is not "false")
            {
                throw new ArgumentOutOfRangeException(nameof(value), value);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return SyncProperty(doc, propertyName, value == "(unspecified)" ? null : value);
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