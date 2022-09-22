using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class C_RequiredComposite : IC_RequiredComposite
    {
        public C_RequiredComposite()
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

        private string _requiredCompAttr;

        public string RequiredCompAttr
        {
            get { return _requiredCompAttr; }
            set
            {
                _requiredCompAttr = value;
            }
        }

        private ICollection<C_MultipleDependent> _c_MultipleDependents;

        public virtual ICollection<C_MultipleDependent> C_MultipleDependents
        {
            get
            {
                return _c_MultipleDependents ??= new List<C_MultipleDependent>();
            }
            set
            {
                _c_MultipleDependents = value;
            }
        }


    }
}
