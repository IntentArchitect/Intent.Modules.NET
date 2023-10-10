using System.Collections.Generic;
using System.Linq;
using CosmosDB.Application.Clients;
using CosmosDB.Application.Clients.CreateClient;
using CosmosDB.Application.Clients.UpdateClient;
using CosmosDB.Application.Common.Pagination;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public static class ClientAssertions
    {
        public static void AssertEquivalent(CreateClientCommand expectedDto, Client actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Type.Should().Be(expectedDto.Type);
            actualEntity.Name.Should().Be(expectedDto.Name);
        }

        public static void AssertEquivalent(PagedResult<ClientDto> actualDtos, IPagedResult<Client> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().Match<PagedResult<ClientDto>>(p => p == null || !p.Data.Any());
                return;
            }
            actualDtos.Data.Should().HaveSameCount(expectedEntities);
            actualDtos.PageSize.Should().Be(expectedEntities.PageSize);
            actualDtos.PageCount.Should().Be(expectedEntities.PageCount);
            actualDtos.PageNumber.Should().Be(expectedEntities.PageNo);
            actualDtos.TotalCount.Should().Be(expectedEntities.TotalCount);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var dto = actualDtos.Data.ElementAt(i);
                var entity = expectedEntities.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Identifier.Should().Be(entity.Identifier);
                dto.Type.Should().Be(entity.Type);
                dto.Name.Should().Be(entity.Name);
            }
        }

        public static void AssertEquivalent(IEnumerable<ClientDto> actualDtos, IEnumerable<Client> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().BeNullOrEmpty();
                return;
            }

            actualDtos.Should().HaveSameCount(actualDtos);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var entity = expectedEntities.ElementAt(i);
                var dto = actualDtos.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Identifier.Should().Be(entity.Identifier);
                dto.Type.Should().Be(entity.Type);
                dto.Name.Should().Be(entity.Name);
            }
        }

        public static void AssertEquivalent(ClientDto actualDto, Client expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Identifier.Should().Be(expectedEntity.Identifier);
            actualDto.Type.Should().Be(expectedEntity.Type);
            actualDto.Name.Should().Be(expectedEntity.Name);
        }

        public static void AssertEquivalent(UpdateClientCommand expectedDto, Client actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Identifier.Should().Be(expectedDto.Identifier);
            actualEntity.Type.Should().Be(expectedDto.Type);
            actualEntity.Name.Should().Be(expectedDto.Name);
        }
    }
}