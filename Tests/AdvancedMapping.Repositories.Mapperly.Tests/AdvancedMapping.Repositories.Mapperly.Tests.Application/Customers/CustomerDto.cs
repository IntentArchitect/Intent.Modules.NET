using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers
{
    public class CustomerDto
    {
        public CustomerDto()
        {
            Name = null!;
            Email = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsVip { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? MetadataJson { get; set; }
        public bool? PreferencesNewsletter { get; set; }
        public bool? PreferencesSpecials { get; set; }

        public static CustomerDto Create(
            Guid id,
            string name,
            string email,
            bool isVip,
            DateTime? birthDate,
            string? metadataJson,
            bool? preferencesNewsletter, bool? preferencesSpecials)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Email = email,
                IsVip = isVip,
                BirthDate = birthDate,
                MetadataJson = metadataJson,
                PreferencesNewsletter = preferencesNewsletter,
                PreferencesSpecials = preferencesSpecials
            };
        }
    }
}