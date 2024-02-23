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

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class GetMongoInvoiceMongoLineByIdQuery : IRequest<MongoInvoiceMongoLineDto>, IQuery
    {
        public GetMongoInvoiceMongoLineByIdQuery(string mongoInvoiceId, string id)
        {
            MongoInvoiceId = mongoInvoiceId;
            Id = id;
        }

        public string MongoInvoiceId { get; set; }
        public string Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMongoInvoiceMongoLineByIdQueryHandler : IRequestHandler<GetMongoInvoiceMongoLineByIdQuery, MongoInvoiceMongoLineDto>
    {
        private readonly IMongoInvoiceRepository _mongoInvoiceRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetMongoInvoiceMongoLineByIdQueryHandler(IMongoInvoiceRepository mongoInvoiceRepository, IMapper mapper)
        {
            _mongoInvoiceRepository = mongoInvoiceRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MongoInvoiceMongoLineDto> Handle(
            GetMongoInvoiceMongoLineByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _mongoInvoiceRepository.FindByIdAsync(request.MongoInvoiceId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(MongoInvoice)} of Id '{request.MongoInvoiceId}' could not be found");
            }

            var element = aggregateRoot.MongoLines.FirstOrDefault(p => p.Id == request.Id);
            if (element is null)
            {
                throw new NotFoundException($"Could not find MongoLine '{request.Id}'");
            }

            return element.MapToMongoInvoiceMongoLineDto(_mapper);
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetMongoInvoiceMongoLineByIdQueryValidator : AbstractValidator<GetMongoInvoiceMongoLineByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetMongoInvoiceMongoLineByIdQueryValidator()
        {
            // IntentFully(Match = "ConfigureValidationRules")
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.MongoInvoiceId)
                .NotNull();

            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}