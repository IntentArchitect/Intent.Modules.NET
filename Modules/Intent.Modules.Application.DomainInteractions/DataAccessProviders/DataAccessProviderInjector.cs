using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

internal class DataAccessProviderInjector : IDataAccessProviderInjector
{
    private DataAccessProviderInjector() { }

    public static IDataAccessProviderInjector Instance { get; } = new DataAccessProviderInjector();

    public IDataAccessProvider Inject(ICSharpClassMethodDeclaration method, ClassModel classModel)
    {
        return method.InjectDataAccessProvider(classModel);
    }
}