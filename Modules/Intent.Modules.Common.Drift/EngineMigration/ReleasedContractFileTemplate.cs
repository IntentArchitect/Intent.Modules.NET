using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// Stands in for a contract file on a machine whose <c>.intent/output-log.xml</c> still lists it, so the Software Factory
    /// counts the file as generated this run and doesn't propose deleting it. Overwrite disabled, and the file is already there,
    /// so the Software Factory never writes it: the change is locked to what is on disk. Not once-off, which would do the same but
    /// also record the file in the committed <c>output.config.xml</c>, where a later run reads it back as an ignored file. It
    /// carries the contract entry's correlation id, so the log keeps the entry as a contract one and <see cref="OutputLogPruning"/>
    /// still removes it; once it has, nothing registers this template for the file again.
    /// </summary>
    public class ReleasedContractFileTemplate : IntentTemplateBase<ReleasedContractFileTemplate.ReleasedFile>
    {
        public const string TemplateId = "Intent.Common.Drift.ReleasedContractFile";

        public ReleasedContractFileTemplate(IOutputTarget outputTarget, ReleasedFile model) : base(TemplateId, outputTarget, model)
        {
        }

        public override string GetCorrelationId() => Model.CorrelationId;

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var folder = Path.GetRelativePath(OutputTarget.Location, Path.GetDirectoryName(Model.FullPath)!);
            return new TemplateFileConfig(
                fileName: Path.GetFileNameWithoutExtension(Model.FullPath),
                fileExtension: Path.GetExtension(Model.FullPath).TrimStart('.'),
                relativeLocation: folder == "." ? string.Empty : folder,
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled);
        }

        /// <summary>The file as it is. The Software Factory reads the file itself for a change it can't write, so this is never written.</summary>
        public override string TransformText() => File.ReadAllText(Model.FullPath);

        /// <summary>A contract file: where it is, and the correlation id the log lists it under.</summary>
        public sealed record ReleasedFile(string FullPath, string CorrelationId);
    }
}
