using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn
{
    public class CreateAggregateTestNoIdReturnCommand : IRequest, ICommand
    {
        public CreateAggregateTestNoIdReturnCommand(string attribute)
        {
            Attribute = attribute;
        }

        public string Attribute { get; set; }
    }
}