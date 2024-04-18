using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Eventing.GoogleCloud.PubSub;

public static class InteropCoordinator
{
    public static bool ShouldInstallMongoDbUnitOfWork(IApplication application)
    {
        return application.FindTemplateInstance<IClassProvider>("Domain.UnitOfWork.MongoDb") != null;
    }
}