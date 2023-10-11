using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Azure.TableStorage.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static TableSettings GetTableSettings(this ClassModel model)
        {
            var stereotype = model.GetStereotype("TableSettings");
            return stereotype != null ? new TableSettings(stereotype) : null;
        }


        public static bool HasTableSettings(this ClassModel model)
        {
            return model.HasStereotype("TableSettings");
        }

        public static bool TryGetTableSettings(this ClassModel model, out TableSettings stereotype)
        {
            if (!HasTableSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TableSettings(model.GetStereotype("TableSettings"));
            return true;
        }

        public class TableSettings
        {
            private IStereotype _stereotype;

            public TableSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string PartitionKey()
            {
                return _stereotype.GetProperty<string>("PartitionKey");
            }

            public string ETag()
            {
                return _stereotype.GetProperty<string>("ETag");
            }

        }

    }
}