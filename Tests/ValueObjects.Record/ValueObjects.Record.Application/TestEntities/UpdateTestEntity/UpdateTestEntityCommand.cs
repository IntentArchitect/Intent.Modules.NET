using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities.UpdateTestEntity
{
    public class UpdateTestEntityCommand : IRequest, ICommand
    {
        public UpdateTestEntityCommand(string name,
            UpdateTestEntityMoneyDto amount,
            UpdateTestEntityAddressDto address,
            Guid id)
        {
            Name = name;
            Amount = amount;
            Address = address;
            Id = id;
        }

        public string Name { get; set; }
        public UpdateTestEntityMoneyDto Amount { get; set; }
        public UpdateTestEntityAddressDto Address { get; set; }
        public Guid Id { get; set; }
    }
}