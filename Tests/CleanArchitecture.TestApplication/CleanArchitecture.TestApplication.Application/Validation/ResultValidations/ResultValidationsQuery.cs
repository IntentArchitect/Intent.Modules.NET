using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.ResultValidations
{
    public class ResultValidationsQuery : IRequest<ValidatedResultDto>, IQuery
    {
        public ResultValidationsQuery()
        {
        }
    }
}