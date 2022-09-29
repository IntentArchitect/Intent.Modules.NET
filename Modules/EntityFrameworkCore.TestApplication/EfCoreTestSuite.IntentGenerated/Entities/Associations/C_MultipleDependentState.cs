using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class C_MultipleDependent : IC_MultipleDependent
    {

        private string _multipleDepAttr;

        public string MultipleDepAttr
        {
            get { return _multipleDepAttr; }
            set
            {
                _multipleDepAttr = value;
            }
        }


        public Guid C_RequiredCompositeId { get; set; }

    }
}
