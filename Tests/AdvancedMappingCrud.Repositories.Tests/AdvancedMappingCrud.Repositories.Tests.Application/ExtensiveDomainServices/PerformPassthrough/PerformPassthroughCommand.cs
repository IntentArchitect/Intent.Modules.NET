using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformPassthrough
{
    public class PerformPassthroughCommand : IRequest, ICommand
    {
        public PerformPassthroughCommand(string concreteAttr, string baseAttr)
        {
            ConcreteAttr = concreteAttr;
            BaseAttr = baseAttr;
        }

        public string ConcreteAttr { get; set; }
        public string BaseAttr { get; set; }
    }
}