using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ServiceToServiceInvocation
{
    public class GetDataEntryDto
    {
        public GetDataEntryDto()
        {
            Data = null!;
        }

        public string Data { get; set; }

        public static GetDataEntryDto Create(string data)
        {
            return new GetDataEntryDto
            {
                Data = data
            };
        }
    }
}