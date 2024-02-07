using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Redis.Om.Repositories.Templates
{
    internal class RedisOmProvider
    {
        public const string Id = "cdafe291-b62d-4014-9f94-5b8d733d85f3";

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
