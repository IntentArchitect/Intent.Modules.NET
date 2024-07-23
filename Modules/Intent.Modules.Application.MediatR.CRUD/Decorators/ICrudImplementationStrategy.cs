using System.Collections.Generic;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Application.MediatR.CRUD.Decorators
{
    internal interface ICrudImplementationStrategy
    {
        bool IsMatch();
        void ApplyStrategy();
        void BindToTemplate(ICSharpFileBuilderTemplate template);
    }
}