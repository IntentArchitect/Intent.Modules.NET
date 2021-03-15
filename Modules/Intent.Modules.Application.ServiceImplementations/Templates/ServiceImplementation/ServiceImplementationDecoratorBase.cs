using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public abstract class ServiceImplementationDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<string> GetUsings(ServiceModel service)
        {
            return new List<string>();
        }

        public virtual IEnumerable<ConstructorParameter> GetConstructorDependencies(ServiceModel service)
        {
            return new List<ConstructorParameter>();
        }

        public virtual string GetDecoratedImplementation(ServiceModel service, OperationModel operationModel)
        {
            return string.Empty;
        }

        public int Priority { get; } = 0;
    }
}
