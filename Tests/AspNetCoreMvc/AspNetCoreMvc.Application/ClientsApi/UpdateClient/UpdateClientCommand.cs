using System;
using AspNetCoreMvc.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AspNetCoreMvc.Application.ClientsApi.UpdateClient
{
    public class UpdateClientCommand : IRequest, ICommand
    {
        public UpdateClientCommand(string? name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public string? Name { get; set; }
        public Guid Id { get; set; }
    }
}