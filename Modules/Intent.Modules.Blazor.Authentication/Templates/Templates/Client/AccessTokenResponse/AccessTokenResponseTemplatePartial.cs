using Intent.Engine;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Client.AccessTokenResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AccessTokenResponseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Client.AccessTokenResponseTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AccessTokenResponseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass($"AccessTokenResponse", @class =>
                {
                    @class.AddProperty("string", "AccessToken");
                    @class.AddProperty("string", "RefreshToken");
                    @class.AddProperty("string?", "TokenType");
                    @class.AddProperty("DateTime?", "ExpiresIn", p =>
                    {
                        p.AddAttribute("[JsonConverter(typeof(NullableExpiresInConverter))]");
                    });

                    @class.AddNestedClass("NullableExpiresInConverter", nested =>
                    {
                        CSharpFile.AddUsing("System.Text.Json");
                        CSharpFile.AddUsing("System.Text.Json.Serialization");

                        nested.WithBaseType("JsonConverter<DateTime?>");
                        nested.AddMethod("DateTime?", "Read", method =>
                        {
                            method.Override();
                            method.AddParameter("Utf8JsonReader", "reader", p => p.WithRefParameterModifier());
                            method.AddParameter("Type", "typeToConvert");
                            method.AddParameter("JsonSerializerOptions", "options");
                            method.AddStatements(@"// JSON null → null
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                // Number → seconds
                if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetInt64(out var seconds))
                        return DateTime.UtcNow.AddSeconds(seconds);

                    throw new JsonException(""expiresIn number is not Int64."");
                }

                // String (ISO date, seconds, empty, or null-like)
                if (reader.TokenType == JsonTokenType.String)
                {
                    var raw = reader.GetString();

                    // """" or ""null"" → null
                    if (string.IsNullOrWhiteSpace(raw) || raw.Equals(""null"", StringComparison.OrdinalIgnoreCase))
                        return null;

                    // ISO timestamp
                    if (DateTimeOffset.TryParse(raw, out var dto))
                        return dto.UtcDateTime;

                    // seconds as string
                    if (long.TryParse(raw, out var seconds))
                        return DateTime.UtcNow.AddSeconds(seconds);

                    throw new JsonException($""Cannot parse expiresIn value: {raw}"");
                }

                throw new JsonException(
                    $""Unexpected token type for expiresIn: {reader.TokenType}"");".ConvertToStatements());
                        });

                        nested.AddMethod("void", "Write", method =>
                        {
                            method.Override();
                            method.AddParameter("Utf8JsonWriter", "writer");
                            method.AddParameter("DateTime?", "value");
                            method.AddParameter("JsonSerializerOptions", "options");
                            method.AddStatements(@"if (value == null)
                {
                    writer.WriteNullValue();
                    return;
                }

                var utc = value.Value.Kind == DateTimeKind.Utc
                    ? value.Value
                    : value.Value.ToUniversalTime();

                long seconds = (long)Math.Max(
                    0,
                    (utc - DateTime.UtcNow).TotalSeconds);

                writer.WriteNumberValue(seconds);".ConvertToStatements());
                        });
                    });

                });
        }

        public override bool CanRunTemplate()
        {
            //2 Template need this
            //JWT Service 
            //PersistentAuthenticationStateProviderTemplate
            return base.CanRunTemplate() && (!ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() || ExecutionContext.GetSettings().GetBlazor().Authentication().IsJwt());
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