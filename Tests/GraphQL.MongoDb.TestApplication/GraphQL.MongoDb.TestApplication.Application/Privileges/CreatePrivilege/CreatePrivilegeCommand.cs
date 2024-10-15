using System;
using System.Collections.Generic;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Privileges.CreatePrivilege
{
    public class CreatePrivilegeCommand : IRequest<PrivilegeDto>, ICommand
    {
        public CreatePrivilegeCommand(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
        public string Name { get; set; }

        public string? Description { get; set; }

    }
}