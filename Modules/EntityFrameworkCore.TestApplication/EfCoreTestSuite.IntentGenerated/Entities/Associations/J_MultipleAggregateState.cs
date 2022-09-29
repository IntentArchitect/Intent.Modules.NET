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

        public Guid Id
        { get; set; }

        private string _multipleAggrAttr;

        public string MultipleAggrAttr
        {
            get { return _multipleAggrAttr; }
            set
            {
                _multipleAggrAttr = value;
            }
        }


        public Guid J_RequiredDependentId { get; set; }

        public virtual J_RequiredDependent J_RequiredDependent
        { get; set; }

        IJ_RequiredDependent IJ_MultipleAggregate.J_RequiredDependent
        {
            get => J_RequiredDependent;
            set => J_RequiredDependent = (J_RequiredDependent)value;
        }


    }
}
