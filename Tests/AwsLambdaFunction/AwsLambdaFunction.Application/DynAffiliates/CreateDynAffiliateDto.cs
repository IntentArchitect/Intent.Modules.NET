using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynAffiliates
{
    public class CreateDynAffiliateDto
    {
        public CreateDynAffiliateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static CreateDynAffiliateDto Create(string name)
        {
            return new CreateDynAffiliateDto
            {
                Name = name
            };
        }
    }
}