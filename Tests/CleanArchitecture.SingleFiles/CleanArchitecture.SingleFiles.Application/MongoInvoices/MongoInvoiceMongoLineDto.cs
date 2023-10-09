using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Mappings;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.MongoInvoices
{
    public class MongoInvoiceMongoLineDto : IMapFrom<MongoLine>
    {
        public MongoInvoiceMongoLineDto()
        {
            MongoInvoiceId = null!;
            Id = null!;
            Name = null!;
        }

        public string MongoInvoiceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public static MongoInvoiceMongoLineDto Create(string mongoInvoiceId, string id, string name)
        {
            return new MongoInvoiceMongoLineDto
            {
                MongoInvoiceId = mongoInvoiceId,
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MongoLine, MongoInvoiceMongoLineDto>();
        }
    }
}