using System;
using System.IO;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution.Merging;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    /// <summary>
    /// Reconciles <see cref="VisualStudioSolutionSlnxTemplate"/>'s freshly-generated .slnx content
    /// with whatever is already on disk. The template itself has no knowledge of the existing file;
    /// this weaver is where renames/moves are detected and manual edits are preserved, using
    /// <see cref="SlnxMerger"/>.
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class VisualStudioSolutionSlnxWeaver : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.VisualStudio.Projects.VisualStudioSolutionSlnxWeaver";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        // VisualStudioSolutionTemplate (classic .sln) and VisualStudioSolutionSlnxTemplate share the
        // same Id by design, so switching format replaces the same logical output instead of Intent
        // seeing two unrelated files - which means Id alone can't tell them apart here. Match on the
        // actual template type instead (the same disambiguation VisualStudioSolutionSyncProcessor
        // already uses), otherwise this would try to parse classic .sln content as XML.
        public bool CanTransform(IOutputFile output) =>
            output.Template is VisualStudioSolutionSlnxTemplate;

        public void Transform(IOutputFile output)
        {
            var generated = output.Content;

            // VisualStudioSolutionTemplate and VisualStudioSolutionSlnxTemplate share the same output
            // Id, so PreviousFilePath still resolves to the OLD file's location when the model was just
            // switched from .sln to .slnx (or back) - but that file's content is a completely different,
            // non-XML format that has nothing to do with this template's own merge history. Treat that
            // case as if there is no history at all, rather than feeding cross-format content into the
            // .slnx-specific merge logic.
            var isFormatSwitch = !string.Equals(
                Path.GetExtension(output.PreviousFilePath),
                Path.GetExtension(output.TargetFilePath),
                StringComparison.OrdinalIgnoreCase);

            var existing = isFormatSwitch ? null : output.GetPreviousFilePathContent();
            var previousOutput = isFormatSwitch ? null : output.GetPreviousTemplateOutput();

            // The merge only ever rearranges/preserves entries already present in Existing or
            // Generated - it never discards user content - so its output is never destructive.
            output.SetHasDestructiveChanges(HasDestructiveChanges.False);
            output.ChangeContent(SlnxMerger.Merge(generated, existing, previousOutput));
        }
    }
}
