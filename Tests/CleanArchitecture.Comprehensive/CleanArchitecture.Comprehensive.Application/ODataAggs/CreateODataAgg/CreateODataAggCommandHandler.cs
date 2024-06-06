using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery;
using CleanArchitecture.Comprehensive.Domain.Repositories.ODataQuery;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.ODataAggs.CreateODataAgg
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateODataAggCommandHandler : IRequestHandler<CreateODataAggCommand, Guid>
    {
        private readonly IODataAggRepository _oDataAggRepository;

        [IntentManaged(Mode.Merge)]
        public CreateODataAggCommandHandler(IODataAggRepository oDataAggRepository)
        {
            _oDataAggRepository = oDataAggRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateODataAggCommand request, CancellationToken cancellationToken)
        {
            var newODataAgg = new ODataAgg
            {
                Name = request.Name,
            };

            _oDataAggRepository.Add(newODataAgg);
            await _oDataAggRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newODataAgg.Id;
        }
    }
}