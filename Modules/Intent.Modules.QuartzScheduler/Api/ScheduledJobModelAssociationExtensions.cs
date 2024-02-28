using Intent.Modelers.Services.CQRS.Api;
using Intent.QuartzScheduler.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.QuartzScheduler.Api
{
	public static class ScheduledJobModelAssociationExtensions
	{
		public static CommandModel? PublishedCommand(this ScheduledJobModel model)
		{
			var end = model
						.InternalElement
						.AssociatedElements
						.FirstOrDefault(x => x.Association.SpecializationType == ScheduledCommandPublishModel.SpecializationType && x.IsTargetEnd());
			return end?.TypeReference?.Element.AsCommandModel();
		}
	}
}
