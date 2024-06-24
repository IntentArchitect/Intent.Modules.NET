using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Solace.Templates.BaseMessageConverter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BaseMessageConverterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.BaseMessageConverter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BaseMessageConverterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Text.Json.Serialization")
                .AddClass($"BaseMessageConverter", @class =>
                {
                    @class.WithBaseType($"JsonConverter<{this.GetBaseMessageName()}>");
                    @class.AddField("Dictionary<string, Type>", "_typeLookup", p => p.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetMessageRegistryName(), "messageRegistry");
                        ctor.AddStatement("_typeLookup = new Dictionary<string, Type>();");
                        ctor.AddForEachStatement("message", "messageRegistry.MessageTypes", stmt =>
                        {
                            stmt.AddStatement("_typeLookup.Add(message.Value, message.Key);");
                        });
                    });

                    @class.AddMethod($"{this.GetBaseMessageName()}", "Read", method =>
                    {
                        method.Override();
                        method.AddParameter("Utf8JsonReader", "reader", p => p.WithRefParameterModifier());
                        method.AddParameter("Type", "typeToConvert");
                        method.AddParameter("JsonSerializerOptions", "options");
                        method.AddStatements(@"using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
			{
				var root = doc.RootElement;
				var messageTypeString = root.GetProperty(""MessageType"").GetString();
				if (messageTypeString is null)
					throw new Exception($""No `MessageType` property found in message {root.GetRawText()}"");

                if (!_typeLookup.TryGetValue(messageTypeString, out var messageType ))
                {
					throw new InvalidOperationException(""Unknown message type"");
				}
				var result = JsonSerializer.Deserialize(root.GetRawText(), messageType, options) as BaseMessage;
				if (result is null)
					throw new Exception($""Unable to deserialize {root.GetRawText()} as {messageType.Name}"");
				return result;
			}".ConvertToStatements());
                    });

                    @class.AddMethod("void", "Write", method =>
                    {
                        method.Override();
                        method.AddParameter("Utf8JsonWriter", "writer");
                        method.AddParameter($"{this.GetBaseMessageName()}", "value");
                        method.AddParameter("JsonSerializerOptions", "options");
                        method.AddStatement("JsonSerializer.Serialize(writer, value, value.GetType(), options);");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}