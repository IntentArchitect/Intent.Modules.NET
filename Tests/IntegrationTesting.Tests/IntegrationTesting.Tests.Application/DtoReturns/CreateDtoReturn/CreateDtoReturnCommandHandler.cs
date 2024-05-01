using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.CreateDtoReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDtoReturnCommandHandler : IRequestHandler<CreateDtoReturnCommand, DtoReturnDto>
    {
        private readonly IDtoReturnRepository _dtoReturnRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public CreateDtoReturnCommandHandler(IDtoReturnRepository dtoReturnRepository, IMapper mapper)
        {
            _dtoReturnRepository = dtoReturnRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DtoReturnDto> Handle(CreateDtoReturnCommand request, CancellationToken cancellationToken)
        {
            var dtoReturn = new DtoReturn
            {
                Name = request.Name
            };

            _dtoReturnRepository.Add(dtoReturn);
            await _dtoReturnRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return dtoReturn.MapToDtoReturnDto(_mapper);
        }
    }
}