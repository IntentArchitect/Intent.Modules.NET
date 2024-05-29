using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.AdvancedMappingMongoInvoices
{
    public static class MongoInvoiceDtoMappingExtensions
    {
        public static MongoInvoiceDto MapToMongoInvoiceDto(this MongoInvoice projectFrom, IMapper mapper)
            => mapper.Map<MongoInvoiceDto>(projectFrom);

        public static List<MongoInvoiceDto> MapToMongoInvoiceDtoList(this IEnumerable<MongoInvoice> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToMongoInvoiceDto(mapper)).ToList();
    }
}