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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class GetMongoInvoiceByIdQuery : IRequest<MongoInvoiceDto>, IQuery
    {
        public GetMongoInvoiceByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMongoInvoiceByIdQueryHandler : IRequestHandler<GetMongoInvoiceByIdQuery, MongoInvoiceDto>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetMongoInvoiceByIdQueryHandler(IMongoInvoiceRepository mongoInvoiceRepository, IMapper mapper)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MongoInvoiceDto> Handle(GetMongoInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var mongoInvoice = await _mongoInvoiceRepository.FindByIdAsync(request.Id, cancellationToken);
            if (mongoInvoice is null)
            {
                throw new NotFoundException($"Could not find MongoInvoice '{request.Id}'");
            }

            return mongoInvoice.MapToMongoInvoiceDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMongoInvoiceByIdQueryValidator : AbstractValidator<GetMongoInvoiceByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetMongoInvoiceByIdQueryValidator()
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