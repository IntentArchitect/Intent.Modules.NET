using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV1
{
    public class TestCommandV1 : IRequest, ICommand
    {
        public TestCommandV1(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}