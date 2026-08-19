using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Blazor.Api
{
    [IntentMerge]
    public static class ComponentModelStereotypeExtensions
    {
        public static RenderOnServer GetRenderOnServer(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(RenderOnServer.DefinitionId);
            return stereotype != null ? new RenderOnServer(stereotype) : null;
        }


        public static bool HasRenderOnServer(this ComponentModel model)
        {
            return model.HasStereotype(RenderOnServer.DefinitionId);
        }

        public static bool TryGetRenderOnServer(this ComponentModel model, out RenderOnServer stereotype)
        {
            if (!HasRenderOnServer(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RenderOnServer(model.GetStereotype(RenderOnServer.DefinitionId));
            return true;
        }

        public class RenderOnServer
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fbf946ea-47cc-486b-b15f-aaa58497b819";

            public RenderOnServer(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public static Secured GetSecured(this ComponentModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static IReadOnlyCollection<Secured> GetSecureds(this ComponentModel model)
        {
            var stereotypes = model
                .GetStereotypes(Secured.DefinitionId)
                .Select(stereotype => new Secured(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasSecured(this ComponentModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        public static bool TryGetSecured(this ComponentModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        public class Secured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";

            public Secured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

        }

    }
}