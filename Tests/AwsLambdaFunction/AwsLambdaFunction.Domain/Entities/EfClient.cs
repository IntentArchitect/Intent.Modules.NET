using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AwsLambdaFunction.Domain.Entities
{
    public class EfClient
    {
        public EfClient()
        {
            Name = null!;
            Affiliate = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid AffiliateId { get; set; }

        public virtual ICollection<EfSite> Sites { get; set; } = [];

        public virtual EfAffiliate Affiliate { get; set; }
    }
}