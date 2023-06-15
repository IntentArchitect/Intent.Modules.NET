using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.TestVersionedProxy
{
    public class TestQueryV1
    {
        public static TestQueryV1 Create(
            string value)
        {
            return new TestQueryV1
            {
                Value = value,
            };
        }

        public string Value { get; set; }
    }
}