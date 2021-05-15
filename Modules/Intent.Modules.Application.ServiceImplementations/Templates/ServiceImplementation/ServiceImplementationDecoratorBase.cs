using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public abstract class ServiceImplementationDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            return new List<ConstructorParameter>();
        }

        public virtual string GetDecoratedImplementation(OperationModel operationModel)
        {
            return string.Empty;
        }

        public int Priority { get; } = 0;
    }
}
