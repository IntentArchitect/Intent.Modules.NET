using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.UpdateDtoReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDtoReturnCommandHandler : IRequestHandler<UpdateDtoReturnCommand, DtoReturnDto>
    {
        private readonly IDtoReturnRepository _dtoReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public UpdateDtoReturnCommandHandler(IDtoReturnRepository dtoReturnRepository, IMapper mapper)
        {
            _dtoReturnRepository = dtoReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DtoReturnDto> Handle(UpdateDtoReturnCommand request, CancellationToken cancellationToken)
        {
            var dtoReturn = await _dtoReturnRepository.FindByIdAsync(request.Id, cancellationToken);
            if (dtoReturn is null)
            {
                throw new NotFoundException($"Could not find DtoReturn '{request.Id}'");
            }

            dtoReturn.Name = request.Name;
            return dtoReturn.MapToDtoReturnDto(_mapper);
        }
    }
}