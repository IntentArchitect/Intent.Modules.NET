using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformValueObjAsync
{
    public class PerformValueObjAsyncCommand : IRequest, ICommand
    {
        public PerformValueObjAsyncCommand(string value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public string Value1 { get; set; }
        public int Value2 { get; set; }
    }
}