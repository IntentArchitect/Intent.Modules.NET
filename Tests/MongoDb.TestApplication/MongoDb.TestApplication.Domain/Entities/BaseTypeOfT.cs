using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities
{
    public abstract class BaseTypeOfT<T>
    {
        public string Id { get; set; }

        public T BaseAttribute { get; set; }
    }
}