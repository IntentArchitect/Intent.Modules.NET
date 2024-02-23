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

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class GetEfInvoiceByIdQuery : IRequest<EfInvoiceDto>, IQuery
    {
        public GetEfInvoiceByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfInvoiceByIdQueryHandler : IRequestHandler<GetEfInvoiceByIdQuery, EfInvoiceDto>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEfInvoiceByIdQueryHandler(IEfInvoiceRepository efInvoiceRepository, IMapper mapper)
        {
            _efInvoiceRepository = efInvoiceRepository;
            _mapper = mapper;
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
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}