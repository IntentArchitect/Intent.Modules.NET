using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.CQRS.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    public static class ScheduledCommandPublishModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<ScheduledCommandPublishTargetEndModel> ScheduledCommandPublishTargets(this HangfireJobModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ScheduledCommandPublishModel.SpecializationType && x.IsTargetEnd())
                .Select(x => ScheduledCommandPublishModel.CreateFromEnd(x).TargetEnd)
                .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public static IList<ScheduledCommandPublishSourceEndModel> HangfireJobs(this CommandModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ScheduledCommandPublishModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ScheduledCommandPublishModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}