using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>, ICommand
    {
        public CreateUserCommand(string name, string surname, string email, Guid quoteId)
        {
            Name = name;
            Surname = surname;
            Email = email;
            QuoteId = quoteId;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid QuoteId { get; set; }
    }
}