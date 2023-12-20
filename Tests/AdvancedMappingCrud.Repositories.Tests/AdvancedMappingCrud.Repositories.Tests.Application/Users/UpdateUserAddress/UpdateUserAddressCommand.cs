using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.UpdateUserAddress
{
    public class UpdateUserAddressCommand : IRequest, ICommand
    {
        public UpdateUserAddressCommand(Guid userId, Guid id, string line1, string line2, string city, string postal)
        {
            UserId = userId;
            Id = id;
            Line1 = line1;
            Line2 = line2;
            City = city;
            Postal = postal;
        }

        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Postal { get; set; }
    }
}