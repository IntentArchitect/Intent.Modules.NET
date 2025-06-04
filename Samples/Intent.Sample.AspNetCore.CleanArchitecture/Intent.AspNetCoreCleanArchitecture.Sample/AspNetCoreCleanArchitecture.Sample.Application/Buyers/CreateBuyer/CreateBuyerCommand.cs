using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.CreateBuyer
{
    public class CreateBuyerCommand : IRequest<Guid>, ICommand
    {
        public CreateBuyerCommand(string name, string surname, string email, bool isActive, CreateBuyerAddressDto address)
        {
            Name = name;
            Surname = surname;
            Email = email;
            IsActive = isActive;
            Address = address;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public CreateBuyerAddressDto Address { get; set; }
    }
}