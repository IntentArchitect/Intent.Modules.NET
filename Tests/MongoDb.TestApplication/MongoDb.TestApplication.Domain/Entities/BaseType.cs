using System;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities
{
    public abstract class BaseType
    {
        public string Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}