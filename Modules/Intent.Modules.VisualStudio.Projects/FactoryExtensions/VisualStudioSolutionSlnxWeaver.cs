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
            var existing = output.GetPreviousFilePathContent();
            var previousOutput = output.GetPreviousTemplateOutput();

            var result = SlnxMerger.Merge(generated, existing, previousOutput);

            output.SetHasDestructiveChanges(result.HasDestructiveChanges);
            output.ChangeContent(result.Content);
        }
    }
}
