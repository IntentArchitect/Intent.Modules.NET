using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.DomainServices
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