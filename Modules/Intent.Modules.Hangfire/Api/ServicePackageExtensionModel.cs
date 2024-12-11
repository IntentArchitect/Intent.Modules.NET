using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Hangfire.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServicePackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServicePackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<HangfireJobModel> Jobs => UnderlyingPackage.ChildElements
            .GetElementsOfType(HangfireJobModel.SpecializationTypeId)
            .Select(x => new HangfireJobModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public IList<HangfireQueueModel> Queues => UnderlyingPackage.ChildElements
            .GetElementsOfType(HangfireQueueModel.SpecializationTypeId)
            .Select(x => new HangfireQueueModel(x))
            .ToList();

    }
}