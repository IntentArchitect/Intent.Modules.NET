using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.ServiceContract;

public static class InteropCoordinator
{
    public static bool ShouldInstallUnitOfWork(IApplication application)
    {
        var hasUnitOfWorkInterface =
            application.FindTemplateInstance<IClassProvider>(TemplateRoles.Domain.UnitOfWork) != null ||
            application.FindTemplateInstance<IClassProvider>(TemplateRoles.Application.Common.DbContextInterface) != null;
        var hasUnitOfWorkConcrete = application.FindTemplateInstance<IClassProvider>("Infrastructure.Data.DbContext") != null;
        return hasUnitOfWorkInterface && hasUnitOfWorkConcrete;
    }

    public static bool ShouldInstallMessageBus(IApplication application)
    {
        return application.FindTemplateInstance<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface) != null;
    }
    
    public static bool ShouldInstallMongoDbUnitOfWork(IApplication application)
    {
        return application.FindTemplateInstance<IClassProvider>("Domain.UnitOfWork.MongoDb") != null;
    }
}