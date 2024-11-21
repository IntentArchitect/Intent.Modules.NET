using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Entities.BasicAuditing.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static BasicAuditing GetBasicAuditing(this ClassModel model)
        {
            var stereotype = model.GetStereotype(BasicAuditing.DefinitionId);
            return stereotype != null ? new BasicAuditing(stereotype) : null;
        }


        public static bool HasBasicAuditing(this ClassModel model)
        {
            return model.HasStereotype(BasicAuditing.DefinitionId);
        }

        public static bool TryGetBasicAuditing(this ClassModel model, out BasicAuditing stereotype)
        {
            if (!HasBasicAuditing(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new BasicAuditing(model.GetStereotype(BasicAuditing.DefinitionId));
            return true;
        }

        public class BasicAuditing
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "796ec2fb-cdb9-4f8e-b096-a4f72c7a7f93";

            public BasicAuditing(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}