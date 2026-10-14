using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Domain.Guardrails.Contracts.Api
{
    public static class ClassConstructorModelStereotypeExtensions
    {
        public static PrimaryConstructor GetPrimaryConstructor(this ClassConstructorModel model)
        {
            var stereotype = model.GetStereotype(PrimaryConstructor.DefinitionId);
            return stereotype != null ? new PrimaryConstructor(stereotype) : null;
        }


        public static bool HasPrimaryConstructor(this ClassConstructorModel model)
        {
            return model.HasStereotype(PrimaryConstructor.DefinitionId);
        }

        public static bool TryGetPrimaryConstructor(this ClassConstructorModel model, out PrimaryConstructor stereotype)
        {
            if (!HasPrimaryConstructor(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PrimaryConstructor(model.GetStereotype(PrimaryConstructor.DefinitionId));
            return true;
        }

        public class PrimaryConstructor
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d95d7c59-fe11-4f0a-84a9-d17a8c097cc1";

            public PrimaryConstructor(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}