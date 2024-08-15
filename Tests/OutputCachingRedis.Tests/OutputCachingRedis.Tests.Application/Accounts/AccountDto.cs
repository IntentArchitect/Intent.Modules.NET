using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using OutputCachingRedis.Tests.Application.Common.Mappings;
using OutputCachingRedis.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OutputCachingRedis.Tests.Application.Accounts
{
    public class AccountDto : IMapFrom<Account>
    {
        public AccountDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static AccountDto Create(Guid id, string name)
        {
            return new AccountDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Account, AccountDto>();
        }
    }
}