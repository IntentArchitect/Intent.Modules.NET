using System.Collections.Generic;
using Intent.Modules.Application.MediatR.Templates;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    internal interface ICrudImplementationStrategy
    {
        bool IsMatch();
        IEnumerable<RequiredService> GetRequiredServices();
        string GetImplementation();
        void OnStrategySelection();
    }
}