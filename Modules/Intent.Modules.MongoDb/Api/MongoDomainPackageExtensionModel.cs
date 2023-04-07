using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.MongoDb.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.MongoDb.Api
{
    [IntentManaged(Mode.Merge)]
    public class MongoDomainPackageExtensionModel : MongoDomainPackageModel
    {
        [IntentManaged(Mode.Ignore)]
        public MongoDomainPackageExtensionModel(IPackage package) : base(package)
        {
        }

    }
}