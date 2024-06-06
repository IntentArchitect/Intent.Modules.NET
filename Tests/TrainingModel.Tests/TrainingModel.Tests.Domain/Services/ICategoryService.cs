using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace TrainingModel.Tests.Domain.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetOrCreateCategoriesAsync(IEnumerable<string> categoryNames, CancellationToken cancellationToken = default);
    }
}