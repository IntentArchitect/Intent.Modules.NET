using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class GetDaprInvoiceDaprLinesQuery : IRequest<List<DaprInvoiceDaprLineDto>>, IQuery
    {
        public GetDaprInvoiceDaprLinesQuery(string daprInvoiceId)
        {
            DaprInvoiceId = daprInvoiceId;
        }

        public string DaprInvoiceId { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDaprInvoiceDaprLinesQueryHandler : IRequestHandler<GetDaprInvoiceDaprLinesQuery, List<DaprInvoiceDaprLineDto>>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetDaprInvoiceDaprLinesQueryHandler(IDaprInvoiceRepository daprInvoiceRepository, IMapper mapper)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DaprInvoiceDaprLineDto>> Handle(
            GetDaprInvoiceDaprLinesQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _daprInvoiceRepository.FindByIdAsync(request.DaprInvoiceId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(DaprInvoice)} of Id '{request.DaprInvoiceId}' could not be found");
            }
            return aggregateRoot.DaprLines.MapToDaprInvoiceDaprLineDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDaprInvoiceDaprLinesQueryValidator : AbstractValidator<GetDaprInvoiceDaprLinesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDaprInvoiceDaprLinesQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DaprInvoiceId)
                .NotNull();
        }
    }
}