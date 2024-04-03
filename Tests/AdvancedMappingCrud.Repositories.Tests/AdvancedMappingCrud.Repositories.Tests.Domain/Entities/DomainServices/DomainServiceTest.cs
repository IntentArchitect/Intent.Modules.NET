using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices
{
    public class DomainServiceTest : IHasDomainEvent
    {
        public DomainServiceTest(IMyDomainService service)
        {
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected DomainServiceTest()
        {
        }

        public Guid Id { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void MyOp(IMyDomainService service)
        {
            // [IntentFully]
            throw new NotImplementedException("Replace with your implementation...");
        }
    }
}