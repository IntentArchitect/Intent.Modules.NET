using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Repositories.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class RepositoryExtensionsModel : RepositoryModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<StoredProcedureModel> StoredProcedures => _element.ChildElements
            .GetElementsOfType(StoredProcedureModel.SpecializationTypeId)
            .Select(x => new StoredProcedureModel(x))
            .ToList();

    }
}