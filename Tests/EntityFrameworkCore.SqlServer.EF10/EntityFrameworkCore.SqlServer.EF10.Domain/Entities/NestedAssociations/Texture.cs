using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.EF10.Domain.Entities.NestedAssociations
{
    public class Texture
    {
        public Texture()
        {
            TextureAttribute = null!;
        }
        public Guid Id { get; set; }

        public string TextureAttribute { get; set; }
    }
}