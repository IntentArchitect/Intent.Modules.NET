using System.Diagnostics.CodeAnalysis;
using GrpcServer.Api.Protos.Messages.TypeTests;
using GrpcServer.Application;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages
{
    public partial class TypeTestDto
    {
        public Application.TypeTestDto ToContract()
        {
            return new Application.TypeTestDto
            {
                BinaryField = BinaryField.ToContract(),
                BoolField = BoolField.ToContract(),
                ByteField = ByteField.ToContract(),
                CharField = CharField.ToContract(),
                ComplexValueField = ComplexValueField.ToContract(),
                DateOnlyField = DateOnlyField.ToContract(),
                DateTimeField = DateTimeField.ToContract(),
                DateTimeOffsetField = DateTimeOffsetField.ToContract(),
                DecimalField = DecimalField.ToContract(),
                DictionaryField = DictionaryField.ToContract(),
                DoubleField = DoubleField.ToContract(),
                EnumField = EnumField.ToContract(),
                FloatField = FloatField.ToContract(),
                GuidField = GuidField.ToContract(),
                IntField = IntField.ToContract(),
                LongField = LongField.ToContract(),
                ObjectField = ObjectField.ToContract(),
                PagedResultField = PagedResultField.ToContract(),
                ShortField = ShortField.ToContract(),
                StringField = StringField.ToContract(),
                TimeSpanField = TimeSpanField.ToContract()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static TypeTestDto? Create(Application.TypeTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new TypeTestDto
            {
                BinaryField = BinaryTestDto.Create(contract.BinaryField),
                BoolField = BoolTestDto.Create(contract.BoolField),
                ByteField = ByteTestDto.Create(contract.ByteField),
                CharField = CharTestDto.Create(contract.CharField),
                ComplexValueField = ComplexValueTestDto.Create(contract.ComplexValueField),
                DateOnlyField = DateOnlyTestDto.Create(contract.DateOnlyField),
                DateTimeField = DateTimeTestDto.Create(contract.DateTimeField),
                DateTimeOffsetField = DateTimeOffsetTestDto.Create(contract.DateTimeOffsetField),
                DecimalField = DecimalTestDto.Create(contract.DecimalField),
                DictionaryField = DictionaryTestDto.Create(contract.DictionaryField),
                DoubleField = DoubleTestDto.Create(contract.DoubleField),
                EnumField = EnumTestDto.Create(contract.EnumField),
                FloatField = FloatTestDto.Create(contract.FloatField),
                GuidField = GuidTestDto.Create(contract.GuidField),
                IntField = IntTestDto.Create(contract.IntField),
                LongField = LongTestDto.Create(contract.LongField),
                ObjectField = ObjectTestDto.Create(contract.ObjectField),
                PagedResultField = PagedResultTestDto.Create(contract.PagedResultField),
                ShortField = ShortTestDto.Create(contract.ShortField),
                StringField = StringTestDto.Create(contract.StringField),
                TimeSpanField = TimeSpanTestDto.Create(contract.TimeSpanField)
            };

            return message;
        }
    }
}