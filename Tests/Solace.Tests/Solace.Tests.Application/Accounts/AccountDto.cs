using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application.Common.Mappings;
using Solace.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Solace.Tests.Application.Accounts
{
    public class AccountDto : IMapFrom<Account>
    {
        public AccountDto()
        {
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public static AccountDto Create(Guid id, Guid customerId)
        {
            return new AccountDto
            {
                Id = id,
                CustomerId = customerId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Account, AccountDto>();
        }
    }
}