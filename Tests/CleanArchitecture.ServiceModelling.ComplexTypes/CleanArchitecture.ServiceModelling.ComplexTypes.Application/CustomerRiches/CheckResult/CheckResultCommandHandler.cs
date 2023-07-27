using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Exceptions;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.CheckResult
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CheckResultCommandHandler : IRequestHandler<CheckResultCommand, CheckAddressDCDto>
    {
        private readonly ICustomerRichRepository _customerRichRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CheckResultCommandHandler(ICustomerRichRepository customerRichRepository, IMapper mapper)
        {
            _customerRichRepository = customerRichRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CheckAddressDCDto> Handle(CheckResultCommand request, CancellationToken cancellationToken)
        {
            var existingCustomerRich = await _customerRichRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCustomerRich is null)
            {
                throw new NotFoundException($"Could not find CustomerRich '{request.Id}'");
            }

            var result = existingCustomerRich.GetAddress();
            return result.MapToCheckAddressDCDto(_mapper);
        }
    }
}