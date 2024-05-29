using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Interfaces;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingDaprInvoices
{
    public class GetDaprInvoicesQuery : IRequest<List<DaprInvoiceDto>>, IQuery
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        private readonly IMapper _mapper;

        public GetDaprInvoicesQuery(IDaprInvoiceRepository daprInvoiceRepository, IMapper mapper)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
            _mapper = mapper;
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDaprInvoicesQueryHandler : IRequestHandler<GetDaprInvoicesQuery, List<DaprInvoiceDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetDaprInvoicesQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<DaprInvoiceDto>> Handle(GetDaprInvoicesQuery request, CancellationToken cancellationToken)
        {
            var daprInvoices = await _daprInvoiceRepository.FindAllAsync(cancellationToken);
            return daprInvoices.MapToDaprInvoiceDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDaprInvoicesQueryValidator : AbstractValidator<GetDaprInvoicesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDaprInvoicesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}