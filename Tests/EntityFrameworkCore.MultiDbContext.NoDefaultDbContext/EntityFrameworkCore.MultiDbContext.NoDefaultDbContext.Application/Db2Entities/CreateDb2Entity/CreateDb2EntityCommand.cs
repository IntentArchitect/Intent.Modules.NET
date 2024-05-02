using System;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.CreateDb2Entity
{
    public class CreateDb2EntityCommand : IRequest<Guid>, ICommand
    {
        public CreateDb2EntityCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}