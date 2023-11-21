using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities
{
    public class DerivedOfT : BaseTypeOfT<int>
    {
        public string DerivedAttribute { get; set; }
    }
}