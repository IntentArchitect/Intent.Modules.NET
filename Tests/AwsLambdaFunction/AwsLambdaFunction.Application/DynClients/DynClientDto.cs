using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynClients
{
    public class DynClientDto
    {
        public DynClientDto()
        {
            Id = null!;
            Name = null!;
            AffiliateId = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string AffiliateId { get; set; }

        public static DynClientDto Create(string id, string name, string affiliateId)
        {
            return new DynClientDto
            {
                Id = id,
                Name = name,
                AffiliateId = affiliateId
            };
        }
    }
}