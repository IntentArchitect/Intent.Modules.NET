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
        public static ExposeAsOData GetExposeAsOData(this ClassModel model)
        {
            var stereotype = model.GetStereotype(ExposeAsOData.DefinitionId);
            return stereotype != null ? new ExposeAsOData(stereotype) : null;
        }


        public static bool HasExposeAsOData(this ClassModel model)
        {
            return model.HasStereotype(ExposeAsOData.DefinitionId);
        }

        public static bool TryGetExposeAsOData(this ClassModel model, out ExposeAsOData stereotype)
        {
            if (!HasExposeAsOData(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ExposeAsOData(model.GetStereotype(ExposeAsOData.DefinitionId));
            return true;
        }

        public class ExposeAsOData
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "0b58c045-255d-4f09-8ad2-25e9496480f4";

            public ExposeAsOData(IStereotype stereotype)
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