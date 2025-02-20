using GrpcServer.Application.Common.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application
{
    public class TypeTestDto
    {
        public TypeTestDto()
        {
            BinaryField = null!;
            BinaryFieldCollection = null!;
            BoolFieldCollection = null!;
            ByteFieldCollection = null!;
            CharFieldCollection = null!;
            ComplexTypeField = null!;
            ComplexTypeFieldCollection = null!;
            DateOnlyFieldCollection = null!;
            DateTimeFieldCollection = null!;
            DateTimeOffsetFieldCollection = null!;
            DecimalFieldCollection = null!;
            DictionaryField = null!;
            DictionaryFieldCollection = null!;
            DoubleFieldCollection = null!;
            FloatFieldCollection = null!;
            GuidFieldCollection = null!;
            IntFieldCollection = null!;
            LongFieldCollection = null!;
            ObjectField = null!;
            ObjectFieldCollection = null!;
            PagedResultField = null!;
            ShortFieldCollection = null!;
            StringField = null!;
            StringFieldCollection = null!;
            TimeSpanFieldCollection = null!;
        }

        public byte[] BinaryField { get; set; }
        public List<byte[]> BinaryFieldCollection { get; set; }
        public byte[]? BinaryFieldNullable { get; set; }
        public List<byte[]>? BinaryFieldNullableCollection { get; set; }
        public bool BoolField { get; set; }
        public List<bool> BoolFieldCollection { get; set; }
        public bool? BoolFieldNullable { get; set; }
        public List<bool>? BoolFieldNullableCollection { get; set; }
        public byte ByteField { get; set; }
        public List<byte> ByteFieldCollection { get; set; }
        public byte? ByteFieldNullable { get; set; }
        public List<byte>? ByteFieldNullableCollection { get; set; }
        public char CharField { get; set; }
        public List<char> CharFieldCollection { get; set; }
        public char? CharFieldNullable { get; set; }
        public List<char>? CharFieldNullableCollection { get; set; }
        public ComplexTypeDto ComplexTypeField { get; set; }
        public List<ComplexTypeDto> ComplexTypeFieldCollection { get; set; }
        public ComplexTypeDto? ComplexTypeFieldNullable { get; set; }
        public List<ComplexTypeDto>? ComplexTypeFieldNullableCollection { get; set; }
        public DateOnly DateOnlyField { get; set; }
        public List<DateOnly> DateOnlyFieldCollection { get; set; }
        public DateOnly? DateOnlyFieldNullable { get; set; }
        public List<DateOnly>? DateOnlyFieldNullableCollection { get; set; }
        public DateTime DateTimeField { get; set; }
        public List<DateTime> DateTimeFieldCollection { get; set; }
        public DateTime? DateTimeFieldNullable { get; set; }
        public List<DateTime>? DateTimeFieldNullableCollection { get; set; }
        public DateTimeOffset DateTimeOffsetField { get; set; }
        public List<DateTimeOffset> DateTimeOffsetFieldCollection { get; set; }
        public DateTimeOffset? DateTimeOffsetFieldNullable { get; set; }
        public List<DateTimeOffset>? DateTimeOffsetFieldNullableCollection { get; set; }
        public decimal DecimalField { get; set; }
        public List<decimal> DecimalFieldCollection { get; set; }
        public decimal? DecimalFieldNullable { get; set; }
        public List<decimal>? DecimalFieldNullableCollection { get; set; }
        public Dictionary<string, string> DictionaryField { get; set; }
        public List<Dictionary<string, string>> DictionaryFieldCollection { get; set; }
        public Dictionary<string, string>? DictionaryFieldNullable { get; set; }
        public List<Dictionary<string, string>>? DictionaryFieldNullableCollection { get; set; }
        public double DoubleField { get; set; }
        public List<double> DoubleFieldCollection { get; set; }
        public double? DoubleFieldNullable { get; set; }
        public List<double>? DoubleFieldNullableCollection { get; set; }
        public float FloatField { get; set; }
        public List<float> FloatFieldCollection { get; set; }
        public float? FloatFieldNullable { get; set; }
        public List<float>? FloatFieldNullableCollection { get; set; }
        public Guid GuidField { get; set; }
        public List<Guid> GuidFieldCollection { get; set; }
        public Guid? GuidFieldNullable { get; set; }
        public List<Guid>? GuidFieldNullableCollection { get; set; }
        public int IntField { get; set; }
        public List<int> IntFieldCollection { get; set; }
        public int? IntFieldNullable { get; set; }
        public List<int>? IntFieldNullableCollection { get; set; }
        public long LongField { get; set; }
        public List<long> LongFieldCollection { get; set; }
        public long? LongFieldNullable { get; set; }
        public List<long>? LongFieldNullableCollection { get; set; }
        public object ObjectField { get; set; }
        public List<object> ObjectFieldCollection { get; set; }
        public object? ObjectFieldNullable { get; set; }
        public List<object>? ObjectFieldNullableCollection { get; set; }
        public PagedResult<ComplexTypeDto> PagedResultField { get; set; }
        public PagedResult<ComplexTypeDto> PagedResultFieldNullable { get; set; }
        public short ShortField { get; set; }
        public List<short> ShortFieldCollection { get; set; }
        public short? ShortFieldNullable { get; set; }
        public List<short>? ShortFieldNullableCollection { get; set; }
        public string StringField { get; set; }
        public List<string> StringFieldCollection { get; set; }
        public string? StringFieldNullable { get; set; }
        public List<string>? StringFieldNullableCollection { get; set; }
        public TimeSpan TimeSpanField { get; set; }
        public List<TimeSpan> TimeSpanFieldCollection { get; set; }
        public TimeSpan? TimeSpanFieldNullable { get; set; }
        public List<TimeSpan>? TimeSpanFieldNullableCollection { get; set; }

        public static TypeTestDto Create(
            byte[] binaryField,
            List<byte[]> binaryFieldCollection,
            byte[]? binaryFieldNullable,
            List<byte[]>? binaryFieldNullableCollection,
            bool boolField,
            List<bool> boolFieldCollection,
            bool? boolFieldNullable,
            List<bool>? boolFieldNullableCollection,
            byte byteField,
            List<byte> byteFieldCollection,
            byte? byteFieldNullable,
            List<byte>? byteFieldNullableCollection,
            char charField,
            List<char> charFieldCollection,
            char? charFieldNullable,
            List<char>? charFieldNullableCollection,
            ComplexTypeDto complexTypeField,
            List<ComplexTypeDto> complexTypeFieldCollection,
            ComplexTypeDto? complexTypeFieldNullable,
            List<ComplexTypeDto>? complexTypeFieldNullableCollection,
            DateOnly dateOnlyField,
            List<DateOnly> dateOnlyFieldCollection,
            DateOnly? dateOnlyFieldNullable,
            List<DateOnly>? dateOnlyFieldNullableCollection,
            DateTime dateTimeField,
            List<DateTime> dateTimeFieldCollection,
            DateTime? dateTimeFieldNullable,
            List<DateTime>? dateTimeFieldNullableCollection,
            DateTimeOffset dateTimeOffsetField,
            List<DateTimeOffset> dateTimeOffsetFieldCollection,
            DateTimeOffset? dateTimeOffsetFieldNullable,
            List<DateTimeOffset>? dateTimeOffsetFieldNullableCollection,
            decimal decimalField,
            List<decimal> decimalFieldCollection,
            decimal? decimalFieldNullable,
            List<decimal>? decimalFieldNullableCollection,
            Dictionary<string, string> dictionaryField,
            List<Dictionary<string, string>> dictionaryFieldCollection,
            Dictionary<string, string>? dictionaryFieldNullable,
            List<Dictionary<string, string>>? dictionaryFieldNullableCollection,
            double doubleField,
            List<double> doubleFieldCollection,
            double? doubleFieldNullable,
            List<double>? doubleFieldNullableCollection,
            float floatField,
            List<float> floatFieldCollection,
            float? floatFieldNullable,
            List<float>? floatFieldNullableCollection,
            Guid guidField,
            List<Guid> guidFieldCollection,
            Guid? guidFieldNullable,
            List<Guid>? guidFieldNullableCollection,
            int intField,
            List<int> intFieldCollection,
            int? intFieldNullable,
            List<int>? intFieldNullableCollection,
            long longField,
            List<long> longFieldCollection,
            long? longFieldNullable,
            List<long>? longFieldNullableCollection,
            object objectField,
            List<object> objectFieldCollection,
            object? objectFieldNullable,
            List<object>? objectFieldNullableCollection,
            PagedResult<ComplexTypeDto> pagedResultField,
            PagedResult<ComplexTypeDto> pagedResultFieldNullable,
            short shortField,
            List<short> shortFieldCollection,
            short? shortFieldNullable,
            List<short>? shortFieldNullableCollection,
            string stringField,
            List<string> stringFieldCollection,
            string? stringFieldNullable,
            List<string>? stringFieldNullableCollection,
            TimeSpan timeSpanField,
            List<TimeSpan> timeSpanFieldCollection,
            TimeSpan? timeSpanFieldNullable,
            List<TimeSpan>? timeSpanFieldNullableCollection)
        {
            return new TypeTestDto
            {
                BinaryField = binaryField,
                BinaryFieldCollection = binaryFieldCollection,
                BinaryFieldNullable = binaryFieldNullable,
                BinaryFieldNullableCollection = binaryFieldNullableCollection,
                BoolField = boolField,
                BoolFieldCollection = boolFieldCollection,
                BoolFieldNullable = boolFieldNullable,
                BoolFieldNullableCollection = boolFieldNullableCollection,
                ByteField = byteField,
                ByteFieldCollection = byteFieldCollection,
                ByteFieldNullable = byteFieldNullable,
                ByteFieldNullableCollection = byteFieldNullableCollection,
                CharField = charField,
                CharFieldCollection = charFieldCollection,
                CharFieldNullable = charFieldNullable,
                CharFieldNullableCollection = charFieldNullableCollection,
                ComplexTypeField = complexTypeField,
                ComplexTypeFieldCollection = complexTypeFieldCollection,
                ComplexTypeFieldNullable = complexTypeFieldNullable,
                ComplexTypeFieldNullableCollection = complexTypeFieldNullableCollection,
                DateOnlyField = dateOnlyField,
                DateOnlyFieldCollection = dateOnlyFieldCollection,
                DateOnlyFieldNullable = dateOnlyFieldNullable,
                DateOnlyFieldNullableCollection = dateOnlyFieldNullableCollection,
                DateTimeField = dateTimeField,
                DateTimeFieldCollection = dateTimeFieldCollection,
                DateTimeFieldNullable = dateTimeFieldNullable,
                DateTimeFieldNullableCollection = dateTimeFieldNullableCollection,
                DateTimeOffsetField = dateTimeOffsetField,
                DateTimeOffsetFieldCollection = dateTimeOffsetFieldCollection,
                DateTimeOffsetFieldNullable = dateTimeOffsetFieldNullable,
                DateTimeOffsetFieldNullableCollection = dateTimeOffsetFieldNullableCollection,
                DecimalField = decimalField,
                DecimalFieldCollection = decimalFieldCollection,
                DecimalFieldNullable = decimalFieldNullable,
                DecimalFieldNullableCollection = decimalFieldNullableCollection,
                DictionaryField = dictionaryField,
                DictionaryFieldCollection = dictionaryFieldCollection,
                DictionaryFieldNullable = dictionaryFieldNullable,
                DictionaryFieldNullableCollection = dictionaryFieldNullableCollection,
                DoubleField = doubleField,
                DoubleFieldCollection = doubleFieldCollection,
                DoubleFieldNullable = doubleFieldNullable,
                DoubleFieldNullableCollection = doubleFieldNullableCollection,
                FloatField = floatField,
                FloatFieldCollection = floatFieldCollection,
                FloatFieldNullable = floatFieldNullable,
                FloatFieldNullableCollection = floatFieldNullableCollection,
                GuidField = guidField,
                GuidFieldCollection = guidFieldCollection,
                GuidFieldNullable = guidFieldNullable,
                GuidFieldNullableCollection = guidFieldNullableCollection,
                IntField = intField,
                IntFieldCollection = intFieldCollection,
                IntFieldNullable = intFieldNullable,
                IntFieldNullableCollection = intFieldNullableCollection,
                LongField = longField,
                LongFieldCollection = longFieldCollection,
                LongFieldNullable = longFieldNullable,
                LongFieldNullableCollection = longFieldNullableCollection,
                ObjectField = objectField,
                ObjectFieldCollection = objectFieldCollection,
                ObjectFieldNullable = objectFieldNullable,
                ObjectFieldNullableCollection = objectFieldNullableCollection,
                PagedResultField = pagedResultField,
                PagedResultFieldNullable = pagedResultFieldNullable,
                ShortField = shortField,
                ShortFieldCollection = shortFieldCollection,
                ShortFieldNullable = shortFieldNullable,
                ShortFieldNullableCollection = shortFieldNullableCollection,
                StringField = stringField,
                StringFieldCollection = stringFieldCollection,
                StringFieldNullable = stringFieldNullable,
                StringFieldNullableCollection = stringFieldNullableCollection,
                TimeSpanField = timeSpanField,
                TimeSpanFieldCollection = timeSpanFieldCollection,
                TimeSpanFieldNullable = timeSpanFieldNullable,
                TimeSpanFieldNullableCollection = timeSpanFieldNullableCollection
            };
        }
    }
}