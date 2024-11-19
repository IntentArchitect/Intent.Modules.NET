using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.ODataQuery
{
    public class ODataChild
    {
        public ODataChild()
        {
            No = null!;
        }
        public Guid Id { get; set; }

        public string No { get; set; }

        public Guid ODataAggId { get; set; }
    }
}