using System;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityDefaults.CreateEntityDefault
{
    public class CreateEntityDefaultCommand : IRequest<Guid>, ICommand
    {
        public CreateEntityDefaultCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}