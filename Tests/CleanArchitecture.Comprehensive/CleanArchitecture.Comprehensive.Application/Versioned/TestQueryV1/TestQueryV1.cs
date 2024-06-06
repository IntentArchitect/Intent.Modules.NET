using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Versioned.TestQueryV1
{
    public class TestQueryV1 : IRequest<int>, IQuery
    {
        public TestQueryV1(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}