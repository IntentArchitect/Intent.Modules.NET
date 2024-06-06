using System.Collections.Generic;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.WithCompositeKeys.GetWithCompositeKeys
{
    public class GetWithCompositeKeysQuery : IRequest<List<WithCompositeKeyDto>>, IQuery
    {
        public GetWithCompositeKeysQuery()
        {
        }
    }
}