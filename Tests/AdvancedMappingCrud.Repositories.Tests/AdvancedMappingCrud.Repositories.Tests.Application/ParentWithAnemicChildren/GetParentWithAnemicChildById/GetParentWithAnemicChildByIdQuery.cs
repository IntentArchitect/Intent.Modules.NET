using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren.GetParentWithAnemicChildById
{
    public class GetParentWithAnemicChildByIdQuery : IRequest<ParentWithAnemicChildDto>, IQuery
    {
        public GetParentWithAnemicChildByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}