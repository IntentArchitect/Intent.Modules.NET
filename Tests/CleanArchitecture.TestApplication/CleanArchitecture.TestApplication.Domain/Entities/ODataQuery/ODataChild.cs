using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.TestApplication.Domain.Entities.ODataQuery
{
    public class ODataChild
    {
        public Guid Id { get; set; }

        public string No { get; set; }

        public Guid ODataAggId { get; set; }
    }
}