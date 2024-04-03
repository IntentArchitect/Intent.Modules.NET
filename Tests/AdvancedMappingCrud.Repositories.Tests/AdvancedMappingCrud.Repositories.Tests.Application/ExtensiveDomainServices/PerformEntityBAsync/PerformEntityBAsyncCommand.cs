using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformEntityBAsync
{
    public class PerformEntityBAsyncCommand : IRequest, ICommand
    {
        public PerformEntityBAsyncCommand(string baseAttr, string concreteAttr)
        {
            BaseAttr = baseAttr;
            ConcreteAttr = concreteAttr;
        }

        public string BaseAttr { get; set; }
        public string ConcreteAttr { get; set; }
    }
}