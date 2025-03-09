using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application
{
    public class ComplexTypeDto
    {
        public ComplexTypeDto()
        {
            Field = null!;
        }

        public string Field { get; set; }

        public static ComplexTypeDto Create(string field)
        {
            return new ComplexTypeDto
            {
                Field = field
            };
        }
    }
}