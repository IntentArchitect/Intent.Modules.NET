using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application
{
    public class ReceiveOrgCommandDepartmentsDto
    {
        public ReceiveOrgCommandDepartmentsDto()
        {
            Name = null!;
            Code = null!;
            Description = null!;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static ReceiveOrgCommandDepartmentsDto Create(string name, string code, string description)
        {
            return new ReceiveOrgCommandDepartmentsDto
            {
                Name = name,
                Code = code,
                Description = description
            };
        }
    }
}