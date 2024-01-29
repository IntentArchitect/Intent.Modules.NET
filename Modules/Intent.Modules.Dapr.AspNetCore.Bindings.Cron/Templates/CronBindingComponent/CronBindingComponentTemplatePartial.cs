using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Dapr.AspNetCore.Bindings.Cron.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Bindings.Cron.Templates.CronBindingComponent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CronBindingComponentTemplate : IntentTemplateBase<CommandModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Dapr.AspNetCore.Bindings.Cron.CronBindingComponent";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CronBindingComponentTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var config = new TemplateFileConfig(
                fileName: $"bindings.cron.{Name.ToKebabCase().Replace("/", "-").Replace("\\", "-")}",
                fileExtension: "yml",
                relativeLocation: "dapr/components"
            );

            config.CustomMetadata.Add("RelativeOutputPathPrefix", "dapr/components");

            return config;
        }

        private string Name => Model.GetStereotypeProperty<string>("Http Settings", "Route");
        private string Schedule => Model.GetDaprCronBinding().Schedule();
    }
}