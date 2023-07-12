using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modelers.WebClient.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Blazor.HttpClients.Api
{
    [IntentManaged(Mode.Merge)]
    public class PackageServiceProxyExtensionsModel : WebClientModel
    {
        [IntentManaged(Mode.Ignore)]
        public PackageServiceProxyExtensionsModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<ServiceProxyModel> ServiceProxies => UnderlyingPackage.ChildElements
            .GetElementsOfType(ServiceProxyModel.SpecializationTypeId)
            .Select(x => new ServiceProxyModel(x))
            .ToList();

    }
}