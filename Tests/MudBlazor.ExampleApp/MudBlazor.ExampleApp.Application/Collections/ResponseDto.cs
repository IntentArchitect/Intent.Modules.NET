using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Collections
{
    public class ResponseDto
    {
        public ResponseDto()
        {
            Three = null!;
        }

        public int One { get; set; }
        public int Two { get; set; }
        public string Three { get; set; }

        public static ResponseDto Create(int one, int two, string three)
        {
            return new ResponseDto
            {
                One = one,
                Two = two,
                Three = three
            };
        }
    }
}