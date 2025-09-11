using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Application.DynAffiliates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AwsLambdaFunction.Application.Interfaces
{
    public interface IDynAffiliatesService
    {
        Task<string> CreateDynAffiliate(CreateDynAffiliateDto dto, CancellationToken cancellationToken = default);
        Task UpdateDynAffiliate(string id, UpdateDynAffiliateDto dto, CancellationToken cancellationToken = default);
        Task<DynAffiliateDto> FindAffiliateById(string id, CancellationToken cancellationToken = default);
        Task<List<DynAffiliateDto>> FindDynAffiliates(CancellationToken cancellationToken = default);
        Task DeleteDynAffiliate(string id, CancellationToken cancellationToken = default);
    }
}