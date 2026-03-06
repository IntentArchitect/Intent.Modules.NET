using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string email, bool isVip, DateTime? birthDate, string? metadataJson)
        {
            Name = name;
            Email = email;
            IsVip = isVip;
            BirthDate = birthDate;
            MetadataJson = metadataJson;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsVip { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? MetadataJson { get; set; }
    }
}