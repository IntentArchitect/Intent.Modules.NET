using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfAffiliates
{
    public class CreateEfAffiliateDto
    {
        public CreateEfAffiliateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateEfAffiliateDto Create(string name)
        {
            return new CreateEfAffiliateDto
            {
                Name = name
            };
        }
    }
}