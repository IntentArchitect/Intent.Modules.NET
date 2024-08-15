using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Accounts.CreateAccount
{
    public class CreateAccountCommand : IRequest<Guid>, ICommand
    {
        public CreateAccountCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}