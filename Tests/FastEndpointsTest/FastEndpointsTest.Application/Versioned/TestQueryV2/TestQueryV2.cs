using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Versioned.TestQueryV2
{
    public class TestQueryV2 : IRequest<int>, IQuery
    {
        public TestQueryV2(string value)
        {
            Value = value;
        }

        [QueryParam]
        public string Value { get; set; }
    }
}