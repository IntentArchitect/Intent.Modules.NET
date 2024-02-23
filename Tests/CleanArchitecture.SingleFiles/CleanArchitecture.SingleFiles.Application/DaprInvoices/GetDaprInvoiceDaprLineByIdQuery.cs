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

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class GetDaprInvoiceDaprLineByIdQuery : IRequest<DaprInvoiceDaprLineDto>, IQuery
    {
        public GetDaprInvoiceDaprLineByIdQuery(string daprInvoiceId, string id)
        {
            DaprInvoiceId = daprInvoiceId;
            Id = id;
        }

        public string DaprInvoiceId { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetDaprInvoiceDaprLineByIdQueryHandler : IRequestHandler<GetDaprInvoiceDaprLineByIdQuery, DaprInvoiceDaprLineDto>
    {
        private readonly IDaprInvoiceRepository _daprInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetDaprInvoiceDaprLineByIdQueryHandler(IDaprInvoiceRepository daprInvoiceRepository, IMapper mapper)
        {
            _daprInvoiceRepository = daprInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<DaprInvoiceDaprLineDto> Handle(
            GetDaprInvoiceDaprLineByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _daprInvoiceRepository.FindByIdAsync(request.DaprInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(DaprInvoice)} of Id '{request.DaprInvoiceId}' could not be found");
            }

            var element = aggregateRoot.DaprLines.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find DaprLine '{request.Id}'");
            }

            return element.MapToDaprInvoiceDaprLineDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDaprInvoiceDaprLineByIdQueryValidator : AbstractValidator<GetDaprInvoiceDaprLineByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDaprInvoiceDaprLineByIdQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.DaprInvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}