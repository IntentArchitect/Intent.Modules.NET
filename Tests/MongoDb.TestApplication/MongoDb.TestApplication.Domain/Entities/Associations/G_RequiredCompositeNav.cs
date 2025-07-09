using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    public class G_RequiredCompositeNav
    {
        public G_RequiredCompositeNav()
        {
            Id = null!;
            Attribute = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public ICollection<G_MultipleDependent> G_MultipleDependents { get; set; } = [];
    }
}