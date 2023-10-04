using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ScalarCollectionReturnType.QueryWithCollectionReturn
{
    public class QueryWithCollectionReturn : IRequest<List<string>>, IQuery
    {
        public QueryWithCollectionReturn()
        {
        }
    }
}