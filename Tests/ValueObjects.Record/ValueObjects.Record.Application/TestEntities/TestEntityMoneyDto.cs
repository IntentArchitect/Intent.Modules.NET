using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using ValueObjects.Record.Application.Common.Mappings;
using ValueObjects.Record.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities
{
    public class TestEntityMoneyDto : IMapFrom<Money>
    {
        public TestEntityMoneyDto()
        {
            Currency = null!;
        }

        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public static TestEntityMoneyDto Create(decimal amount, string currency)
        {
            return new TestEntityMoneyDto
            {
                Amount = amount,
                Currency = currency
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Money, TestEntityMoneyDto>();
        }
    }
}