using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServicePackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public ServicePackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public HangfireConfigurationModel Hangfire => UnderlyingPackage.ChildElements
            .GetElementsOfType(HangfireConfigurationModel.SpecializationTypeId)
            .Select(x => new HangfireConfigurationModel(x))
            .SingleOrDefault();

    }
}