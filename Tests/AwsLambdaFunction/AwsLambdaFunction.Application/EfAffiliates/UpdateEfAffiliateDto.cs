using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.EfAffiliates
{
    public class UpdateEfAffiliateDto
    {
        public UpdateEfAffiliateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateEfAffiliateDto Create(string name)
        {
            return new UpdateEfAffiliateDto
            {
                Name = name
            };
        }
    }
}