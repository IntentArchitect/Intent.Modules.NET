using Intent.Engine;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.UnitOfWork.Persistence.Shared;

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.Services;

internal class InteropCoordinator
{
    public static bool ShouldInstallUnitOfWork(ICSharpFileBuilderTemplate template)
    {
        return template.SystemUsesPersistenceUnitOfWork();
    }

    public static bool ShouldInstallMessageBus(IApplication application)
    {
        // Support newer MessageBusInterface role as well as legacy EventBusInterface role.
        return application.FindTemplateInstances<IClassProvider>(TemplateRoles.Application.Eventing.MessageBusInterface).Any() ||
               application.FindTemplateInstances<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface).Any();
    }
}