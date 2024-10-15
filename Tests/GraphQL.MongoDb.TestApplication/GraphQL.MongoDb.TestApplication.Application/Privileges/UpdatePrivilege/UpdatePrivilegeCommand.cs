using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.UpdatePrivilege
{
    public class UpdatePrivilegeCommand : IRequest<PrivilegeDto>, ICommand
    {
        public UpdatePrivilegeCommand(string id, string name, string? description = null)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

    }
}