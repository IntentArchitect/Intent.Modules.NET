using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfClients
{
    public class EfClientDto
    {
        public EfClientDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid AffiliateId { get; set; }

        public static EfClientDto Create(Guid id, string name, Guid affiliateId)
        {
            return new EfClientDto
            {
                Id = id,
                Name = name,
                AffiliateId = affiliateId
            };
        }
    }
}