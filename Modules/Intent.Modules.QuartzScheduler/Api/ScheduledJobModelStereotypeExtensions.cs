using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.QuartzScheduler.Api
{
    public static class ScheduledJobModelStereotypeExtensions
    {
        public static Scheduling GetScheduling(this ScheduledJobModel model)
        {
            var stereotype = model.GetStereotype("b7a34ab9-8f7f-415f-92bd-c85cfe02f29d");
            return stereotype != null ? new Scheduling(stereotype) : null;
        }


        public static bool HasScheduling(this ScheduledJobModel model)
        {
            return model.HasStereotype("b7a34ab9-8f7f-415f-92bd-c85cfe02f29d");
        }

        public static bool TryGetScheduling(this ScheduledJobModel model, out Scheduling stereotype)
        {
            if (!HasScheduling(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Scheduling(model.GetStereotype("b7a34ab9-8f7f-415f-92bd-c85cfe02f29d"));
            return true;
        }

        public class Scheduling
        {
            private IStereotype _stereotype;

            public Scheduling(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool Enabled()
            {
                return _stereotype.GetProperty<bool>("Enabled");
            }

            public string CronSchedule()
            {
                return _stereotype.GetProperty<string>("Cron Schedule");
            }

            public bool DisallowConcurrentExecution()
            {
                return _stereotype.GetProperty<bool>("Disallow Concurrent Execution");
            }

        }

    }
}