using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.OData.EntityFramework.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static OData GetOData(this ClassModel model)
        {
            var stereotype = model.GetStereotype(OData.DefinitionId);
            return stereotype != null ? new OData(stereotype) : null;
        }


        public static bool HasOData(this ClassModel model)
        {
            return model.HasStereotype(OData.DefinitionId);
        }

        public static bool TryGetOData(this ClassModel model, out OData stereotype)
        {
            if (!HasOData(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new OData(model.GetStereotype(OData.DefinitionId));
            return true;
        }

        public class OData
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "0b58c045-255d-4f09-8ad2-25e9496480f4";

            public OData(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool GetAll()
            {
                return _stereotype.GetProperty<bool>("GetAll");
            }

            public bool GetById()
            {
                return _stereotype.GetProperty<bool>("GetById");
            }

            public bool Post()
            {
                return _stereotype.GetProperty<bool>("Post");
            }

            public bool Delete()
            {
                return _stereotype.GetProperty<bool>("Delete");
            }

            public bool Patch()
            {
                return _stereotype.GetProperty<bool>("Patch");
            }

            public bool Put()
            {
                return _stereotype.GetProperty<bool>("Put");
            }

        }

    }
}