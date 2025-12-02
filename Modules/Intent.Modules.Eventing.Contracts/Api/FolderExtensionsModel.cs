using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Eventing.Contracts.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderExtensionsModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderExtensionsModel(IElement element) : base(element)
        {
        }

        public IList<MessageBusProviderModel> MessageBusProviders => _element.ChildElements
            .GetElementsOfType(MessageBusProviderModel.SpecializationTypeId)
            .Select(x => new MessageBusProviderModel(x))
            .ToList();

    }
}