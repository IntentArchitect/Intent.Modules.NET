using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.DDD.CreateAccountHolder
{
    public class CreateAccountHolder : IRequest<Guid>, ICommand
    {
        public CreateAccountHolder(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}