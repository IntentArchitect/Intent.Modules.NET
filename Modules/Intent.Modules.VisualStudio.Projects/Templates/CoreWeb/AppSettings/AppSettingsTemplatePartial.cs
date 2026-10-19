using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Sync;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
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
        private readonly List<string> _droppedKeys = [];
        private bool _droppedKeysLogged;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AppSettingsTemplate(IOutputTarget outputTarget, AppSettingsModel model = null) : base(TemplateId, outputTarget, model)
        {
            OnEmitOrPublished<AppSettingRegistrationRequest>(HandleAppSetting);
            OnEmitOrPublished<ConnectionStringRegistrationRequest>(HandleConnectionString);
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

        /// <summary>
        /// Adds a setting to this file only. Unlike an <see cref="AppSettingRegistrationRequest"/>, which
        /// every applicable host project claims, it can't land in another project's file. Ignored when
        /// the template is hands-off.
        /// </summary>
        internal void AddSetting(string key, object value)
        {
            if (Model.IsHandsOff)
            {
                _droppedKeys.Add(key);
                return;
            }

            if (_appSettings.Any(x => x.Key == key))
            {
                return;
            }

            _appSettings.Add(new AppSettingRegistrationRequest(key, value));
        }

        public override bool CanRunTemplate()
        {
            if (Model.IsHandsOff)
            {
                // Never create the file. An existing one is still "generated" (unchanged, below) so that
                // the Software Factory neither rewrites it nor proposes deleting it.
                LogDroppedKeys();
                return TryGetExistingFileContent(out _) && base.CanRunTemplate();
            }

            return base.CanRunTemplate();
        }

        public override string RunTemplate()
        {
            var hasExistingFile = TryGetExistingFileContent(out var existingContent);

            if (Model.IsHandsOff)
            {
                LogDroppedKeys();
                return existingContent;
            }

            if (hasExistingFile)
            {
                return UpdateExistingFile(existingContent, ApplySettings);
            }

            var settings = GetEditorConfigSettings();
            return CreateNewFile(
                content: TransformText(),
                applySettings: ApplySettings,
                indentChar: settings["indent_style"].Equals("tab", StringComparison.OrdinalIgnoreCase) ? '\t' : ' ',
                indentation: int.Parse(settings["indent_size"]));
        }

        /// <summary>
        /// An existing file is the developer's: it is returned byte-for-byte when nothing was added, and
        /// otherwise keeps its own indentation and newlines rather than taking them from .editorconfig.
        /// </summary>
        internal static string UpdateExistingFile(string existingContent, Action<JObject> applySettings)
        {
            var (cleanContent, commentBlocks) = JsonCommentPreserver.ExtractAndStrip(existingContent);
            var original = JsonFileFormatting.Parse(cleanContent);
            var json = (JObject)original.DeepClone();

            applySettings(json);

            if (JToken.DeepEquals(original, json))
            {
                return existingContent;
            }

            var updated = JsonCommentPreserver.Restore(JsonFileFormatting.Serialize(json, cleanContent), commentBlocks);
            return updated.ReplaceLineEndings(JsonFileFormatting.DetectNewLine(existingContent));
        }

        internal static string CreateNewFile(string content, Action<JObject> applySettings, char indentChar, int indentation)
        {
            var (cleanContent, commentBlocks) = JsonCommentPreserver.ExtractAndStrip(content);
            var json = JsonFileFormatting.Parse(cleanContent);

            applySettings(json);

            var serialized = JsonFileFormatting.Serialize(json, indentChar, indentation, Environment.NewLine);
            return JsonCommentPreserver.Restore(serialized, commentBlocks);
        }

        private void LogDroppedKeys()
        {
            if (_droppedKeysLogged || _droppedKeys.Count == 0)
            {
                return;
            }

            _droppedKeysLogged = true;
            var fileName = $"appsettings{(Model.RuntimeEnvironment != null ? $".{Model.RuntimeEnvironment.Name.ToPascalCase()}" : string.Empty)}.json";
            Logging.Log.Info(
                $"\"Generate AppSettings File\" is off for project \"{OutputTarget.GetProject()?.Name ?? OutputTarget.Name}\", " +
                $"so these settings were not added to its {fileName}: {string.Join(", ", _droppedKeys.Distinct())}");
        }

        private void ApplySettings(JObject json)
        {
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
                    ParseEditorConfigToSettings(editorConfig, filePath, settings);
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

        /// <summary>
        /// Applies the properties of every section of an <c>.editorconfig</c> whose glob matches
        /// <paramref name="filePath"/>, so e.g. a <c>[*.cs]</c> <c>indent_size</c> never reaches JSON.
        /// </summary>
        internal static void ParseEditorConfigToSettings(string content, string filePath, Dictionary<string, string> settings)
        {
            var lines = content.ReplaceLineEndings("\n").Split('\n');

            // Properties before the first section (the preamble, e.g. "root = true") apply to no file
            var sectionMatches = false;
            foreach (var line in lines.Select(l => l.Trim()))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith(';'))
                {
                    continue;
                }

                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    sectionMatches = EditorConfigSectionMatches(line[1..^1], filePath);
                    continue;
                }

                if (!sectionMatches)
                {
                    continue;
                }

                var keyValue = line.Split(['='], 2);
                if (keyValue.Length == 2)
                {
                    settings[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }
        }

        /// <summary>
        /// Matches an <c>.editorconfig</c> section glob (<c>*</c>, <c>**</c>, <c>?</c>, <c>[...]</c>,
        /// <c>{a,b}</c>) against a file. A glob without a <c>/</c> matches the file name in any folder.
        /// </summary>
        internal static bool EditorConfigSectionMatches(string glob, string filePath)
        {
            var normalizedPath = filePath.Replace('\\', '/');
            var pattern = new StringBuilder();
            var braceDepth = 0;

            for (var i = 0; i < glob.Length; i++)
            {
                var c = glob[i];
                switch (c)
                {
                    case '*' when i + 1 < glob.Length && glob[i + 1] == '*':
                        pattern.Append(".*");
                        i++;
                        break;
                    case '*':
                        pattern.Append("[^/]*");
                        break;
                    case '?':
                        pattern.Append("[^/]");
                        break;
                    case '[':
                        var closeIndex = glob.IndexOf(']', i + 1);
                        if (closeIndex < 0)
                        {
                            pattern.Append(@"\[");
                            break;
                        }

                        var characterClass = glob[(i + 1)..closeIndex];
                        pattern.Append('[')
                            .Append(characterClass.StartsWith('!') ? "^" + characterClass[1..] : characterClass)
                            .Append(']');
                        i = closeIndex;
                        break;
                    case '{':
                        braceDepth++;
                        pattern.Append("(?:");
                        break;
                    case '}' when braceDepth > 0:
                        braceDepth--;
                        pattern.Append(')');
                        break;
                    case ',' when braceDepth > 0:
                        pattern.Append('|');
                        break;
                    default:
                        pattern.Append(Regex.Escape(c.ToString()));
                        break;
                }
            }

            var target = glob.Contains('/') ? normalizedPath : Path.GetFileName(normalizedPath);
            var prefix = glob.Contains('/') ? "(?:^|.*/)" : "^";

            try
            {
                return Regex.IsMatch(target, $"{prefix}{pattern.ToString().TrimStart('/')}$", RegexOptions.IgnoreCase);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var (runtimeEnvironment, dependsOn) = !string.IsNullOrWhiteSpace(Model.RuntimeEnvironment?.Name)
                ? ($".{Model.RuntimeEnvironment.Name.ToPascalCase()}", "appsettings.json")
                : (null, null);

            var templateConfig = new TemplateFileConfig(
                    fileName: $"appsettings{runtimeEnvironment}",
                    fileExtension: "json",
                    relativeLocation: Model.Location);

            if (Model.IsHandsOff)
            {
                // A hands-off file's .csproj items are the developer's too: neither added nor removed
                templateConfig.CustomMetadata[FileAddedData.LeaveProjectItemsUnchangedKey] = "true";
            }
            else if (!Model.IncludeAllowHosts && !Model.IncludeAspNetCoreLoggingLevel && !Model.RequiresSpecifiedRole)
            {
                templateConfig.WithRemoveItemType("Content");
                templateConfig.WithCopyToOutputDirectory(CopyToOutputDirectory.CopyIfNewer);
            }

            templateConfig.WithDependsOn(dependsOn);

            return templateConfig;
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

            // Claimed even when hands-off, so it can't fall through to another project's file
            @event.MarkHandled();

            if (Model.IsHandsOff)
            {
                _droppedKeys.Add(@event.Key);
                return;
            }

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

            if (Model.IsHandsOff)
            {
                _droppedKeys.Add($"ConnectionStrings:{@event.Name}");
                return;
            }

            if (_connectionStrings.Any(x => x.Name == @event.Name && x.ConnectionString != @event.ConnectionString))
            {
                throw new Exception($"Misconfiguration in [{GetType().Name}]: ConnectionString with name [{@event.Name}] already defined with different value to [{@event.ConnectionString}].");
            }

            _connectionStrings.Add(@event);
        }
    }
}
