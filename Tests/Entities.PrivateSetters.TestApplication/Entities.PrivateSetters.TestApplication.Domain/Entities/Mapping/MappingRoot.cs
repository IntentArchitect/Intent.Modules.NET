using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Mapping
{
    public class MappingRoot
    {
        private List<MappingCompositeMultiple> _mappingCompositeMultiples = new List<MappingCompositeMultiple>();

        public MappingRoot(IEnumerable<MappingCompositeMultiple> mappingCompositeMultiples,
            MappingCompositeSingle mappingCompositeSingle)
        {
            _mappingCompositeMultiples.Clear();
            _mappingCompositeMultiples.AddRange(mappingCompositeMultiples);
            MappingCompositeSingle = mappingCompositeSingle;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected MappingRoot()
        {
            MappingCompositeSingle = null!;
        }

        public Guid Id { get; private set; }

        public virtual IReadOnlyCollection<MappingCompositeMultiple> MappingCompositeMultiples
        {
            get => _mappingCompositeMultiples.AsReadOnly();
            private set => _mappingCompositeMultiples = new List<MappingCompositeMultiple>(value);
        }

        public virtual MappingCompositeSingle MappingCompositeSingle { get; private set; }

        public void SetSingle(MappingCompositeSingle mappingCompositeSingle)
        {
            MappingCompositeSingle = mappingCompositeSingle;
        }

        public void SetMultiple(IEnumerable<MappingCompositeMultiple> mappingCompositeMultiples)
        {
            _mappingCompositeMultiples.Clear();
            _mappingCompositeMultiples.AddRange(mappingCompositeMultiples);
        }
    }
}