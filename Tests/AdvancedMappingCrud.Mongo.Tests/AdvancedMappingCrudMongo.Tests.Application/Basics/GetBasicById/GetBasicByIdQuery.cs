using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Basics.GetBasicById
{
    public class GetBasicByIdQuery : IRequest<BasicDto>, IQuery
    {
        public GetBasicByIdQuery(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}