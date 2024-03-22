using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices
{
    public class ConcreteEntityB : BaseEntityB
    {
        public ConcreteEntityB(string baseAttr, string concreteAttr)
        {
            BaseAttr = baseAttr;
            ConcreteAttr = concreteAttr;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected ConcreteEntityB()
        {
            ConcreteAttr = null!;
        }

        public string ConcreteAttr { get; set; }
    }
}