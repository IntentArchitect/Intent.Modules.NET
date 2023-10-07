using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn
{
    public class UpdateAggregateTestNoIdReturnCommand : IRequest, ICommand
    {
        public UpdateAggregateTestNoIdReturnCommand(Guid id, string attribute)
        {
            Id = id;
            Attribute = attribute;
        }

        public Guid Id { get; private set; }
        public string Attribute { get; set; }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}