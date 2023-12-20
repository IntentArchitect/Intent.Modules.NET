using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.DeleteUserAddress
{
    public class DeleteUserAddressCommand : IRequest, ICommand
    {
        public DeleteUserAddressCommand(Guid userId, Guid id)
        {
            UserId = userId;
            Id = id;
        }

        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}