using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities
{
    public abstract class BaseType
    {
        public string Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}