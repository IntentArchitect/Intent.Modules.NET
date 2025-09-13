using Anthropic.SDK.Messaging;
using Intent.Engine;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Modules.AI.Blazor.Tasks.Helpers;
using Intent.Templates;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks.Config
{
    public class PromptConfig
    {
        internal const string DefaultConfigFileName = "prompt-config.json";

        [JsonIgnore]
        public string FilePath { get; set; }

        [JsonPropertyName("metadata")]
        [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("rules")]
        public List<string> Rules { get; set; } = new List<string>();

        [JsonPropertyName("input-files")]
        public List<InputFile> InputFiles { get; set; } = new List<InputFile>();

        [JsonPropertyName("templates")]
        public List<Template> Templates { get; set; } = new List<Template>();
        [JsonPropertyName("mcp-servers")]
        public List<McpServer> McpServers { get; set; } = new();

        internal static string GetTemplatePromptPath(string solutionPath, string applicationName)
        { 
            return Path.Combine(solutionPath, "AI.Prompt.Templates", applicationName, "Intent.Blazor.AI");
        }

        internal static PromptConfig? TryLoad(string solutionPath, string applicationName)
        {
            var path = GetTemplatePromptPath(solutionPath, applicationName);
            var filename = System.IO.Directory.Exists(path) ? Path.Combine(path, DefaultConfigFileName) : path;
            if (System.IO.File.Exists(filename))
            {
                return Load(filename);
            }
            return null;
        }

        private static PromptConfig Load(string configFilePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            options.Converters.Add(new DictionaryStringObjectJsonConverter());

            string json = System.IO.File.ReadAllText(configFilePath);
            var config = JsonSerializer.Deserialize<PromptConfig>(json, options)
                         ?? throw new InvalidOperationException("Config deserialized to null.");

            // Store the config file directory in memory for later path resolution (if you want)
            config.FilePath = Path.GetDirectoryName(Path.GetFullPath(configFilePath))!;

            return config;
        }

        public Dictionary<string, object> GetMetadata(string templateId)
        {
            var template = Templates.FirstOrDefault(t => t.Id == templateId);
            if (template == null)
                return Metadata;
            return DictionaryHelper.MergeDictionaries(Metadata, template.Metadata);
        }

        internal IEnumerable<string> GetInputFile(string templateId)
        {
            var template = Templates.FirstOrDefault(t => t.Id == templateId);
            if (string.IsNullOrWhiteSpace(templateId) || template == null)
                return InputFiles.Select(f => f.Filename);
            return InputFiles.Select(f => f.Filename)
                    .Concat(template.InputFiles.Select(f => f.Filename));
        }

        public IEnumerable<ICodebaseFile> LoadTemplateFiles(string templateId)
        {
            var template = Templates.FirstOrDefault(t => t.Id == templateId);
            if (string.IsNullOrEmpty(FilePath) || template is null)
            {
                return [];
            }
            var folderPath = Path.Combine(FilePath, template.TemplateFolder);

            if (!System.IO.Directory.Exists(folderPath))
                throw new System.IO.DirectoryNotFoundException($"Template folder not found: {folderPath}");

            var files = System.IO.Directory.GetFiles(folderPath, "*.*", System.IO.SearchOption.TopDirectoryOnly);

            return files.Select(f => new PromptFile(System.IO.File.ReadAllText(f))).ToList();
        }

        internal string GetAdditionalRules(string templateId)
        {
            var result = new StringBuilder();
            var template = Templates.FirstOrDefault(t => t.Id == templateId);

            foreach (var rule in Rules)
            {
                result.AppendLine($"* {rule}");
            }
            if (template is not null)
            {
                foreach (var rule in template.Rules)
                {
                    result.AppendLine($"* {rule}");
                }
            }
            return result.ToString();
        }

    }

    public class InputFile
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("file-name")]
        public string Filename { get; set; }
    }

    public class Template
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("applicability")]
        public TemplateMatch Match { get; set; } = new TemplateMatch();

        [JsonPropertyName("template-folder")]
        public string TemplateFolder { get; set; }

        /// <summary>
        /// If you have options in you template and you want to user to give specific info for the template.
        /// </summary>
        [JsonPropertyName("default-user-prompt")]
        public string DefaultUserPrompt { get; set; }

        [JsonPropertyName("metadata")]
        [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        [JsonPropertyName("rules")]
        public List<string> Rules { get; set; } = new List<string>();

        [JsonPropertyName("input-files")]
        public List<InputFile> InputFiles { get; set; } = new List<InputFile>();

    }

    public class KeywordRule
    {
        [JsonPropertyName("word")]
        public string Word { get; set; } = "";   // exact token match, case-insensitive (e.g., "add")
        [JsonPropertyName("weight")]
        public double Weight { get; set; } = 1;
    }

    public class TemplateMatch
    {
        [JsonPropertyName("key-words")]
        public List<KeywordRule> Keywords { get; set; } = new();
        [JsonPropertyName("negatives")]
        public List<KeywordRule> Negatives { get; set; } = new();
        [JsonPropertyName("priority")]
        public int Priority { get; set; } = 0; // used only to break ties deterministically
    }

    public class McpServer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("transport")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public McpTransport Transport { get; set; } = McpTransport.Process;

        [JsonPropertyName("command")]
        public string? Command { get; set; }

        [JsonPropertyName("args")]
        public List<string>? Args { get; set; }

        [JsonPropertyName("workingDirectory")]
        public string? WorkingDirectory { get; set; }

        [JsonPropertyName("env")]
        public Dictionary<string, string>? Env { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("group")]
        public string? Group { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("toolFilter")]
        public List<string>? ToolFilter { get; set; }
    }

    public enum McpTransport
    {
        Process,
        Sse
    }
}
