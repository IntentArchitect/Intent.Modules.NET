using System;
using System.Collections.Generic;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class J_RequiredDependent : IJ_RequiredDependent
    {

        public Guid Id
        { get; set; }

        private string _reqDepAttr;

        public string ReqDepAttr
        {
            get { return _reqDepAttr; }
            set
            {
                _reqDepAttr = value;
            }
        }


    }
}
