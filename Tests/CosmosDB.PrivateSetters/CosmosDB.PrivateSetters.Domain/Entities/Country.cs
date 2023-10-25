using System;
using Intent.RoslynWeaver.Attributes;

namespace CosmosDB.PrivateSetters.Domain.Entities
{
    public class Country
    {
        private int? _id;

        public int Id
        {
            get => _id ?? throw new NullReferenceException("_id has not been set");
            private set => _id = value;
        }

        public string Name { get; private set; }
    }
}