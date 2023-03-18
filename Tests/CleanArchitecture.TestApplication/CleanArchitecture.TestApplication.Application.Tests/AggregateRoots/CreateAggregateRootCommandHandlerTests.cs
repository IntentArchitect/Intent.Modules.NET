using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var testCommand = CreateTestCommand();
            yield return new object[] { testCommand };
        }
        
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateRootToRepository(CreateAggregateRootCommand testCommand)
        {
            // Arrange
            var expectedAggregateRootId = Guid.NewGuid();
            
            AggregateRoot addedAggregateRoot = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.OnAdd(ent => addedAggregateRoot = ent);
            repository.OnSaveChanges(() => addedAggregateRoot.Id = expectedAggregateRootId);

            var validator = GetValidationBehaviour();
            var sut = new CreateAggregateRootCommandHandler(repository);

            // Act
            var result = await validator.Handle(
                request: testCommand, 
                cancellationToken: CancellationToken.None, 
                next: () => sut.Handle(testCommand, CancellationToken.None));
            
            // Assert
            result.Should().Be(expectedAggregateRootId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            addedAggregateRoot.Should().BeEquivalentTo(testCommand, c => c
                .Excluding(x => x.Aggregate));

            AssertEquivalent(testCommand, addedAggregateRoot);
        }
        
        public static IEnumerable<object[]> GetFailingResultTestData()
        {
            CreateAggregateRootCommand testCommand;
            testCommand = CreateTestCommand();
            testCommand.AggregateAttr = null;
            yield return new object[] { testCommand, "AggregateAttr" };
            
            testCommand = CreateTestCommand();
            testCommand.Composites = null;
            yield return new object[] { testCommand, "Composites" };
        }

        [Theory]
        [MemberData(nameof(GetFailingResultTestData))]
        public async Task Handle_WithInvalidCommand_ThrowsException(CreateAggregateRootCommand testCommand, string propertyName)
        {
            // Arrange
            var expectedAggregateRootId = Guid.NewGuid();
            
            AggregateRoot addedAggregateRoot = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.OnAdd(ent => addedAggregateRoot = ent);
            repository.OnSaveChanges(() => addedAggregateRoot.Id = expectedAggregateRootId);
        
            var validator = GetValidationBehaviour();
            var sut = new CreateAggregateRootCommandHandler(repository);
            
            // Act
            var act = async () =>
            {
                await validator.Handle(
                    request: testCommand, 
                    cancellationToken: CancellationToken.None, 
                    next: () => sut.Handle(testCommand, CancellationToken.None));
            };
            
            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
                .And.Errors.Should().Contain(p => p.PropertyName == propertyName);
        }

        private ValidationBehaviour<CreateAggregateRootCommand, Guid> GetValidationBehaviour()
        {
            return new ValidationBehaviour<CreateAggregateRootCommand, Guid>(new[] { new CreateAggregateRootCommandValidator() });
        }

        private void AssertEquivalent(CreateAggregateRootCommand expectedDto, AggregateRoot actualEntity)
        {
            actualEntity.Should().NotBeNull();
            actualEntity.AggregateAttr.Should().Be(expectedDto.AggregateAttr);
            actualEntity.Composite.Should().NotBeNull();
            actualEntity.Composite.CompositeAttr.Should().Be(expectedDto.Composite.CompositeAttr);
            actualEntity.Composite.Composite.Should().NotBeNull();
            actualEntity.Composite.Composite.CompositeAttr.Should().Be(expectedDto.Composite.Composite.CompositeAttr);

            AssertEquivalent(expectedDto.Composites, actualEntity.Composites);
            AssertEquivalent(expectedDto.Composite.Composites, actualEntity.Composite.Composites);
        }

        private void AssertEquivalent(
            IEnumerable<CreateAggregateRootCompositeManyBDto> expectedDtos, IEnumerable<CompositeManyB> actualEntities)
        {
            actualEntities.Should().HaveSameCount(actualEntities);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
                entity.Composite.CompositeAttr.Should().Be(dto.Composite.CompositeAttr);
                AssertEquivalent(entity.Composites, dto.Composites);
            }
        }

        private void AssertEquivalent(IEnumerable<CompositeManyBB> expectedDtos, IEnumerable<CreateAggregateRootCompositeManyBCompositeManyBBDto> actualEntities)
        {
            actualEntities.Should().HaveSameCount(actualEntities);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            }
        }

        private void AssertEquivalent(
            IEnumerable<CreateAggregateRootCompositeSingleACompositeManyAADto> expectedDtos, IEnumerable<CompositeManyAA> actualEntities)
        {
            actualEntities.Should().HaveSameCount(actualEntities);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                entity.CompositeAttr.Should().Be(dto.CompositeAttr);
            }
        }

        private static CreateAggregateRootCommand CreateTestCommand()
        {
            var testCommand = new CreateAggregateRootCommand
            {
                AggregateAttr = "Test 598212A6-28A8-42B7-AF8C-443CC8D29890",
                Composite = new CreateAggregateRootCompositeSingleADto
                {
                    CompositeAttr = "Test A9EFA644-1E70-47D0-A82C-22DA89FC501B",
                    Composite = new CreateAggregateRootCompositeSingleACompositeSingleAADto
                    {
                        CompositeAttr = "Test A94D6413-EC9C-4F90-860C-F1C1FC1C5C62",
                    },
                    Composites = new List<CreateAggregateRootCompositeSingleACompositeManyAADto>
                    {
                        new()
                        {
                            CompositeAttr = "Test 1722280D-DE69-4058-9549-A7E6644BA90E"
                        },
                        new()
                        {
                            CompositeAttr = "Test 915C3CF6-BDF9-453E-BC07-4C641CCF398C"
                        },
                        new()
                        {
                            CompositeAttr = "Test F9D8622B-48FE-4753-AEE8-F11117608DC0"
                        }
                    }
                },
                Composites = new List<CreateAggregateRootCompositeManyBDto>
                {
                    new()
                    {
                        CompositeAttr = "Test 8E36FE61-E856-4693-B1B2-FD15C84E489D",
                        SomeDate = DateTime.Now,
                        Composite = new CreateAggregateRootCompositeManyBCompositeSingleBBDto
                        {
                            CompositeAttr = "Test 26F92331-01AA-4AD8-97C2-D9CEB2311118"
                        },
                        Composites = new List<CreateAggregateRootCompositeManyBCompositeManyBBDto>
                        {
                            new()
                            {
                                CompositeAttr = "Test 67484EA2-FC76-49C9-B017-1A1523FDDC48"
                            },
                            new()
                            {
                                CompositeAttr = "Test 6FE6063EE-83AF-4A74-B193-DF209DBE61F6"
                            },
                            new()
                            {
                                CompositeAttr = "Test EA133B76-B8BD-4681-A286-7D67DDC0DE16"
                            }
                        }
                    }
                }
            };
            return testCommand;
        }
    }
}