using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.UpdateUser
{
    public class UpdateUserCommand : IRequest, ICommand
    {
        public UpdateUserCommand(Guid id, string email, Guid quoteId, string name, string surname)
        {
            Id = id;
            Email = email;
            QuoteId = quoteId;
            Name = name;
            Surname = surname;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public Guid QuoteId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}