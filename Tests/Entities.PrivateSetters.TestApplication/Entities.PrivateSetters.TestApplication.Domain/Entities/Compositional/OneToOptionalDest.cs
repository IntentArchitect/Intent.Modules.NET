using System;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    public class OneToOptionalDest
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OneToOptionalDest()
        {
            Attribute = null!;
            OneToOptionalSource = null!;
        }

        public OneToOptionalDest(string attribute)
        {
            Attribute = attribute;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OneToOptionalSource OneToOptionalSource { get; private set; }

        public void Operation(string attribute)
        {
            Attribute = attribute;
        }
    }
}