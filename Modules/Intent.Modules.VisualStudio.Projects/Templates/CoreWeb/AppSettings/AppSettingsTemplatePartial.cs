using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                using (var jw = new JsonTextWriter(sw))
                {
                    var settings = GetEditorConfigSettings();
                    jw.Formatting = Formatting.Indented;
                    jw.Indentation = int.Parse(settings["indent_size"]);
                    jw.IndentChar = settings["indent_style"].Equals("tab", StringComparison.OrdinalIgnoreCase) ? '\t' : ' '; ;

                    var serializer = new JsonSerializer();
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
            var filePath = FileMetadata.GetFilePath();
            var directory = Path.GetDirectoryName(filePath);

            var editorConfigs = GetEditorConfigFiles(directory);
            if (editorConfigs.Count == 0)
            {
                return DefaultSettings;
            }

            var settings = new Dictionary<string, string>(DefaultSettings, StringComparer.OrdinalIgnoreCase);
            foreach (var editorConfig in editorConfigs)
            {
                try
                {
                    ParseEditorConfigToSettings(editorConfig, settings);
                }
                catch (Exception ex)
                {
                    Logging.Log.Warning($"Unable to parse .editorConfig at {editorConfig}({ex.Message})");
                }
            }

            return settings;
        }

        /// <summary>
        /// Returns content of .editorconfig files ordered by parent folder depth ascending.
        /// </summary>
        private List<string> GetEditorConfigFiles(string directory)
        {
            var editorConfigContentFromTemplates = ExecutionContext
                .FindTemplateInstances<IntentTemplateBase>(TemplateDependency.OfType<IntentTemplateBase>())
                .Where(template => template.FileMetadata.GetFilePath().EndsWith(".editorconfig", StringComparison.OrdinalIgnoreCase))
                .Select(template =>
                {
                    string content;
                    var currentPath = template.FileMetadata.GetFilePath();
                    var previousPath = template.GetExistingFilePath();

                    switch (template.FileMetadata.OverwriteBehaviour)
                    {
                        case OverwriteBehaviour.Always:
                            content = template.RunTemplate();
                            break;
                        case OverwriteBehaviour.OnceOff:
                        case OverwriteBehaviour.OverwriteDisabled:
                            if (!template.TryGetExistingFileContent(out content))
                            {
                                content = template.RunTemplate();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    return new
                    {
                        Content = content,
                        CurrentPath = currentPath,
                        PreviousPath = previousPath
                    };
                })
                .ToArray();

            var editorConfigFiles = new List<string>();

            // Start from the specified directory
            var currentDir = directory;

            // Traverse upwards until we reach the root
            while (!string.IsNullOrEmpty(currentDir))
            {
                var pathToCheck = Path.Combine(currentDir, ".editorconfig");

                var editorConfigTemplate = editorConfigContentFromTemplates.SingleOrDefault(x => x.CurrentPath == pathToCheck);

                // First check if a template outputs to the path as it will have the content as appropriate depending
                // on whether the file was renamed and its overwrite behaviour.
                if (editorConfigTemplate != default)
                {
                    editorConfigFiles.Add(editorConfigTemplate.Content);
                }
                // Ignore renamed files as already covered above
                else if (editorConfigContentFromTemplates.Any(x => x.PreviousPath == pathToCheck && x.PreviousPath != x.CurrentPath))
                {
                    // NOP
                }
                // Finally check to see if there is a completely unmanaged file on the disk drive
                else if (File.Exists(pathToCheck))
                {
                    editorConfigFiles.Add(File.ReadAllText(pathToCheck));
                }

                // Move to the parent directory
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }

            return editorConfigFiles;
        }

        private static void ParseEditorConfigToSettings(string content, Dictionary<string, string> settings)
        {
            var lines = content.ReplaceLineEndings("\n").Split('\n');

            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                {
                    continue; // Skip comments and section headers
                }
                if (line.StartsWith('['))
                {
                    var match = line.Substring(1, line.Length - 2); // Remove []
                    if (match is not "*" or "*.json")
                    {
                        continue;
                    }
                }

                var keyValue = line.Split(['='], 2);
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
