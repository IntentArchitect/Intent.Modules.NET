using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Eventing;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.WebConfig
{
    public class WebApiWebConfigFileTemplate : IntentFileTemplateBase<object>, ITemplate, IHasDecorators<IWebConfigDecorator>
    {
        public const string IDENTIFIER = "Intent.VisualStudio.Projects.WebConfig";


        private IList<IWebConfigDecorator> _decorators = new List<IWebConfigDecorator>();
        private readonly IList<AppSettingRegistrationRequest> _appSettings = new List<AppSettingRegistrationRequest>();
        private readonly IList<ConnectionStringRegistrationRequest> _connectionStrings = new List<ConnectionStringRegistrationRequest>();

        public WebApiWebConfigFileTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base(IDENTIFIER, project, null)
        {
            eventDispatcher.Subscribe<AppSettingRegistrationRequest>(HandleAppSetting);
            eventDispatcher.Subscribe<ConnectionStringRegistrationRequest>(HandleConnectionString);
        }

        public override string GetCorrelationId()
        {
            return $"{IDENTIFIER}#{OutputTarget.Id}";
        }

        private void HandleAppSetting(AppSettingRegistrationRequest @event)
        {
            if (_appSettings.Any(x => x.Key == @event.Key && x.Value != @event.Value))
            {
                // Throw exception?
                return;
            }
            _appSettings.Add(@event);
        }

        private void HandleConnectionString(ConnectionStringRegistrationRequest @event)
        {
            if (_connectionStrings.Any(x => x.Name == @event.Name && x.ConnectionString != @event.ConnectionString))
            {
                throw new Exception($"Misconfiguration in [{GetType().Name}]: ConnectionString with name [{@event.Name}] already defined with different value to [{@event.ConnectionString}].");
            }
            _connectionStrings.Add(@event);
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: "Web",
                fileExtension: "config",
                relativeLocation: ""
                );
        }

        public override string RunTemplate()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = TransformText();
            }

            var doc = XDocument.Parse(content);
            if (doc == null) throw new Exception("doc is null");
            if (doc.Root == null) throw new Exception("doc.Root is null");

            var namespaces = new XmlNamespaceManager(new NameTable());
            namespaces.AddNamespace("ns", doc.Root.GetDefaultNamespace().NamespaceName);

            var configurationElement = doc.XPathSelectElement("/ns:configuration", namespaces);
            if (configurationElement == null)
            {
                doc.Add(configurationElement = new XElement("configuration"));
            }

            // App Settings:
            var appSettingsElement = doc.XPathSelectElement("/ns:configuration/ns:appSettings", namespaces);
            if (appSettingsElement == null && _appSettings.Any())
            {
                configurationElement.AddFirst(appSettingsElement = new XElement("appSettings"));
            }

            foreach (var appSetting in _appSettings)
            {
                if (appSettingsElement == null) throw new Exception("appSettingsElement is null");

                if (appSettingsElement.XPathSelectElement($"//add[@key='{appSetting.Key}']", namespaces) == null)
                {
                    var setting = new XElement("add");
                    setting.Add(new XAttribute("key", appSetting.Key));
                    setting.Add(new XAttribute("value", appSetting.Value));
                    appSettingsElement.Add(setting);
                }
            }

            // Connection Strings:
            var connectionStringsElement = doc.XPathSelectElement("/ns:configuration/ns:connectionStrings", namespaces);
            if (connectionStringsElement == null && _connectionStrings.Any())
            {
                connectionStringsElement = new XElement("connectionStrings");

                if (appSettingsElement != null)
                {
                    appSettingsElement.AddBeforeSelf(connectionStringsElement);
                }
                else
                {
                    configurationElement.AddFirst(connectionStringsElement);
                }
            }

            foreach (var connectionString in _connectionStrings)
            {
                if (connectionStringsElement == null) throw new Exception("connectionStringsElement is null");

                if (connectionStringsElement.XPathSelectElement($"//add[@name='{connectionString.Name}']", namespaces) == null)
                {
                    var setting = new XElement("add");
                    setting.Add(new XAttribute("name", connectionString.Name));
                    setting.Add(new XAttribute("providerName", connectionString.ProviderName));
                    setting.Add(new XAttribute("connectionString", connectionString.ConnectionString));
                    connectionStringsElement.Add(setting);
                }
            }

            foreach (var webConfigDecorator in GetDecorators())
            {
                webConfigDecorator.Install(doc, Project);
            }

            return doc.ToStringUTF8();
        }

        public override string TransformText()
        {
            return @"<!--
  For more information on how to configure your ASP.NET application, please visit
  http://go.microsoft.com/fwlink/?LinkId=301879
  -->
<configuration>
  <configSections>
  </configSections>
  <appSettings>
  </appSettings>
  <system.web>
    <compilation debug=""true"" targetFramework=""4.5.2"" />
    <httpRuntime targetFramework=""4.5.2"" />
  </system.web>
  <system.webServer>
    <handlers>
      <remove name=""ExtensionlessUrlHandler-Integrated-4.0"" />
      <remove name=""OPTIONSVerbHandler"" />
      <remove name=""TRACEVerbHandler"" />
      <add name=""ExtensionlessUrlHandler-Integrated-4.0"" path=""*."" verb=""*"" type=""System.Web.Handlers.TransferRequestHandler"" preCondition=""integratedMode,runtimeVersionv4.0"" />
    </handlers>
  </system.webServer>
  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
    </assemblyBinding>
  </runtime>
</configuration>";
        }

        public IEnumerable<IWebConfigDecorator> GetDecorators()
        {
            return _decorators;
        }

        public void AddDecorator(IWebConfigDecorator decorator)
        {
            _decorators.Add(decorator);
        }
    }
}
