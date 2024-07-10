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
            var stereotype = model.GetStereotype("a321ab87-a4a2-4a09-a9ca-2cb6ecd758be");
            return stereotype != null ? new ODataQuery(stereotype) : null;
        }


        public static bool HasODataQuery(this QueryModel model)
        {
            return model.HasStereotype("a321ab87-a4a2-4a09-a9ca-2cb6ecd758be");
        }

        public static bool TryGetODataQuery(this QueryModel model, out ODataQuery stereotype)
        {
            if (!HasODataQuery(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ODataQuery(model.GetStereotype("a321ab87-a4a2-4a09-a9ca-2cb6ecd758be"));
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