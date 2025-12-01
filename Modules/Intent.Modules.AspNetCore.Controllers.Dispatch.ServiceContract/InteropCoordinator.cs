using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Common.UnitOfWork.Shared;

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.ServiceContract;

public static class InteropCoordinator
{
    public static bool ShouldInstallUnitOfWork(ICSharpFileBuilderTemplate template)
    {
        return template.SystemUsesPersistenceUnitOfWork();
    }

    public static bool ShouldInstallMessageBus(IApplication application)
    {
        return application.FindTemplateInstances<IClassProvider>(TemplateRoles.Application.Eventing.MessageBusInterface).Any() ||
               application.FindTemplateInstances<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface).Any();
    }
    
    public static bool ShouldInstallMongoDbUnitOfWork(IApplication application)
    {
        return application.FindTemplateInstance<IClassProvider>("Domain.UnitOfWork.MongoDb") != null;
    }
}