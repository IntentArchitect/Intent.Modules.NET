using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Dapper.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class RepositoryExtensionsModel : RepositoryModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryExtensionsModel(IElement element) : base(element)
        {
        }

    }
}