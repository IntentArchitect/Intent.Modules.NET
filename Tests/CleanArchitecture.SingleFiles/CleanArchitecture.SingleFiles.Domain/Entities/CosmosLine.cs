using System;
using Intent.RoslynWeaver.Attributes;

namespace CleanArchitecture.SingleFiles.Domain.Entities
{
    public class CosmosLine
    {
        private string? _id;

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }
    }
}