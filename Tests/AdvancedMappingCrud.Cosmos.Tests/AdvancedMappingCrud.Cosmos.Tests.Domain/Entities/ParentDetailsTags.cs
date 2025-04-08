using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Domain.Entities
{
    public class ParentDetailsTags
    {
        private string? _id;

        public ParentDetailsTags()
        {
            Id = null!;
            TagName = null!;
            TagValue = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string TagName { get; set; }

        public string TagValue { get; set; }
    }
}