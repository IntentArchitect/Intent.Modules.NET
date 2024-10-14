using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildren
{
    public class GetParentWithAnemicChildrenQuery : IRequest<List<ParentWithAnemicChildDto>>, IQuery
    {
        public GetParentWithAnemicChildrenQuery()
        {
        }
    }
}