using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreTestSuite.IntentGenerated.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfCoreTestSuite.IntentGenerated.Entities.Associations
{

    public partial class C_RequiredComposite : IC_RequiredComposite
    {

        public Guid Id { get; set; }

        public string RequiredCompAttr { get; set; }

        public virtual ICollection<C_MultipleDependent> CMultipleDependents { get; set; } = new List<C_MultipleDependent>();

        ICollection<IC_MultipleDependent> IC_RequiredComposite.CMultipleDependents
        {
            get => CMultipleDependents.CreateWrapper<IC_MultipleDependent, C_MultipleDependent>();
            set => CMultipleDependents = value.Cast<C_MultipleDependent>().ToList();
        }


    }
}
