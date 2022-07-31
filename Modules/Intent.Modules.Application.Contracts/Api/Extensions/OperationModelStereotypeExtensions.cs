using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static Asynchronous GetAsynchronous(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Asynchronous");
            return stereotype != null ? new Asynchronous(stereotype) : null;
        }

        public static bool HasAsynchronous(this OperationModel model)
        {
            return model.HasStereotype("Asynchronous");
        }

        public static bool TryGetAsynchronous(this OperationModel model, out Asynchronous stereotype)
        {
            if (!HasAsynchronous(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Asynchronous(model.GetStereotype("Asynchronous"));
            return true;
        }

        public static Synchronous GetSynchronous(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Synchronous");
            return stereotype != null ? new Synchronous(stereotype) : null;
        }

        public static bool HasSynchronous(this OperationModel model)
        {
            return model.HasStereotype("Synchronous");
        }

        public static bool TryGetSynchronous(this OperationModel model, out Synchronous stereotype)
        {
            if (!HasSynchronous(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Synchronous(model.GetStereotype("Synchronous"));
            return true;
        }


        public class Asynchronous
        {
            private IStereotype _stereotype;

            public Asynchronous(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class Synchronous
        {
            private IStereotype _stereotype;

            public Synchronous(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}