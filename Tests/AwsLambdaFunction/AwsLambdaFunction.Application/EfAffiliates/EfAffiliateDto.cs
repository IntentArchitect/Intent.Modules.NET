using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfAffiliates
{
    public class EfAffiliateDto
    {
        public EfAffiliateDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static EfAffiliateDto Create(Guid id, string name)
        {
            return new EfAffiliateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}