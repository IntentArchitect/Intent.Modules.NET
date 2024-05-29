using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingEfInvoices
{
    public class GetEfInvoiceByIdQuery : IRequest<EfInvoiceDto>, IQuery
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IMapper _mapper;

        public GetEfInvoiceByIdQuery(Guid id, IEfInvoiceRepository efInvoiceRepository, IMapper mapper)
        {
            Id = id;
            _efInvoiceRepository = efInvoiceRepository;
            _mapper = mapper;
        }

        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfInvoiceByIdQueryHandler : IRequestHandler<GetEfInvoiceByIdQuery, EfInvoiceDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfInvoiceByIdQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EfInvoiceDto> Handle(GetEfInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var efInvoice = await _efInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (efInvoice is null)
            {
                throw new NotFoundException($"Could not find EfInvoice '{request.Id}'");
            }
            return efInvoice.MapToEfInvoiceDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEfInvoiceByIdQueryValidator : AbstractValidator<GetEfInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfInvoiceByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}