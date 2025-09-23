using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AwsLambdaFunction.Application.DynAffiliates
{
    public class DynAffiliateDto
    {
        public DynAffiliateDto()
        {
            Id = null!;
            Name = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public static DynAffiliateDto Create(string id, string name)
        {
            return new DynAffiliateDto
            {
                Id = id,
                Name = name
            };
        }
    }
}