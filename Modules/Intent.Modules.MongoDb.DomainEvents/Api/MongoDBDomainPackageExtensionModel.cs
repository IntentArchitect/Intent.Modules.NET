using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.MongoDb.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.MongoDb.DomainEvents.Api
{
    [IntentManaged(Mode.Merge)]
    public class MongoDBDomainPackageExtensionModel : MongoDomainPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public MongoDBDomainPackageExtensionModel(IPackage package) : base(package)
        {
        }

        [IntentManaged(Mode.Fully)]
        public IList<DomainEventModel> DomainEvents => UnderlyingPackage.ChildElements
            .GetElementsOfType(DomainEventModel.SpecializationTypeId)
            .Select(x => new DomainEventModel(x))
            .ToList();

    }
}