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

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingMongoInvoices
{
    public class GetMongoInvoicesQuery : IRequest<List<MongoInvoiceDto>>, IQuery
    {

        public GetMongoInvoicesQuery()
        {
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMongoInvoicesQueryHandler : IRequestHandler<GetMongoInvoicesQuery, List<MongoInvoiceDto>>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public GetMongoInvoicesQueryHandler(IMongoInvoiceRepository mongoInvoiceRepository, IMapper mapper)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MongoInvoiceDto>> Handle(GetMongoInvoicesQuery request, CancellationToken cancellationToken)
        {
            var mongoInvoices = await _mongoInvoiceRepository.FindAllAsync(cancellationToken);
            return mongoInvoices.MapToMongoInvoiceDtoList(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMongoInvoicesQueryValidator : AbstractValidator<GetMongoInvoicesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetMongoInvoicesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}