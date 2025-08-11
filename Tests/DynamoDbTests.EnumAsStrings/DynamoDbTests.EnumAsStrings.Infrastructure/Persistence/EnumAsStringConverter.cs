using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.EnumAsStringConverter", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence
{
    internal class EnumAsStringConverter<TEnum> : IPropertyConverter
        where TEnum : struct
    {
        public DynamoDBEntry ToEntry(object value)
        {
            return new Primitive(value.ToString());
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            return Enum.Parse<TEnum>(entry.AsString());
        }
    }
}