using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.GetAllImplementation.Customers
{
    /// <summary>
    /// 5978511809 - GetAllImplementation strategy thinks it should apply even for unmapped queries
    /// </summary>
    public class GetallimplementationCustomerGetallimplementationOrderDto : IMapFrom<GetAllImplementationOrder>
    {
        public GetallimplementationCustomerGetallimplementationOrderDto()
        {
            GetallimplementationCustomerid = null!;
        }

        public string GetallimplementationCustomerid { get; set; }

        public static GetallimplementationCustomerGetallimplementationOrderDto Create(string getallimplementationCustomerid)
        {
            return new GetallimplementationCustomerGetallimplementationOrderDto
            {
                GetallimplementationCustomerid = getallimplementationCustomerid
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GetAllImplementationOrder, GetallimplementationCustomerGetallimplementationOrderDto>();
        }
    }
}