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
        public J_MultipleAggregate()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

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
        private J_RequiredDependent _j_RequiredDependent;

        public virtual J_RequiredDependent J_RequiredDependent
        {
            get
            {
                return _j_RequiredDependent;
            }
            set
            {
                _j_RequiredDependent = value;
            }
        }


    }
}
