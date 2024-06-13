using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.SqlOutParameter.SpMultipleOut
{
    public class SpMultipleOutQuery : IRequest<MultipleOutDto>, IQuery
    {
        public SpMultipleOutQuery()
        {
        }
    }
}