using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public static class MongoInvoiceMongoLineDtoMappingExtensions
    {
        public static MongoInvoiceMongoLineDto MapToMongoInvoiceMongoLineDto(this MongoLine projectFrom, IMapper mapper)
            => mapper.Map<MongoInvoiceMongoLineDto>(projectFrom);

        public static List<MongoInvoiceMongoLineDto> MapToMongoInvoiceMongoLineDtoList(this IEnumerable<MongoLine> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMongoInvoiceMongoLineDto(mapper)).ToList();
    }
}