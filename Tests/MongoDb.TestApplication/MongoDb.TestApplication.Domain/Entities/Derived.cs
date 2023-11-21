using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities
{
    public class Derived : BaseType
    {
        public string DerivedAttribute { get; set; }
    }
}