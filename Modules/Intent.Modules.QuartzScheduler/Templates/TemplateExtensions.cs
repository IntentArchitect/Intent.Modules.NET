using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.QuartzScheduler.Templates.QuartzConfiguration;
using Intent.Modules.QuartzScheduler.Templates.ScheduledJob;
using Intent.QuartzScheduler.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.QuartzScheduler.Templates
{
    public static class TemplateExtensions
    {
        public static string GetQuartzConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(QuartzConfigurationTemplate.TemplateId);
        }
        public static string GetScheduledJobName<T>(this IIntentTemplate<T> template) where T : ScheduledJobModel
        {
            return template.GetTypeName(ScheduledJobTemplate.TemplateId, template.Model);
        }

        public static string GetScheduledJobName(this IIntentTemplate template, ScheduledJobModel model)
        {
            return template.GetTypeName(ScheduledJobTemplate.TemplateId, model);
        }

    }
}