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

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class GetDaprInvoiceByIdQuery : IRequest<DaprInvoiceDto>, IQuery
    {
        public GetDaprInvoiceByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDaprInvoiceByIdQueryHandler : IRequestHandler<GetDaprInvoiceByIdQuery, DaprInvoiceDto>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetDaprInvoiceByIdQueryHandler(IDaprInvoiceRepository daprInvoiceRepository, IMapper mapper)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DaprInvoiceDto> Handle(GetDaprInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var daprInvoice = await _daprInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (daprInvoice is null)
            {
                throw new NotFoundException($"Could not find DaprInvoice '{request.Id}'");
            }

            return daprInvoice.MapToDaprInvoiceDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDaprInvoiceByIdQueryValidator : AbstractValidator<GetDaprInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDaprInvoiceByIdQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}