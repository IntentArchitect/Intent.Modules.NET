using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Unversioned.Test
{
    public class TestQuery : IRequest<int>, IQuery
    {
        public TestQuery(string value)
        {
            Value = value;
        }

        [FromQueryParams]
        public string Value { get; set; }
    }
}