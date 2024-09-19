using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Hangfire.Api;
using Intent.Modules.Hangfire.Templates.HangfireConfiguration;
using Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter;
using Intent.Modules.Hangfire.Templates.HangfireJobs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHangfireConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(HangfireConfigurationTemplate.TemplateId);
        }

        public static string GetHangfireDashboardAuthFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(HangfireDashboardAuthFilterTemplate.TemplateId);
        }

        public static string GetHangfireJobsName<T>(this IIntentTemplate<T> template) where T : HangfireJobModel
        {
            return template.GetTypeName(HangfireJobsTemplate.TemplateId, template.Model);
        }

        public static string GetHangfireJobsName(this IIntentTemplate template, HangfireJobModel model)
        {
            return template.GetTypeName(HangfireJobsTemplate.TemplateId, model);
        }

    }
}