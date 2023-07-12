using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.BasicAudit.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static BasicAudit GetBasicAudit(this ClassModel model)
        {
            var stereotype = model.GetStereotype("Basic Audit");
            return stereotype != null ? new BasicAudit(stereotype) : null;
        }


        public static bool HasBasicAudit(this ClassModel model)
        {
            return model.HasStereotype("Basic Audit");
        }

        public static bool TryGetBasicAudit(this ClassModel model, out BasicAudit stereotype)
        {
            if (!HasBasicAudit(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new BasicAudit(model.GetStereotype("Basic Audit"));
            return true;
        }

        public class BasicAudit
        {
            private IStereotype _stereotype;

            public BasicAudit(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}