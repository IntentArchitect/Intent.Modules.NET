using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Versioned.TestCommandV2
{
    /// <summary>
    /// Command comment
    /// </summary>
    public class TestCommandV2 : IRequest, ICommand
    {
        public TestCommandV2(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}