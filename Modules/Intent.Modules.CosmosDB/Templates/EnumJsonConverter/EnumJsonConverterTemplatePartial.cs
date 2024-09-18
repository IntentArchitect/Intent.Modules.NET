using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.EnumJsonConverter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EnumJsonConverterTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.EnumJsonConverter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EnumJsonConverterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Newtonsoft.Json")
                .AddClass($"EnumJsonConverter", @class =>
                {
                    @class.WithBaseType("JsonConverter");
                    @class.AddMethod("void", "WriteJson", method =>
                    {
                        method.Override();
                        method.AddParameter("JsonWriter", "writer")
                            .AddParameter("object?", "value")
                            .AddParameter("JsonSerializer", "serializer");
                        method.AddStatement("writer.WriteValue(value?.ToString());");
                    });
                    @class.AddMethod("object?", "ReadJson", method =>
                    {
                        method.Override();
                        method.AddParameter("JsonReader", "reader")
                            .AddParameter("Type", "objectType")
                            .AddParameter("object?", "existingValue")
                            .AddParameter("JsonSerializer", "serializer");
                        method.AddStatement(
                            """
                            if (reader.Value is null || reader.TokenType == JsonToken.Null)
                            {
                                return null;
                            }
                            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                return Enum.Parse(objectType.GenericTypeArguments[0], reader.Value.ToString()!);
                            }

                            return Enum.Parse(objectType, reader.Value.ToString()!);
                            """);
                    });
                    @class.AddMethod("bool", "CanConvert", method =>
                    {
                        method.Override();
                        method.AddParameter("Type", "objectType");
                        method.AddStatement("return objectType.IsEnum;");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.Settings.GetCosmosDb().StoreEnumsAsStrings();
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