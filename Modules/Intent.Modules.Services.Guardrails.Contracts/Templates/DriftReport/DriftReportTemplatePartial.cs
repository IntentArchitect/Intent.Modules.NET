using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Services.Guardrails.Contracts.Drift;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.DriftReport
{
    /// <summary>
    /// The Services designer's drift check. The check is the drift module's (<see cref="DriftReportRunner"/>): this template
    /// contributes the Services designer to it (<see cref="ServicesDrift"/>) and writes the designer's report. Its signature
    /// is merged rather than fully managed, so it keeps <see cref="IDriftDesignerSource"/>.
    /// </summary>
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class DriftReportTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate, IDriftDesignerSource
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.DriftReport";

        [IntentManaged(Mode.Ignore)]
        private readonly DriftReportRunner _runner;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DriftReportTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = false;
            _runner = new DriftReportRunner(this, ServicesDrift.Designer);
            MarkdownFile = _runner.ReportFile;
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

        [IntentManaged(Mode.Ignore)]
        public DriftDesigner Designer => ServicesDrift.Designer;

        /// <summary>The report is only written when it needs attention (<see cref="DriftReportRunner.CanRun"/>).</summary>
        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            return _runner.CanRun && base.CanRunTemplate();
        }
    }
}
