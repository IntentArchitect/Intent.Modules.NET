using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries.UpdateCountry
{
    public class UpdateCountryCommand : IRequest, ICommand
    {
        public UpdateCountryCommand(string name, string code, Guid id)
        {
            Name = name;
            Code = code;
            Id = id;
        }

        public string Name { get; set; }
        public string Code { get; set; }
        public Guid Id { get; set; }
    }
}