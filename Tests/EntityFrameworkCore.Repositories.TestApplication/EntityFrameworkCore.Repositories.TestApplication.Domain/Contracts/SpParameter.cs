using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DataContract", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts
{
    public record SpParameter
    {
        public SpParameter(byte[] attributeBinary,
            bool attributeBool,
            byte attributeByte,
            DateTime attributeDate,
            DateTime attributeDateTime,
            DateTimeOffset attributeDateTimeOffset,
            decimal attributeDecimal,
            double attributeDouble,
            float attributeFloat,
            Guid attributeGuid,
            int attributeInt,
            long attributeLong,
            short attributeShort,
            string attributeString)
        {
            AttributeBinary = attributeBinary;
            AttributeBool = attributeBool;
            AttributeByte = attributeByte;
            AttributeDate = attributeDate;
            AttributeDateTime = attributeDateTime;
            AttributeDateTimeOffset = attributeDateTimeOffset;
            AttributeDecimal = attributeDecimal;
            AttributeDouble = attributeDouble;
            AttributeFloat = attributeFloat;
            AttributeGuid = attributeGuid;
            AttributeInt = attributeInt;
            AttributeLong = attributeLong;
            AttributeShort = attributeShort;
            AttributeString = attributeString;
        }

        public byte[] AttributeBinary { get; init; }
        public bool AttributeBool { get; init; }
        public byte AttributeByte { get; init; }
        public DateTime AttributeDate { get; init; }
        public DateTime AttributeDateTime { get; init; }
        public DateTimeOffset AttributeDateTimeOffset { get; init; }
        public decimal AttributeDecimal { get; init; }
        public double AttributeDouble { get; init; }
        public float AttributeFloat { get; init; }
        public Guid AttributeGuid { get; init; }
        public int AttributeInt { get; init; }
        public long AttributeLong { get; init; }
        public short AttributeShort { get; init; }
        public string AttributeString { get; init; }
    }
}