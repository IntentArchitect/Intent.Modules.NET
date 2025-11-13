using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.UnitTesting.Api
{
    public static class IntegrationEventHandlerModelStereotypeExtensions
    {
        public static UnitTest GetUnitTest(this IntegrationEventHandlerModel model)
        {
            var stereotype = model.GetStereotype(UnitTest.DefinitionId);
            return stereotype != null ? new UnitTest(stereotype) : null;
        }


        public static bool HasUnitTest(this IntegrationEventHandlerModel model)
        {
            return model.HasStereotype(UnitTest.DefinitionId);
        }

        public static bool TryGetUnitTest(this IntegrationEventHandlerModel model, out UnitTest stereotype)
        {
            if (!HasUnitTest(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new UnitTest(model.GetStereotype(UnitTest.DefinitionId));
            return true;
        }

        public class UnitTest
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "4965bed2-6320-49d1-beba-0fbc6fd4dfe6";

            public UnitTest(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}