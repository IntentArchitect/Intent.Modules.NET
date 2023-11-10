using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.ODataQuery.Api
{
    public static class QueryModelStereotypeExtensions
    {
        public static ODataQuery GetODataQuery(this QueryModel model)
        {
            var stereotype = model.GetStereotype("ODataQuery");
            return stereotype != null ? new ODataQuery(stereotype) : null;
        }


        public static bool HasODataQuery(this QueryModel model)
        {
            return model.HasStereotype("ODataQuery");
        }

        public static bool TryGetODataQuery(this QueryModel model, out ODataQuery stereotype)
        {
            if (!HasODataQuery(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ODataQuery(model.GetStereotype("ODataQuery"));
            return true;
        }

        public class ODataQuery
        {
            private IStereotype _stereotype;

            public ODataQuery(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool EnableSelect()
            {
                return _stereotype.GetProperty<bool>("Enable Select");
            }
        }

    }
}