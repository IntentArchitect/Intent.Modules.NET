using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Hangfire.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<HangfireQueueModel> Queues => _element.ChildElements
            .GetElementsOfType(HangfireQueueModel.SpecializationTypeId)
            .Select(x => new HangfireQueueModel(x))
            .ToList();

        public IList<HangfireJobModel> Jobs => _element.ChildElements
            .GetElementsOfType(HangfireJobModel.SpecializationTypeId)
            .Select(x => new HangfireJobModel(x))
            .ToList();

    }
}