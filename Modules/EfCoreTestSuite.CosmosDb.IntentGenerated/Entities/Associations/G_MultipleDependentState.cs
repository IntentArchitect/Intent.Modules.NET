using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Associations
{

    public partial class G_MultipleDependent : IG_MultipleDependent
    {
        public G_MultipleDependent()
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

        private string _multipleDepAttr;

        public string MultipleDepAttr
        {
            get { return _multipleDepAttr; }
            set
            {
                _multipleDepAttr = value;
            }
        }


        public Guid G_RequiredCompositeNavId { get; set; }
        private G_RequiredCompositeNav _g_RequiredCompositeNav;

        public virtual G_RequiredCompositeNav G_RequiredCompositeNav
        {
            get
            {
                return _g_RequiredCompositeNav;
            }
            set
            {
                _g_RequiredCompositeNav = value;
            }
        }


    }
}
