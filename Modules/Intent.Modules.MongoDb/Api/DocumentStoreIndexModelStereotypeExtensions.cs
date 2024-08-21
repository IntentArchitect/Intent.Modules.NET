using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.MongoDb.Api
{
    public static class DocumentStoreIndexModelStereotypeExtensions
    {
        public static Settings GetSettings(this DocumentStoreIndexModel model)
        {
            var stereotype = model.GetStereotype("a482271c-a3f0-4391-9c9f-98d06d71e85a");
            return stereotype != null ? new Settings(stereotype) : null;
        }


        public static bool HasSettings(this DocumentStoreIndexModel model)
        {
            return model.HasStereotype("a482271c-a3f0-4391-9c9f-98d06d71e85a");
        }

        public static bool TryGetSettings(this DocumentStoreIndexModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype("a482271c-a3f0-4391-9c9f-98d06d71e85a"));
            return true;
        }

        public class Settings
        {
            private IStereotype _stereotype;

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool UseDefaultName()
            {
                return _stereotype.GetProperty<bool>("Use Default Name");
            }

            public bool Unique()
            {
                return _stereotype.GetProperty<bool>("Unique");
            }

            public SortOrderOptions SortOrder()
            {
                return new SortOrderOptions(_stereotype.GetProperty<string>("Sort Order"));
            }

            public class SortOrderOptions
            {
                public readonly string Value;

                public SortOrderOptions(string value)
                {
                    Value = value;
                }

                public SortOrderOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Ascending":
                            return SortOrderOptionsEnum.Ascending;
                        case "Descending":
                            return SortOrderOptionsEnum.Descending;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAscending()
                {
                    return Value == "Ascending";
                }
                public bool IsDescending()
                {
                    return Value == "Descending";
                }
            }

            public enum SortOrderOptionsEnum
            {
                Ascending,
                Descending
            }
        }

    }
}