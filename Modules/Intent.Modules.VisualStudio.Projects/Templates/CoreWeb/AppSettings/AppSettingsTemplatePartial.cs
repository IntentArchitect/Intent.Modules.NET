using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Templates.WebConfig;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    partial class AppSettingsTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.AppSettings";
        private readonly IList<AppSettingRegistrationRequest> _appSettings = new List<AppSettingRegistrationRequest>();
        private readonly IList<ConnectionStringRegistrationRequest> _connectionStrings = new List<ConnectionStringRegistrationRequest>();

        public AppSettingsTemplate(IProject project, IApplicationEventDispatcher eventDispatcher)
            : base (Identifier, project, null)
        {
            eventDispatcher.Subscribe<AppSettingRegistrationRequest>(HandleAppSetting);
            eventDispatcher.Subscribe<ConnectionStringRegistrationRequest>(HandleConnectionString);
        }

        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var jsonObject = LoadOrCreate(fullFileName);

            foreach (var appSetting in _appSettings)
            {
                if (jsonObject[appSetting.Key] == null)
                {
                    jsonObject[appSetting.Key] = appSetting.Value;
                }
            }

            foreach (var connectionString in _connectionStrings)
            {
                var configConnectionStrings = jsonObject["ConnectionStrings"];
                if (configConnectionStrings == null)
                {
                    configConnectionStrings = new JObject();
                    jsonObject["ConnectionStrings"] = configConnectionStrings;
                }
                if (jsonObject["ConnectionStrings"][connectionString.Name] == null)
                {
                    jsonObject["ConnectionStrings"][connectionString.Name] = connectionString.ConnectionString;
                }
            }

            return JsonConvert.SerializeObject(jsonObject, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }

        private dynamic LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName)
                ? JsonConvert.DeserializeObject(File.ReadAllText(fullFileName))
                : JsonConvert.DeserializeObject(TransformText());
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "appsettings",
                fileExtension: "json"
            );
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
    }
}
