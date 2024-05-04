using System;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAppDefaults.CreateEntityAppDefault
{
    public class CreateEntityAppDefaultCommand : IRequest<Guid>, ICommand
    {
        public CreateEntityAppDefaultCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}