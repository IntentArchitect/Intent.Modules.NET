using System.Collections.Generic;
using AzureFunctions.NET8.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET8.Application.ResponseCodes.GetResponseCodes
{
    public class GetResponseCodesQuery : IRequest<List<ResponseCodeDto>>, IQuery
    {
        public GetResponseCodesQuery()
        {
        }
    }
}