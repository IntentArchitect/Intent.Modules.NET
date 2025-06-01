using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.DiffAudit.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static AuditTable GetAuditTable(this ClassModel model)
        {
            var stereotype = model.GetStereotype(AuditTable.DefinitionId);
            return stereotype != null ? new AuditTable(stereotype) : null;
        }


        public static bool HasAuditTable(this ClassModel model)
        {
            return model.HasStereotype(AuditTable.DefinitionId);
        }

        public static bool TryGetAuditTable(this ClassModel model, out AuditTable stereotype)
        {
            if (!HasAuditTable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AuditTable(model.GetStereotype(AuditTable.DefinitionId));
            return true;
        }

        public static DiffAudit GetDiffAudit(this ClassModel model)
        {
            var stereotype = model.GetStereotype(DiffAudit.DefinitionId);
            return stereotype != null ? new DiffAudit(stereotype) : null;
        }


        public static bool HasDiffAudit(this ClassModel model)
        {
            return model.HasStereotype(DiffAudit.DefinitionId);
        }

        public static bool TryGetDiffAudit(this ClassModel model, out DiffAudit stereotype)
        {
            if (!HasDiffAudit(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DiffAudit(model.GetStereotype(DiffAudit.DefinitionId));
            return true;
        }

        public class AuditTable
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "92e433aa-4858-4129-849f-ef0f9f0bf9e6";

            public AuditTable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class DiffAudit
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "4c7ff86d-641f-431b-a20f-94b3ccbef265";

            public DiffAudit(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}