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

namespace Intent.Modules.Eventing.Solace.Templates.MessageSerializer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageSerializerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Solace.MessageSerializer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageSerializerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Text")
                .AddUsing("System.Text.Json")
                .AddClass($"MessageSerializer", @class =>
                {
                    @class.AddField("JsonSerializerOptions", "_serializationOptions", p => p.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{this.GetMessageRegistryName()}", "messageRegistry");
                        ctor.AddStatement("_serializationOptions = new JsonSerializerOptions();");
                        ctor.AddStatement($"_serializationOptions.Converters.Add(new {this.GetBaseMessageConverterName()}(messageRegistry));");
                    });

                    @class.AddMethod("byte[]", "SerializeMessage", method =>
                    {
                        method.AddParameter("object", "message");
                        method.AddStatement("return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(message, _serializationOptions));");
                    });

                    @class.AddMethod($"{this.GetBaseMessageName()}", "DeserializeMessage", method =>
                    {
                        method.AddParameter("byte[]", "binary");
                        method.AddStatements(@"string jsonString = Encoding.ASCII.GetString(binary);
			var result = JsonSerializer.Deserialize<BaseMessage>(jsonString, _serializationOptions);
			if (result == null)
				throw new Exception($""Unable to deserialize message as `BaseMessage`. ({jsonString})"");
			return result;".ConvertToStatements());
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