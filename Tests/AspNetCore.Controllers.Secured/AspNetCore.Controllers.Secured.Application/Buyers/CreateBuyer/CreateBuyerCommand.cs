using System;
using AspNetCore.Controllers.Secured.Application.Common.Interfaces;
using AspNetCore.Controllers.Secured.Application.Common.Security;
using AspNetCore.Controllers.Secured.Application.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers.CreateBuyer
{
    [Authorize(Roles = Permissions.Role2, Policy = Permissions.Policy2)]
    public class CreateBuyerCommand : IRequest<Guid>, ICommand
    {
        public CreateBuyerCommand(string name, string surname, string email)
        {
            Name = name;
            Surname = surname;
            Email = email;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}