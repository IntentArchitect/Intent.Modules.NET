using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Entities.Common;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.CreateA_AggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateA_AggregateRootCommandHandler : IRequestHandler<CreateA_AggregateRootCommand, Guid>
    {
        private IA_AggregateRootRepository _a_AggregateRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateA_AggregateRootCommandHandler(IA_AggregateRootRepository a_AggregateRootRepository)
        {
            _a_AggregateRootRepository = a_AggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateA_AggregateRootCommand request, CancellationToken cancellationToken)
        {
            var newA_AggregateRoot = new A_AggregateRoot
            {
                Id = IdentityGenerator.NewSequentialId(),
                AggregateAttr = request.AggregateAttr,
                Composite = request.Composite != null
                    ? new A_Composite_Single
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        CompositeAttr = request.Composite.CompositeAttr,
                        Composite = request.Composite.Composite != null
                            ? new AA1_Composite_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = request.Composite.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = request.Composite.Composites?.Select(composites =>
                            new AA1_Composite_Many
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                            }).ToList(),
                        Aggregation = request.Composite.Aggregation != null
                            ? new AA1_Aggregation_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                AggregationAttr = request.Composite.Aggregation.AggregationAttr,
                            }
                            : null,
                    }
                    : null,
                Composites = request.Composites?.Select(composites =>
                    new A_Composite_Many
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        CompositeAttr = composites.CompositeAttr,
                        Composite = composites.Composite != null
                            ? new AA2_Composite_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = composites.Composites?.Select(composites =>
                            new AA2_Composite_Many
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                            }).ToList(),
                        Aggregation = composites.Aggregation != null
                            ? new AA2_Aggregation_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                AggregationAttr = composites.Aggregation.AggregationAttr,
                            }
                            : null,
                    }).ToList(),
                Aggregation = request.Aggregation != null
                    ? new A_Aggregation_Single
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        AggregationAttr = request.Aggregation.AggregationAttr,
                        Composite = request.Aggregation.Composite != null
                            ? new AA3_Composite_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = request.Aggregation.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = request.Aggregation.Composites?.Select(composites =>
                            new AA3_Composite_Many
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                            }).ToList(),
                        Aggregation = request.Aggregation.Aggregation != null
                            ? new AA3_Aggregation_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                AggregationAttr = request.Aggregation.Aggregation.AggregationAttr,
                            }
                            : null,
                        Aggregations = request.Aggregation.Aggregations?.Select(aggregations =>
                            new AA3_Aggregation_Many
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                AggregationAttr = aggregations.AggregationAttr,
                            }).ToList(),
                    }
                    : null,
                Aggregations = request.Aggregations?.Select(aggregations =>
                    new A_Aggregation_Many
                    {
                        Id = IdentityGenerator.NewSequentialId(),
                        AggregationAttr = aggregations.AggregationAttr,
                        Composite = aggregations.Composite != null
                            ? new AA4_Composite_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = aggregations.Composite.CompositeAttr,
                            }
                            : null,
                        Composites = aggregations.Composites?.Select(composites =>
                            new AA4_Composite_Many
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                CompositeAttr = composites.CompositeAttr,
                            }).ToList(),
                        Aggregation = aggregations.Aggregation != null
                            ? new AA4_Aggregation_Single
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                AggregationAttr = aggregations.Aggregation.AggregationAttr,
                            }
                            : null,
                        Aggregations = aggregations.Aggregations?.Select(aggregations =>
                            new AA4_Aggregation_Many
                            {
                                Id = IdentityGenerator.NewSequentialId(),
                                AggregationAttr = aggregations.AggregationAttr,
                            }).ToList(),
                    }).ToList(),
            };

            _a_AggregateRootRepository.Add(newA_AggregateRoot);

            await _a_AggregateRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newA_AggregateRoot.Id;
        }
    }
}