using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

public interface IDataAccessProviderInjector
{
    IDataAccessProvider Inject(ICSharpClassMethodDeclaration method, ClassModel classModel);
}