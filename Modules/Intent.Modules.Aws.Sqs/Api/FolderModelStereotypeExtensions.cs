using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Aws.Sqs.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static AwsSqsFolderSettings GetAwsSqsFolderSettings(this FolderModel model)
        {
            var stereotype = model.GetStereotype(AwsSqsFolderSettings.DefinitionId);
            return stereotype != null ? new AwsSqsFolderSettings(stereotype) : null;
        }


        public static bool HasAwsSqsFolderSettings(this FolderModel model)
        {
            return model.HasStereotype(AwsSqsFolderSettings.DefinitionId);
        }

        public static bool TryGetAwsSqsFolderSettings(this FolderModel model, out AwsSqsFolderSettings stereotype)
        {
            if (!HasAwsSqsFolderSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AwsSqsFolderSettings(model.GetStereotype(AwsSqsFolderSettings.DefinitionId));
            return true;
        }

        public class AwsSqsFolderSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "721c45f1-add5-4143-8ef7-c6f54db6e06d";

            public AwsSqsFolderSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}