using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.AspNetCore.OutputCaching.Redis.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageExtensionModel : ServicesPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public CachingPoliciesModel CachingPolicies => UnderlyingPackage.ChildElements
            .GetElementsOfType(CachingPoliciesModel.SpecializationTypeId)
            .Select(x => new CachingPoliciesModel(x))
            .SingleOrDefault();

    }
}