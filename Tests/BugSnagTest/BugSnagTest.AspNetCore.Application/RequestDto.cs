using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace BugSnagTest.AspNetCore.Application
{
    public class RequestDto
    {
        public RequestDto()
        {
            Value = null!;
        }

        public string Value { get; set; }

        public static RequestDto Create(string value)
        {
            return new RequestDto
            {
                Value = value
            };
        }
    }
}