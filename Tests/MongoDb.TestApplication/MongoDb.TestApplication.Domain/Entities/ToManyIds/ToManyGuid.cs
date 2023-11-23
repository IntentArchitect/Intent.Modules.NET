using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.ToManyIds
{
    public class ToManyGuid
    {
        private Guid? _id;

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }
    }
}