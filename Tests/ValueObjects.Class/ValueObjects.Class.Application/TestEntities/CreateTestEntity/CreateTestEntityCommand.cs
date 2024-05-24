using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Class.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ValueObjects.Class.Application.TestEntities.CreateTestEntity
{
    public class CreateTestEntityCommand : IRequest<Guid>, ICommand
    {
        public CreateTestEntityCommand(string name, CreateTestEntityMoneyDto amount, CreateTestEntityAddressDto address)
        {
            Name = name;
            Amount = amount;
            Address = address;
        }

        public string Name { get; set; }
        public CreateTestEntityMoneyDto Amount { get; set; }
        public CreateTestEntityAddressDto Address { get; set; }
    }
}