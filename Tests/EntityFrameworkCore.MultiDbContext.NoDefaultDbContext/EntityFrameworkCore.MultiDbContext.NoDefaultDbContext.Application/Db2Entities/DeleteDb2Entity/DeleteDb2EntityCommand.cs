using System;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.DeleteDb2Entity
{
    public class DeleteDb2EntityCommand : IRequest, ICommand
    {
        public DeleteDb2EntityCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}