using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Dapr.AspNetCore.StateManagement.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionModel : DomainPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<StateStoreModel> StateStores => UnderlyingPackage.ChildElements
            .GetElementsOfType(StateStoreModel.SpecializationTypeId)
            .Select(x => new StateStoreModel(x))
            .ToList();

    }
}