using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application
{
    public class GenDto<T>
    {
        public GenDto()
        {
        }

        public T Field { get; set; }

        public static GenDto<T> Create(T field)
        {
            return new GenDto<T>
            {
                Field = field
            };
        }
    }
}