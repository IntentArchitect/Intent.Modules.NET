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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class GetMongoInvoiceMongoLinesQuery : IRequest<List<MongoInvoiceMongoLineDto>>, IQuery
    {
        public GetMongoInvoiceMongoLinesQuery(string mongoInvoiceId)
        {
            MongoInvoiceId = mongoInvoiceId;
        }

        public string MongoInvoiceId { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMongoInvoiceMongoLinesQueryHandler : IRequestHandler<GetMongoInvoiceMongoLinesQuery, List<MongoInvoiceMongoLineDto>>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetMongoInvoiceMongoLinesQueryHandler(IMongoInvoiceRepository mongoInvoiceRepository, IMapper mapper)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MongoInvoiceMongoLineDto>> Handle(
            GetMongoInvoiceMongoLinesQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _mongoInvoiceRepository.FindByIdAsync(request.MongoInvoiceId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(MongoInvoice)} of Id '{request.MongoInvoiceId}' could not be found");
            }
            return aggregateRoot.MongoLines.MapToMongoInvoiceMongoLineDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMongoInvoiceMongoLinesQueryValidator : AbstractValidator<GetMongoInvoiceMongoLinesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetMongoInvoiceMongoLinesQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MongoInvoiceId)
                .NotNull();
        }
    }
}