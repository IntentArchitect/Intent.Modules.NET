using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    interface IImplementationStrategy
    {
        bool IsMatch(OperationModel operationModel);
        void ApplyStrategy(OperationModel operationModel);
    }
}
