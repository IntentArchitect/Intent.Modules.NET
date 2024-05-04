using System;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.CreateEntityAlternate
{
    public class CreateEntityAlternateCommand : IRequest<Guid>, ICommand
    {
        public CreateEntityAlternateCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}