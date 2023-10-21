using System;
using Intent.RoslynWeaver.Attributes;

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class A_RequiredComposite
    {
        public string Id { get; set; }

        public string ReqCompAttribute { get; set; }

        public A_OptionalDependent? A_OptionalDependent { get; set; }
    }
}