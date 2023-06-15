using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy
{
    public class TestCommandV2
    {
        public static TestCommandV2 Create(
            string value)
        {
            return new TestCommandV2
            {
                Value = value,
            };
        }

        public string Value { get; set; }
    }
}