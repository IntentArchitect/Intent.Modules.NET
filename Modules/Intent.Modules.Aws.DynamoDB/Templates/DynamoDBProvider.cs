using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Aws.DynamoDB.Templates
{
    internal class DynamoDBProvider
    {
        public const string Id = "ff0bb8e0-8fcc-4e0b-965d-bdc91d5081b2";

        internal static bool FilterDbProvider(ClassModel x)
        {
            if (!x.InternalElement.Package.HasStereotype("Document Database"))
            {
                return false;
            }

            var setting = x.InternalElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
            return setting == null || setting.Id == Id;
        }
    }
}
