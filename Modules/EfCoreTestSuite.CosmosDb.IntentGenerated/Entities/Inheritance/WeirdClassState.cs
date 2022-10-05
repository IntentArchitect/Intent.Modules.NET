using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Inheritance
{

    public partial class WeirdClass : Composite, IWeirdClass
    {
        public WeirdClass()
        {
        }


        private string _partitionKey;

        public string PartitionKey
        {
            get { return _partitionKey; }
            set
            {
                _partitionKey = value;
            }
        }

        private string _weirdField;

        public string WeirdField
        {
            get { return _weirdField; }
            set
            {
                _weirdField = value;
            }
        }

    }
}
