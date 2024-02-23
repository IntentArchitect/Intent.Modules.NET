using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Common.Exceptions;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.EfInvoices
{
    public class GetEfInvoiceEfLineByIdQuery : IRequest<EfInvoiceEfLineDto>, IQuery
    {
        public GetEfInvoiceEfLineByIdQuery(Guid efInvoicesId, Guid id)
        {
            EfInvoicesId = efInvoicesId;
            Id = id;
        }

        public Guid EfInvoicesId { get; set; }
        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEfInvoiceEfLineByIdQueryHandler : IRequestHandler<GetEfInvoiceEfLineByIdQuery, EfInvoiceEfLineDto>
    {
        private readonly IEfInvoiceRepository _efInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetEfInvoiceEfLineByIdQueryHandler(IEfInvoiceRepository efInvoiceRepository, IMapper mapper)
        {
            _efInvoiceRepository = efInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<EfInvoiceEfLineDto> Handle(
            GetEfInvoiceEfLineByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _efInvoiceRepository.FindByIdAsync(request.EfInvoicesId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(EfInvoice)} of Id '{request.EfInvoicesId}' could not be found");
            }

            var element = aggregateRoot.EfLines.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find EfLine '{request.Id}'");
            }

            return element.MapToEfInvoiceEfLineDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEfInvoiceEfLineByIdQueryValidator : AbstractValidator<GetEfInvoiceEfLineByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEfInvoiceEfLineByIdQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}