using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.ToManyIds
{
    public class ToManyInt
    {
        private int? _id;

        public int Id
        {
            get => _id ?? throw new NullReferenceException("_id has not been set");
            set => _id = value;
        }
    }
}