using System;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates.DeleteEntityAlternate
{
    public class DeleteEntityAlternateCommand : IRequest, ICommand
    {
        public DeleteEntityAlternateCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}