using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Redis.Om.Repositories.Api
{
    //Disambiguation
    using Intent.Modelers.Domain.Api;

    public static class AttributeModelStereotypeExtensions
    {
        public static FieldSetting GetFieldSetting(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(FieldSetting.DefinitionId);
            return stereotype != null ? new FieldSetting(stereotype) : null;
        }


        public static bool HasFieldSetting(this AttributeModel model)
        {
            return model.HasStereotype(FieldSetting.DefinitionId);
        }

        public static bool TryGetFieldSetting(this AttributeModel model, out FieldSetting stereotype)
        {
            if (!HasFieldSetting(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FieldSetting(model.GetStereotype(FieldSetting.DefinitionId));
            return true;
        }

        public static Indexed GetIndexed(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Indexed.DefinitionId);
            return stereotype != null ? new Indexed(stereotype) : null;
        }


        public static bool HasIndexed(this AttributeModel model)
        {
            return model.HasStereotype(Indexed.DefinitionId);
        }

        public static bool TryGetIndexed(this AttributeModel model, out Indexed stereotype)
        {
            if (!HasIndexed(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Indexed(model.GetStereotype(Indexed.DefinitionId));
            return true;
        }

        public static Searchable GetSearchable(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Searchable.DefinitionId);
            return stereotype != null ? new Searchable(stereotype) : null;
        }


        public static bool HasSearchable(this AttributeModel model)
        {
            return model.HasStereotype(Searchable.DefinitionId);
        }

        public static bool TryGetSearchable(this AttributeModel model, out Searchable stereotype)
        {
            if (!HasSearchable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Searchable(model.GetStereotype(Searchable.DefinitionId));
            return true;
        }

        public class FieldSetting
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b92797e6-52ec-464e-80d3-2274cc97a1a1";

            public FieldSetting(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

        }

        public class Indexed
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "e1fc703d-389d-4241-aee1-ee6ce385bbc0";

            public Indexed(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class Searchable
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "0f333544-56ed-499e-8ba2-f856025de6ef";

            public Searchable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}