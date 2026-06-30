using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ObjectMappingTest.Domain.Entities
{
    public class DigitalProduct : Product
    {
        public DigitalProduct()
        {
            DownloadUrl = null!;
        }

        public string DownloadUrl { get; set; }
    }
}