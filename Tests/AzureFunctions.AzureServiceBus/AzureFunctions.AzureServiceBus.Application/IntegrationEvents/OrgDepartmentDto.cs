using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Eventing.Messages
{
    public class OrgDepartmentDto
    {
        public OrgDepartmentDto()
        {
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static OrgDepartmentDto Create(string name, string code, string description)
        {
            return new OrgDepartmentDto
            {
                Name = name,
                Code = code,
                Description = description
            };
        }
    }
}