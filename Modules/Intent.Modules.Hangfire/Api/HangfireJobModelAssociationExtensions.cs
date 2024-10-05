using Intent.Modelers.Services.CQRS.Api;
using System.Linq;

namespace Intent.Modules.Hangfire.Api;
public static class HangfireJobModelAssociationExtensions
{
    public static CommandModel? PublishedCommand(this HangfireJobModel model)
    {
        var end = model
                    .InternalElement
                    .AssociatedElements
                    .FirstOrDefault(x => x.Association.SpecializationType == ScheduledCommandPublishModel.SpecializationType && x.IsTargetEnd());
        return end?.TypeReference?.Element.AsCommandModel();
    }
}
