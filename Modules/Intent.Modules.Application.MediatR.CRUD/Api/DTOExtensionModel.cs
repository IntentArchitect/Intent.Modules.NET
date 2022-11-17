using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Application.MediatR.CRUD.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DTOExtensionModel : DTOModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DTOExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class DTOExtensionModelExtensions
    {

        public static bool HasProjectToDomainMapping(this DTOModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "942eae46-49f1-450e-9274-a92d40ac35fa";
        }

        public static IElementMapping GetProjectToDomainMapping(this DTOModel type)
        {
            return type.HasProjectToDomainMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}