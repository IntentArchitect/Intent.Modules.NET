using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.Folder
{
    public class Folder : IHasDomainEvent
    {
        public Folder(string name, string code)
        {
            Name = name;
            Code = code;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Folder()
        {
            Name = null!;
            Code = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = [];
    }
}