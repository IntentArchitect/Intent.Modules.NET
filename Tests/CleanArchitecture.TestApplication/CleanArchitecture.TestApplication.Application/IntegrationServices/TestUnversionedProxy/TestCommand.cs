using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationServices.TestUnversionedProxy
{
    public class TestCommand
    {
        public static TestCommand Create(
            string value)
        {
            return new TestCommand
            {
                Value = value,
            };
        }

        public string Value { get; set; }
    }
}