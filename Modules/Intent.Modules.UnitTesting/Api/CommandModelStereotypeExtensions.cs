using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.UnitTesting.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static UnitTest GetUnitTest(this CommandModel model)
        {
            var stereotype = model.GetStereotype(UnitTest.DefinitionId);
            return stereotype != null ? new UnitTest(stereotype) : null;
        }


        public static bool HasUnitTest(this CommandModel model)
        {
            return model.HasStereotype(UnitTest.DefinitionId);
        }

        public static bool TryGetUnitTest(this CommandModel model, out UnitTest stereotype)
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