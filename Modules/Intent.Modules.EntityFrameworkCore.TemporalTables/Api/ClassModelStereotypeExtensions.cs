using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.TemporalTables.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static TemporalTable GetTemporalTable(this ClassModel model)
        {
            var stereotype = model.GetStereotype(TemporalTable.DefinitionId);
            return stereotype != null ? new TemporalTable(stereotype) : null;
        }


        public static bool HasTemporalTable(this ClassModel model)
        {
            return model.HasStereotype(TemporalTable.DefinitionId);
        }

        public static bool TryGetTemporalTable(this ClassModel model, out TemporalTable stereotype)
        {
            if (!HasTemporalTable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TemporalTable(model.GetStereotype(TemporalTable.DefinitionId));
            return true;
        }

        public class TemporalTable
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "abd54fa8-f51e-4cd9-b3ce-6512968c2d0c";

            public TemporalTable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string PeriodStartColumnName()
            {
                return _stereotype.GetProperty<string>("Period Start Column Name");
            }

            public string PeriodEndColumnName()
            {
                return _stereotype.GetProperty<string>("Period End Column Name");
            }

            public string HistoryTableName()
            {
                return _stereotype.GetProperty<string>("History Table Name");
            }

        }

    }
}