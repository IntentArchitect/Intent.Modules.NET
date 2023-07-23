using System.Collections.Generic;
using CleanArchitecture.Dapr.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS.GetDerivedOfTS
{
    public class GetDerivedOfTSQuery : IRequest<List<DerivedOfTDto>>, IQuery
    {
        public GetDerivedOfTSQuery()
        {
        }
    }
}