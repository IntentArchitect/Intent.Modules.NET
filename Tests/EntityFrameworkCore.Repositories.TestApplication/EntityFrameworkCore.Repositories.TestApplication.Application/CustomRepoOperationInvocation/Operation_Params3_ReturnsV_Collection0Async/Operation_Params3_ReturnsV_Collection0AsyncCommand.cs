using System;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CustomRepoOperationInvocation.Operation_Params3_ReturnsV_Collection0Async
{
    public class Operation_Params3_ReturnsV_Collection0AsyncCommand : IRequest, ICommand
    {
        public Operation_Params3_ReturnsV_Collection0AsyncCommand(byte[] attributeBinary,
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
            string attributeString,
            Guid id,
            string tag,
            string strParam)
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
            Id = id;
            Tag = tag;
            StrParam = strParam;
        }

        public byte[] AttributeBinary { get; set; }
        public bool AttributeBool { get; set; }
        public byte AttributeByte { get; set; }
        public DateTime AttributeDate { get; set; }
        public DateTime AttributeDateTime { get; set; }
        public DateTimeOffset AttributeDateTimeOffset { get; set; }
        public decimal AttributeDecimal { get; set; }
        public double AttributeDouble { get; set; }
        public float AttributeFloat { get; set; }
        public Guid AttributeGuid { get; set; }
        public int AttributeInt { get; set; }
        public long AttributeLong { get; set; }
        public short AttributeShort { get; set; }
        public string AttributeString { get; set; }
        public Guid Id { get; set; }
        public string Tag { get; set; }
        public string StrParam { get; set; }
    }
}