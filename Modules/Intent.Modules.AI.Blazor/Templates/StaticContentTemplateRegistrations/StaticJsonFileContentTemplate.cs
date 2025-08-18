using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Intent.Modules.AI.Blazor.Templates.StaticContentTemplateRegistrations
{
    /// <summary>
    /// I did this to get the JSON file to work with our weaver. CodeGenType needs to be set. This is a work around for now, should be generically handled by the static content template
    /// Ideally the Static Content Template whould set this up by default, i.e. Static files are correctly set up for merging based on their file types.
    /// </summary>
    public class JsonStaticContentTemplate : IntentTemplateBase
    {
        private readonly string _sourcePath;
        private readonly string _relativeOutputPathPrefix;
        private readonly OverwriteBehaviour _overwriteBehaviour;
        private readonly IReadOnlyDictionary<string, string> _replacements;
        private readonly string _relativeOutputPath;

        /// <summary>
        /// Obsolete. Use <see cref="StaticContentTemplate(string,string,string,string,IOutputTarget,IReadOnlyDictionary{string,string},OverwriteBehaviour)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public JsonStaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements)
            : this(
                sourcePath: sourcePath,
                relativeOutputPath: relativeOutputPath,
                relativeOutputPathPrefix: null,
                templateId: templateId,
                outputTarget: outputTarget,
                replacements: replacements,
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled)
        {
        }

        /// <summary>
        /// Obsolete. Use <see cref="StaticContentTemplate(string,string,string,string,IOutputTarget,IReadOnlyDictionary{string,string},OverwriteBehaviour)"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public JsonStaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour)
            : this(
                sourcePath: sourcePath,
                relativeOutputPath: relativeOutputPath,
                relativeOutputPathPrefix: null,
                templateId: templateId,
                outputTarget: outputTarget,
                replacements: replacements,
                overwriteBehaviour: overwriteBehaviour)
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="StaticContentTemplate"/>.
        /// </summary>
        public JsonStaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string relativeOutputPathPrefix,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour) : base(templateId, outputTarget)
        {
            _sourcePath = sourcePath;
            _overwriteBehaviour = overwriteBehaviour;
            _replacements = replacements ?? new Dictionary<string, string>
            {
                ["ApplicationName"] = outputTarget.ApplicationName(),
                ["ApplicationNameAllLowerCase"] = outputTarget.ApplicationName().ToLowerInvariant()
            };

            if (!string.IsNullOrWhiteSpace(relativeOutputPathPrefix))
            {
                relativeOutputPath = Path.Join(relativeOutputPathPrefix, relativeOutputPath);
                _relativeOutputPathPrefix = relativeOutputPathPrefix;
            }

            _relativeOutputPath = relativeOutputPath.NormalizePath();

        }

        /// <inheritdoc />
        public override string TransformText()
        {
            var result = File.ReadAllText(_sourcePath);

            foreach (var (searchFor, replaceWith) in _replacements)
            {
                result = result.Replace($"<#= {searchFor} #>", replaceWith);
            }

            return result;
        }

        /// <inheritdoc />
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var config = new TemplateFileConfig(
                fileName: Path.GetFileNameWithoutExtension(_relativeOutputPath),
                fileExtension: Path.GetExtension(_relativeOutputPath)?.TrimStart('.') ?? string.Empty,
                relativeLocation: Path.GetDirectoryName(_relativeOutputPath),
                overwriteBehaviour: _overwriteBehaviour,
                codeGenType: "JsonMerger");

            if (_relativeOutputPathPrefix != null)
            {
                config.CustomMetadata.Add("RelativeOutputPathPrefix", _relativeOutputPathPrefix);
            }

            return config;
        }

        /// <inheritdoc />
        public override string GetCorrelationId()
        {
            return $"{Id}#{_relativeOutputPath}";
        }
    }
}
