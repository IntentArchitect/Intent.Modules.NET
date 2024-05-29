using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.Common.Pagination;
using CleanArchitecture.Comprehensive.Application.Pagination;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using CleanArchitecture.Comprehensive.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Pagination.LogEntries
{
    public static class LogEntryAssertions
    {
        public static void AssertEquivalent(PagedResult<LogEntryDto> actualDtos, IPagedList<LogEntry> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().Match<PagedResult<LogEntryDto>>(p => p == null || !p.Data.Any());
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
                dto.Id.Should().Be(entity.Id);
                dto.Message.Should().Be(entity.Message);
                dto.TimeStamp.Should().Be(entity.TimeStamp);
            }
        }
    }
}