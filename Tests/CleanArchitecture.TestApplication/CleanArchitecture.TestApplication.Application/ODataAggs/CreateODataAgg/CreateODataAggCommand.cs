using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ODataAggs.CreateODataAgg
{
    public class CreateODataAggCommand : IRequest<Guid>, ICommand
    {
        public CreateODataAggCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}