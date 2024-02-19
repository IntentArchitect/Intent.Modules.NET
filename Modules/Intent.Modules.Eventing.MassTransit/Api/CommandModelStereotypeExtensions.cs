using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static MassTransitConsumer GetMassTransitConsumer(this CommandModel model)
        {
            var stereotype = model.GetStereotype("b37b2302-97f4-4bc0-b3ad-bc04894c305a");
            return stereotype != null ? new MassTransitConsumer(stereotype) : null;
        }


        public static bool HasMassTransitConsumer(this CommandModel model)
        {
            return model.HasStereotype("b37b2302-97f4-4bc0-b3ad-bc04894c305a");
        }

        public static bool TryGetMassTransitConsumer(this CommandModel model, out MassTransitConsumer stereotype)
        {
            if (!HasMassTransitConsumer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitConsumer(model.GetStereotype("b37b2302-97f4-4bc0-b3ad-bc04894c305a"));
            return true;
        }

        public class MassTransitConsumer
        {
            private IStereotype _stereotype;

            public MassTransitConsumer(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}