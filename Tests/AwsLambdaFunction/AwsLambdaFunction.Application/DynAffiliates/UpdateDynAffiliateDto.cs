using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynAffiliates
{
    public class UpdateDynAffiliateDto
    {
        public UpdateDynAffiliateDto()
        {
            Name = null!;
        }

        public string Name { get; set; }

        public static UpdateDynAffiliateDto Create(string name)
        {
            return new UpdateDynAffiliateDto
            {
                Name = name
            };
        }
    }
}