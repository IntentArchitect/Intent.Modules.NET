using GrpcServer.Application.TypeTests;
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
            BoolField = null!;
            ByteField = null!;
            CharField = null!;
            ComplexValueField = null!;
            DateOnlyField = null!;
            DateTimeField = null!;
            DateTimeOffsetField = null!;
            DecimalField = null!;
            DictionaryField = null!;
            DoubleField = null!;
            EnumField = null!;
            FloatField = null!;
            GuidField = null!;
            IntField = null!;
            LongField = null!;
            ObjectField = null!;
            PagedResultField = null!;
            ShortField = null!;
            StringField = null!;
            TimeSpanField = null!;
        }

        public BinaryTestDto BinaryField { get; set; }
        public BoolTestDto BoolField { get; set; }
        public ByteTestDto ByteField { get; set; }
        public CharTestDto CharField { get; set; }
        public ComplexValueTestDto ComplexValueField { get; set; }
        public DateOnlyTestDto DateOnlyField { get; set; }
        public DateTimeTestDto DateTimeField { get; set; }
        public DateTimeOffsetTestDto DateTimeOffsetField { get; set; }
        public DecimalTestDto DecimalField { get; set; }
        public DictionaryTestDto DictionaryField { get; set; }
        public DoubleTestDto DoubleField { get; set; }
        public EnumTestDto EnumField { get; set; }
        public FloatTestDto FloatField { get; set; }
        public GuidTestDto GuidField { get; set; }
        public IntTestDto IntField { get; set; }
        public LongTestDto LongField { get; set; }
        public ObjectTestDto ObjectField { get; set; }
        public PagedResultTestDto PagedResultField { get; set; }
        public ShortTestDto ShortField { get; set; }
        public StringTestDto StringField { get; set; }
        public TimeSpanTestDto TimeSpanField { get; set; }

        public static TypeTestDto Create(
            BinaryTestDto binaryField,
            BoolTestDto boolField,
            ByteTestDto byteField,
            CharTestDto charField,
            ComplexValueTestDto complexValueField,
            DateOnlyTestDto dateOnlyField,
            DateTimeTestDto dateTimeField,
            DateTimeOffsetTestDto dateTimeOffsetField,
            DecimalTestDto decimalField,
            DictionaryTestDto dictionaryField,
            DoubleTestDto doubleField,
            EnumTestDto enumField,
            FloatTestDto floatField,
            GuidTestDto guidField,
            IntTestDto intField,
            LongTestDto longField,
            ObjectTestDto objectField,
            PagedResultTestDto pagedResultField,
            ShortTestDto shortField,
            StringTestDto stringField,
            TimeSpanTestDto timeSpanField)
        {
            return new TypeTestDto
            {
                BinaryField = binaryField,
                BoolField = boolField,
                ByteField = byteField,
                CharField = charField,
                ComplexValueField = complexValueField,
                DateOnlyField = dateOnlyField,
                DateTimeField = dateTimeField,
                DateTimeOffsetField = dateTimeOffsetField,
                DecimalField = decimalField,
                DictionaryField = dictionaryField,
                DoubleField = doubleField,
                EnumField = enumField,
                FloatField = floatField,
                GuidField = guidField,
                IntField = intField,
                LongField = longField,
                ObjectField = objectField,
                PagedResultField = pagedResultField,
                ShortField = shortField,
                StringField = stringField,
                TimeSpanField = timeSpanField
            };
        }
    }
}