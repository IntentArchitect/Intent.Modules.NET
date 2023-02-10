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
using Intent.Modules.VisualStudio.Projects.FactoryExtensions.NuGet.HelperTypes;
using Intent.Modules.VisualStudio.Projects.NuGet;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

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

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<VisualStudioProjectCreatedEvent>(@event => _projectTemplates.Add(@event.TemplateInstance));
            base.OnBeforeTemplateRegistrations(application);
        }

        protected override void OnAfterTemplateExecution(IApplication application)
        {
            foreach (var template in _projectTemplates)
            {
                var doc = XDocument.Parse(template.LoadContent());
                if (doc.ResolveProjectScheme() != VisualStudioProjectScheme.Sdk)
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
                    hasChange |= SyncManageableBooleanProperty(doc, "ImplicitUsings", netCoreSettings.ImplicitUsings().Value);
                    hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netCoreSettings.GenerateRuntimeConfigurationFiles().Value);
                    hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netCoreSettings.GenerateDocumentationFile().Value);
                }

                if (template.Project is CSharpProjectNETModel model &&
                    model.HasNETSettings())
                {
                    var netSettings = model.GetNETSettings();

                    if (doc.Root!.Attribute("Sdk")!.Value != netSettings.SDK().Value)
                    {
                        doc.Root.Attribute("Sdk")!.Value = netSettings.SDK().Value;
                        hasChange = true;
                    }

                    hasChange |= SyncProperty(doc, "OutputType", netSettings.OutputType().Value switch
                    {
                        "Class Library" => "Library",
                        "Console Application" => "Exe",
                        "Windows Application" => "WinExe",
                        _ => null
                    });
                    hasChange |= SyncProperty(doc, "AzureFunctionsVersion", netSettings.AzureFunctionsVersion().Value, true);
                    hasChange |= SyncProperty(doc, "Configurations", netSettings.Configurations());
                    hasChange |= SyncProperty(doc, "RuntimeIdentifiers", netSettings.RuntimeIdentifiers());
                    hasChange |= SyncProperty(doc, "UserSecretsId", netSettings.UserSecretsId());
                    hasChange |= SyncProperty(doc, "RootNamespace", netSettings.RootNamespace());
                    hasChange |= SyncProperty(doc, "AssemblyName", netSettings.AssemblyName());
                    hasChange |= SyncManageableBooleanProperty(doc, "ImplicitUsings", netSettings.ImplicitUsings().Value);
                    hasChange |= SyncManageableBooleanProperty(doc, "GenerateRuntimeConfigurationFiles", netSettings.GenerateRuntimeConfigurationFiles().Value);
                    hasChange |= SyncManageableBooleanProperty(doc, "GenerateDocumentationFile", netSettings.GenerateDocumentationFile().Value);
                }

                var projectOptions = template.Project.GetCSharpProjectOptions();
                if (projectOptions != null)
                {
                    hasChange |= SyncProperty(
                        doc: doc,
                        propertyName: "LangVersion",
                        value: projectOptions.LanguageVersion().IsDefault()
                            ? null
                            : projectOptions.LanguageVersion().Value,
                        removeIfNullOrEmpty: true);

                    if (projectOptions.Nullable()?.Value == "(unspecified)")
                    {
                        hasChange |= SyncProperty(doc, "Nullable", null, removeIfNullOrEmpty: true);
                    }
                    else if (!string.IsNullOrWhiteSpace(projectOptions.Nullable()?.Value))
                    {
                        hasChange |= SyncProperty(doc, "Nullable", projectOptions.Nullable().Value);
                    }
                    else if (projectOptions.NullableEnabled())
                    {
                        // NullableEnabled() was the old property which is just a checkbox, we fall
                        // back to it if Nullable() is unset.
                        hasChange |= SyncProperty(doc, "Nullable", "enable");
                    }
                }

                if (!hasChange)
                {
                    continue;
                }

                template.UpdateContent(doc.ToFormattedProjectString(), _sfEventDispatcher);
            }

            base.OnAfterTemplateExecution(application);
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
        /// <term><see langword="false"/> / <c>disable</c></term>
        /// <description>The property's value should be set to <c>false</c>. / <c>disable</c></description>
        /// </item>
        /// <item>
        /// <term><see langword="true"/> / <c>enable</c></term>
        /// <description>The property's value should be set to <c>true</c> / <c>enable</c>.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <returns>True if there was a change.</returns>
        private static bool SyncManageableBooleanProperty(XDocument doc, string propertyName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                value is not "(unspecified)" &&
                value is not "enable" &&
                value is not "disable" &&
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
        private static bool SyncProperty(XDocument doc, string propertyName, string value, bool removeIfNullOrEmpty = false)
        {
            var element = GetPropertyGroupElement(doc, propertyName);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (!removeIfNullOrEmpty ||
                    element == null)
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
            var targetFrameworks = template.GetTargetFrameworks().ToArray();
            if (targetFrameworks.Length == 1 && targetFrameworks[0] == "unspecified")
            {
                // User has chosen "(unspecified)" in the Visual Studio designer, useful for
                // scenarios like when a "Directory.Build.props" is being used to set the
                // value.
                return false;
            }

            var element = GetPropertyGroupElement(doc, "TargetFramework") ??
                          GetPropertyGroupElement(doc, "TargetFrameworks");
            if (element == null)
            {
                Logging.Log.Warning($"Could not determine framework element for project \"{template.FilePath}\". " +
                                    "If you're using a \"Directory.Build.props\" file, change the project's Target " +
                                    "Framework to \"(unspecified)\".");
                return false;
            }

            var elementValue = string.Join(";", targetFrameworks.OrderBy(x => x));
            if (element.Value == elementValue)
            {
                return false;
            }

            var elementName = targetFrameworks.Count() == 1
                ? "TargetFramework"
                : "TargetFrameworks";

            element.ReplaceWith(XElement.Parse($"<{elementName}>{elementValue}</{elementName}>"));

            return true;
        }

        private static XElement GetPropertyGroupElement(XDocument doc, string name)
        {
            var (prefix, namespaceManager, _) = doc.GetNamespaceManager();

            return doc.XPathSelectElement($"/{prefix}:Project/{prefix}:PropertyGroup/{prefix}:{name}", namespaceManager);
        }
    }
}