using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Unversioned.Test
{
    public class TestQuery : IRequest<int>, IQuery
    {
        public TestQuery(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}