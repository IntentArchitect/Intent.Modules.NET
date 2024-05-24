using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.People
{
    public class UpdateUpdatePersonDetailsNameDto
    {
        public UpdateUpdatePersonDetailsNameDto()
        {
            First = null!;
            Last = null!;
        }

        public string First { get; set; }
        public string Last { get; set; }

        public static UpdateUpdatePersonDetailsNameDto Create(string first, string last)
        {
            return new UpdateUpdatePersonDetailsNameDto
            {
                First = first,
                Last = last
            };
        }
    }
}