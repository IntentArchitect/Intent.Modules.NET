using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class J_MultipleAggregate : IJ_MultipleAggregate
    {

        public Guid Id { get; set; }

        public string MultipleAggrAttr { get; set; }

        public Guid JRequiredDependentId { get; set; }

        public virtual J_RequiredDependent JRequiredDependent { get; set; }

        IJ_RequiredDependent IJ_MultipleAggregate.JRequiredDependent
        {
            get => JRequiredDependent;
            set => JRequiredDependent = (J_RequiredDependent)value;
        }


    }
}
