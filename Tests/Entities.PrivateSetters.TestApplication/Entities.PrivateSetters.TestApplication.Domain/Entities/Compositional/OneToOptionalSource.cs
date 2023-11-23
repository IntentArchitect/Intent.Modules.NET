using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    public class OneToOptionalSource
    {
        public OneToOptionalSource(string attribute, OneToOptionalDest? oneToOptionalDest)
        {
            Attribute = attribute;
            OneToOptionalDest = oneToOptionalDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OneToOptionalSource()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OneToOptionalDest? OneToOptionalDest { get; private set; }

        public void Operation(string attribute, OneToOptionalDest? oneToOptionalDest)
        {
            Attribute = attribute;
            OneToOptionalDest = oneToOptionalDest;
        }
    }
}