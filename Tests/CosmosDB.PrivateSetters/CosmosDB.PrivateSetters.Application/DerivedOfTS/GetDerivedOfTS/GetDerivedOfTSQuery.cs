using System.Collections.Generic;
using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.DerivedOfTS.GetDerivedOfTS
{
    public class GetDerivedOfTSQuery : IRequest<List<DerivedOfTDto>>, IQuery
    {
        public GetDerivedOfTSQuery()
        {
        }
    }
}