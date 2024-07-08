using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.SqlOutParameter.SpMultipleOut
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SpMultipleOutQueryHandler : IRequestHandler<SpMultipleOutQuery, MultipleOutDto>
    {
        private readonly ISqlOutParameterRepository _sqlOutParameterRepository;

        [IntentManaged(Mode.Merge)]
        public SpMultipleOutQueryHandler(ISqlOutParameterRepository sqlOutParameterRepository)
        {
            _sqlOutParameterRepository = sqlOutParameterRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MultipleOutDto> Handle(SpMultipleOutQuery request, CancellationToken cancellationToken)
        {
            var (param1Output, param2Output, param3Output) = await _sqlOutParameterRepository.Sp_out_params_multiple(cancellationToken);

            // TODO: Implement return type mapping...
            throw new NotImplementedException("Implement return type mapping...");
        }
    }
}