using System;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.DeleteDb1Entity
{
    public class DeleteDb1EntityCommand : IRequest, ICommand
    {
        public DeleteDb1EntityCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}