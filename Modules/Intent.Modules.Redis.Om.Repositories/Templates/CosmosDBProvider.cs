using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Redis.Om.Repositories.Templates
{
    internal class RedisOmProvider
    {
        public const string Id = "3e1a00f7-c6f1-4785-a544-bbcb17602b31";

        internal static bool FilterDbProvider(ClassModel x)
        {
            if (!x.InternalElement.Package.HasStereotype("Document Database"))
                return false;
            var setting = x.InternalElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
            return setting == null ||
                setting.Id == Id;
        }
    }
}
