using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Aws.DynamoDB.Api
{
    public static class AttributeModelStereotypeExtensions
    {
        public static Version GetVersion(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Version.DefinitionId);
            return stereotype != null ? new Version(stereotype) : null;
        }


        public static bool HasVersion(this AttributeModel model)
        {
            return model.HasStereotype(Version.DefinitionId);
        }

        public static bool TryGetVersion(this AttributeModel model, out Version stereotype)
        {
            if (!HasVersion(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Version(model.GetStereotype(Version.DefinitionId));
            return true;
        }

        public class Version
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7cb4b43e-c0b2-465a-9d12-fbf184ebdb69";

            public Version(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}