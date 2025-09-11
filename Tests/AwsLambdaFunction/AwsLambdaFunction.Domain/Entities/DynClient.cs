using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AwsLambdaFunction.Domain.Entities
{
    public class DynClient
    {
        private string? _id;

        public DynClient()
        {
            Id = null!;
            Name = null!;
            AffiliateId = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string AffiliateId { get; set; }

        public ICollection<DynSite> Sites { get; set; } = [];
    }
}