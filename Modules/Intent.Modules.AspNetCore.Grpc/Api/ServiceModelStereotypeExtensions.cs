using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Grpc.Api
{
    public static class ServiceModelStereotypeExtensions
    {
        public static ExposeWithGRPC GetExposeWithGRPC(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(ExposeWithGRPC.DefinitionId);
            return stereotype != null ? new ExposeWithGRPC(stereotype) : null;
        }


        public static bool HasExposeWithGRPC(this ServiceModel model)
        {
            return model.HasStereotype(ExposeWithGRPC.DefinitionId);
        }

        public static bool TryGetExposeWithGRPC(this ServiceModel model, out ExposeWithGRPC stereotype)
        {
            if (!HasExposeWithGRPC(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ExposeWithGRPC(model.GetStereotype(ExposeWithGRPC.DefinitionId));
            return true;
        }

        public class ExposeWithGRPC
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "39863d70-29fe-44df-985d-d28fc00b4a96";

            public ExposeWithGRPC(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}