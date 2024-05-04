using System;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.UpdateDb1Entity
{
    public class UpdateDb1EntityCommand : IRequest, ICommand
    {
        public UpdateDb1EntityCommand(Guid id, string message)
        {
            Id = id;
            Message = message;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }
    }
}