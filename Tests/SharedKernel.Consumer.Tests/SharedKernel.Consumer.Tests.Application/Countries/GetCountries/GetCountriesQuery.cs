using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SharedKernel.Consumer.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace SharedKernel.Consumer.Tests.Application.Countries.GetCountries
{
    public class GetCountriesQuery : IRequest<List<CountryDto>>, IQuery
    {
        public GetCountriesQuery()
        {
        }
    }
}