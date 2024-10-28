using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Versioned.TestQueryV1
{
    public class TestQueryV1 : IRequest<int>, IQuery
    {
        public TestQueryV1(string value)
        {
            Value = value;
        }

        [FromQueryParams]
        public string Value { get; set; }
    }
}