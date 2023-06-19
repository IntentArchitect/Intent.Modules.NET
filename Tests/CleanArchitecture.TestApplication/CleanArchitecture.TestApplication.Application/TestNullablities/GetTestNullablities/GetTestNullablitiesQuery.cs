using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.TestNullablities.GetTestNullablities
{
    public class GetTestNullablitiesQuery : IRequest<List<TestNullablityDto>>, IQuery
    {
        public GetTestNullablitiesQuery()
        {
        }
    }
}