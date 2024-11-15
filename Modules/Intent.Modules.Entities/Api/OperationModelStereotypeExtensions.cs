using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Entities.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static Asynchronous GetAsynchronous(this OperationModel model)
        {
            var stereotype = model.GetStereotype(Asynchronous.DefinitionId);
            return stereotype != null ? new Asynchronous(stereotype) : null;
        }


        public static bool HasAsynchronous(this OperationModel model)
        {
            return model.HasStereotype(Asynchronous.DefinitionId);
        }

        public static bool TryGetAsynchronous(this OperationModel model, out Asynchronous stereotype)
        {
            if (!HasAsynchronous(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Asynchronous(model.GetStereotype(Asynchronous.DefinitionId));
            return true;
        }

        public class Asynchronous
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "F8E749E2-426F-4E0A-AD03-86BFFE122740";

            public Asynchronous(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}