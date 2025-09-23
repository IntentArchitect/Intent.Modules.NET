using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Application.EfAffiliates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AwsLambdaFunction.Application.Interfaces
{
    public interface IEfAffiliatesService
    {
        Task<Guid> CreateEfAffiliate(CreateEfAffiliateDto dto, CancellationToken cancellationToken = default);
        Task UpdateEfAffiliate(Guid id, UpdateEfAffiliateDto dto, CancellationToken cancellationToken = default);
        Task<EfAffiliateDto> FindEfAffiliateById(Guid id, CancellationToken cancellationToken = default);
        Task<List<EfAffiliateDto>> FindEfAffiliates(CancellationToken cancellationToken = default);
        Task DeleteEfAffiliate(Guid id, CancellationToken cancellationToken = default);
    }
}