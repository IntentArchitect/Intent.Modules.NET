using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.CosmosDb.IntentGenerated.Entities.Polymorphic
{

    public partial class Poly_BaseClassNonAbstract : Poly_RootAbstract, IPoly_BaseClassNonAbstract
    {
        public Poly_BaseClassNonAbstract()
        {
        }


        private string _baseField;

        public string BaseField
        {
            get { return _baseField; }
            set
            {
                _baseField = value;
            }
        }


        public Guid? Poly_SecondLevelId { get; set; }
    }
}
