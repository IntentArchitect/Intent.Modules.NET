using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices.Contracts.Services.Products
{
    public class ProductCreateDto
    {
        public ProductCreateDto()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }

        public static ProductCreateDto Create(string name, string surname)
        {
            return new ProductCreateDto
            {
                Name = name,
                Surname = surname
            };
        }
    }
}