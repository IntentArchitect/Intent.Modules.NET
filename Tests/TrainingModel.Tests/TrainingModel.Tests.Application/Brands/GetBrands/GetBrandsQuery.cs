using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TrainingModel.Tests.Application.Brands.GetBrands
{
    public class GetBrandsQuery : IRequest<List<BrandDto>>, IQuery
    {
        public GetBrandsQuery()
        {
        }
    }
}