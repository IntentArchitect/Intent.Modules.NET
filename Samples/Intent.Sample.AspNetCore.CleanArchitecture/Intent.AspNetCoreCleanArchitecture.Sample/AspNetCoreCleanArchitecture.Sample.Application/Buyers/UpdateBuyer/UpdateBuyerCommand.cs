using AspNetCoreCleanArchitecture.Sample.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreCleanArchitecture.Sample.Application.Buyers.UpdateBuyer
{
    public class UpdateBuyerCommand : IRequest, ICommand
    {
        public UpdateBuyerCommand(string name, string surname, string email, bool isActive, UpdateBuyerAddressDto address, Guid id)
        {
            Name = name;
            Surname = surname;
            Email = email;
            IsActive = isActive;
            Address = address;
            Id = id;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public UpdateBuyerAddressDto Address { get; set; }
        public Guid Id { get; set; }
    }
}