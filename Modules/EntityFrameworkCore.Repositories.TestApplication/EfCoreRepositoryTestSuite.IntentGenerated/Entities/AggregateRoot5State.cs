using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreRepositoryTestSuite.IntentGenerated.Entities
{

    public partial class AggregateRoot5 : IAggregateRoot5
    {

        public Guid Id { get; set; }

        public virtual AggregateRoot5EntityWithRepo AggregateRoot5EntityWithRepo { get; set; }

        IAggregateRoot5EntityWithRepo IAggregateRoot5.AggregateRoot5EntityWithRepo
        {
            get => AggregateRoot5EntityWithRepo;
            set => AggregateRoot5EntityWithRepo = (AggregateRoot5EntityWithRepo)value;
        }


    }
}
