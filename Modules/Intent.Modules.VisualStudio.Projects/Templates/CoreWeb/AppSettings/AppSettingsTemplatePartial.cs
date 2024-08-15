using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Intent.Modules.Constants.TemplateRoles.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AppSettingsTemplate : IntentTemplateBase<AppSettingsModel, AppSettingsDecorator>
    {

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.CoreWeb.AppSettings";
        private readonly IList<AppSettingRegistrationRequest> _appSettings = new List<AppSettingRegistrationRequest>();
        private readonly IList<ConnectionStringRegistrationRequest> _connectionStrings = new List<ConnectionStringRegistrationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AppSettingsTemplate(IOutputTarget outputTarget, AppSettingsModel model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(HandleAppSetting);
            ExecutionContext.EventDispatcher.Subscribe<ConnectionStringRegistrationRequest>(HandleConnectionString);
        }

        public override string GetCorrelationId()
        {
            return $"{TemplateId}-{Model.RuntimeEnvironment?.Name ?? "Default"}#{OutputTarget.Id}";
        }

        public bool IncludeAllowHosts()
        {
            return Model.IncludeAllowHosts;
        }

        public bool IncludeAspNetCoreLoggingLevel()
        {
            return Model.IncludeAspNetCoreLoggingLevel;
        }

        public override string RunTemplate()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = TransformText();
            }

            var json = JsonConvert.DeserializeObject<JObject>(content);

            foreach (var appSetting in _appSettings)
            {
                json.SetFieldValue(appSetting.Key, appSetting.Value, false);
            }

            foreach (var connectionString in _connectionStrings)
            {
                var configConnectionStrings = json["ConnectionStrings"];
                if (configConnectionStrings == null)
                {
                    configConnectionStrings = new JObject();
                    json["ConnectionStrings"] = configConnectionStrings;
                }

                configConnectionStrings[connectionString.Name] ??= connectionString.ConnectionString;
            }

            var appSettings = new AppSettingsEditor(json);

            foreach (var decorator in GetDecorators())
            {
                decorator.UpdateSettings(appSettings);
            }

            using (var sw = new StringWriter())
            {
                using (JsonTextWriter jw = new JsonTextWriter(sw))
                {
                    var settings = GetEditorConfigSettings();
                    jw.Formatting = Formatting.Indented;
                    jw.Indentation = int.Parse(settings["indent_size"]);
                    jw.IndentChar = settings["indent_style"].Equals("tab", StringComparison.OrdinalIgnoreCase) ? '\t' : ' '; ;

                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(jw, json);
                    return sw.ToString();
                }
            }
        }

        static Dictionary<string, string> DefaultSettings { get; } = new()
        {
            { "indent_size", "2" },
            { "indent_style", "space" }
        };

        private Dictionary<string, string> GetEditorConfigSettings()
        {
            string filePath = FileMetadata.GetFilePath();
            string directory = Path.GetDirectoryName(filePath);

            var editorFiles = GetEditorFiles(directory);
            if (editorFiles.Any())
            {
                var settings = new Dictionary<string, string>(DefaultSettings, StringComparer.OrdinalIgnoreCase);
                foreach (var editorFile in editorFiles)
                {
                    try
                    {
                        ParseEditorConfigToSettings(editorFile, settings);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log.Warning($"Unable to parse .editorConfig at {editorFile}({ex.Message})");
                    }
                }
                return settings;
            }

            return DefaultSettings;
        }

        static List<string> GetEditorFiles(string directory)
        {
            var editorConfigFiles = new List<string>();

            // Start from the specified directory
            string currentDir = directory;

            // Traverse upwards until we reach the root
            while (!string.IsNullOrEmpty(currentDir))
            {
                string editorConfigFile = Path.Combine(currentDir, ".editorconfig");

                // Check if .editorconfig file exists in the current directory
                if (File.Exists(editorConfigFile))
                {
                    editorConfigFiles.Add(editorConfigFile);
                }

                // Move to the parent directory
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            return editorConfigFiles;
        }

        static void ParseEditorConfigToSettings(string path, Dictionary<string, string> settings)
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines.Select(l => l.Trim()))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue; // Skip comments and section headers
                }
                if (line.StartsWith("["))
                {
                    string match = line.Substring(1, line.Length - 2);//Remove[]
                    if (match is not "*" or "*.json")
                    {
                        continue;
                    }
                }

                var keyValue = line.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    settings[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var (runtimeEnvironment, dependsOn) = !string.IsNullOrWhiteSpace(Model.RuntimeEnvironment?.Name)
                ? ($".{Model.RuntimeEnvironment.Name.ToPascalCase()}", "appsettings.json")
                : (null, null);

            return new TemplateFileConfig(
                fileName: $"appsettings{runtimeEnvironment}",
                fileExtension: "json",
                relativeLocation: Model.Location)
                    .WithDependsOn(dependsOn);
        }

        private void HandleAppSetting(AppSettingRegistrationRequest @event)
        {
            if (Model.RuntimeEnvironment?.Name != @event.RuntimeEnvironment)
            {
                return;
            }

            if (!@event.IsApplicableTo(this, requiresSpecifiedRole: Model.RequiresSpecifiedRole))
            {
                return;
            }

            @event.MarkHandled();

            if (_appSettings.Any(x => x.Key == @event.Key && x.Value != @event.Value))
            {
                return;
            }

            _appSettings.Add(@event);
        }

        private void HandleConnectionString(ConnectionStringRegistrationRequest @event)
        {
            if (Model.RuntimeEnvironment?.Name != @event.RuntimeEnvironment)
            {
                return;
            }

            if (!@event.IsApplicableTo(this, requiresSpecifiedRole: Model.RequiresSpecifiedRole))
            {
                return;
            }

            @event.MarkHandled();

            if (_connectionStrings.Any(x => x.Name == @event.Name && x.ConnectionString != @event.ConnectionString))
            {
                throw new Exception($"Misconfiguration in [{GetType().Name}]: ConnectionString with name [{@event.Name}] already defined with different value to [{@event.ConnectionString}].");
            }

            _connectionStrings.Add(@event);
        }
    }
}
