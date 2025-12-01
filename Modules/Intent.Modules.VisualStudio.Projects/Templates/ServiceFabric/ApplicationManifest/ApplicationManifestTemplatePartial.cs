using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric.ApplicationManifest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class ApplicationManifestTemplate : IntentTemplateBase<ServiceFabricProjectModel>, IXmlDocumentTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.ServiceFabric.ApplicationManifest";

        private readonly List<Action> _onAfterTemplateRegistrations = [];

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApplicationManifestTemplate(IOutputTarget outputTarget, ServiceFabricProjectModel model) : base(TemplateId, outputTarget, model)
        {
            OutputTarget.On<ManifestImportRequiredEvent>(e =>
            {
                if (Document != null)
                {
                    OnImportRequired(e.Data);
                }
                else
                {
                    _onAfterTemplateRegistrations.Add(() => OnImportRequired(e.Data));
                }
            });
            OutputTarget.On<ServiceRegistrationRequiredBase>(e =>
            {
                if (Model.GetServiceFabricSettings().GenerateStartupServices())
                {
                    return;
                }

                if (Document != null)
                {
                    OnServiceRegistrationRequired(e.Data);
                }
                else
                {
                    _onAfterTemplateRegistrations.Add(() => OnServiceRegistrationRequired(e.Data));
                }
            });
        }

        public XmlDocument Document { get; private set; }

        public XmlNamespaceManager NamespaceManager { get; private set; }

        public string Namespace => "http://schemas.microsoft.com/2011/01/fabric";

        private void OnImportRequired(ManifestImportRequiredEvent @event)
        {
            var existing = Document.SelectSingleNode(
                $"/f:ApplicationManifest/f:ServiceManifestImport[f:ServiceManifestRef/@ServiceManifestName='{@event.ServiceManifestName}']",
                NamespaceManager);

            if (existing != null)
            {
                return;
            }

            var import = Document.CreateElement("ServiceManifestImport", Namespace);
            import.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));

            var defaultServicesNode = Document.SelectSingleNode("/f:ApplicationManifest/f:DefaultServices", NamespaceManager);
            if (defaultServicesNode != null)
            {
                Document.DocumentElement!.InsertBefore(Document.CreateWhitespace("  "), defaultServicesNode);
                Document.DocumentElement.InsertBefore(import, defaultServicesNode);
                Document.DocumentElement.InsertBefore(Document.CreateWhitespace($"{Environment.NewLine}"), defaultServicesNode);
            }
            else
            {
                Document.DocumentElement!.AppendChild(Document.CreateWhitespace("  "));
                Document.DocumentElement.AppendChild(import);
                Document.DocumentElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}"));
            }

            var manifestRef = Document.CreateElement("ServiceManifestRef", Namespace);
            manifestRef.SetAttribute("ServiceManifestName", @event.ServiceManifestName);
            manifestRef.SetAttribute("ServiceManifestVersion", "1.0.0");
            import.AppendChild(Document.CreateWhitespace("  "));
            import.AppendChild(manifestRef);
            import.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));

            if (@event.ConfigOverrides != null)
            {
                var configOverrides = Document.CreateElement("ConfigOverrides", Namespace);
                if (@event.ConfigOverrides.Count > 0)
                {
                    configOverrides.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}    "));
                }

                foreach (var configOverride in @event.ConfigOverrides)
                {
                    var settings = Document.CreateElement("Settings", Namespace);
                    settings.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}        "));

                    foreach (var section in configOverride.SettingSections)
                    {
                        var sectionElement = Document.CreateElement("Section", Namespace);
                        sectionElement.SetAttribute("Name", section.Name);
                        sectionElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}          "));

                        settings.AppendChild(Document.CreateWhitespace("  "));
                        settings.AppendChild(sectionElement);
                        settings.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}        "));

                        foreach (var parameter in section.Parameters)
                        {
                            var parameterElement = Document.CreateElement("Parameter", Namespace);
                            parameterElement.SetAttribute("Name", parameter.Name);
                            parameterElement.SetAttribute("Value", parameter.Value ?? "");
                            this.AddParameterMaybe(parameter.Value, parameter.ParameterDefaultValue);

                            sectionElement.AppendChild(Document.CreateWhitespace("  "));
                            sectionElement.AppendChild(parameterElement);
                            sectionElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}          "));
                        }
                    }

                    var configOverrideElement = Document.CreateElement("ConfigOverride", Namespace);
                    configOverrideElement.SetAttribute("Name", configOverride.Name);
                    configOverrideElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}        "));
                    configOverrideElement.AppendChild(settings);
                    configOverrideElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}      "));

                    configOverrides.AppendChild(Document.CreateWhitespace("  "));
                    configOverrides.AppendChild(configOverrideElement);
                    configOverrides.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}    "));
                }

                import.AppendChild(Document.CreateWhitespace("  "));
                import.AppendChild(configOverrides);
                import.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));

                if (@event.EnvironmentOverrides?.Count > 0)
                {
                    var environmentOverrides = Document.CreateElement("EnvironmentOverrides", Namespace);
                    environmentOverrides.SetAttribute("CodePackageRef", "Code");
                    environmentOverrides.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}    "));

                    foreach (var item in @event.EnvironmentOverrides)
                    {
                        var environmentOverride = Document.CreateElement("Parameter", Namespace);
                        environmentOverride.SetAttribute("Name", item.Name);
                        environmentOverride.SetAttribute("Value", item.Value ?? "");
                        this.AddParameterMaybe(item.Value, item.ParameterDefaultValue);

                        environmentOverrides.AppendChild(Document.CreateWhitespace("  "));
                        environmentOverrides.AppendChild(environmentOverride);
                        environmentOverrides.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}    "));
                    }

                    import.AppendChild(Document.CreateWhitespace("  "));
                    import.AppendChild(environmentOverrides);
                    import.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));
                }
            }
        }

        private void OnServiceRegistrationRequired(ServiceRegistrationRequiredBase @event)
        {
            var existing = Document.SelectSingleNode(
                $"/f:ApplicationManifest/f:DefaultServices/f:Service[@Name='{@event.Name}']",
                NamespaceManager);

            if (existing != null)
            {
                return;
            }

            var serviceElement = Helpers.CreateServiceElement(@event: @event, template: this);
            serviceElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}    "));

            var defaultServicesNode = Document.SelectSingleNode("/f:ApplicationManifest/f:DefaultServices", NamespaceManager);
            if (defaultServicesNode == null)
            {
                defaultServicesNode = Document.CreateElement("DefaultServices", Namespace);
                defaultServicesNode.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));

                Document.DocumentElement!.AppendChild(Document.CreateWhitespace("  "));
                Document.DocumentElement.AppendChild(defaultServicesNode);
                Document.DocumentElement.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}"));
            }

            defaultServicesNode.AppendChild(Document.CreateWhitespace("  "));
            defaultServicesNode.AppendChild(serviceElement);
            defaultServicesNode.AppendChild(Document.CreateWhitespace($"{Environment.NewLine}  "));
        }

        public override void AfterTemplateRegistration()
        {
            if (!TryGetExistingFileContent(out var existingFileContent))
            {
                existingFileContent = TransformText().ReplaceLineEndings();
            }

            Document = new XmlDocument
            {
                PreserveWhitespace = true,
            };
            Document.LoadXml(existingFileContent);
            NamespaceManager = new XmlNamespaceManager(Document.NameTable);
            NamespaceManager.AddNamespace("f", Namespace);

            foreach (var action in _onAfterTemplateRegistrations)
            {
                action();
            }

            _onAfterTemplateRegistrations.Clear();

            base.AfterTemplateRegistration();
        }

        public override string RunTemplate()
        {
            return Document.ToUtf8String();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: "ApplicationManifest",
                fileExtension: "xml",
                relativeLocation: "ApplicationPackageRoot"
            );
        }

    }
}