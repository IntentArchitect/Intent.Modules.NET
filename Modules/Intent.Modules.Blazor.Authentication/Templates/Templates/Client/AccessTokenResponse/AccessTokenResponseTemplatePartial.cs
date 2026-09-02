using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Authentication.Api;
using Intent.Modules.Blazor.Authentication.Settings;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static System.Net.Mime.MediaTypeNames;

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
            // This template is shared by the JWT and OIDC modes (see CanRunTemplate), whose token
            // endpoints use different property-name casing, and the wrong choice silently deserializes
            // every property as null:
            //
            //  - OIDC (RFC 6749 §5.1) is snake_case — "access_token", "expires_in". ReadFromJsonAsync
            //    uses JsonSerializerDefaults.Web (camelCase, case-insensitive), which does not match
            //    snake_case, so explicit [JsonPropertyName] attributes are required.
            //  - JWT's ASP.NET Core Identity backend returns camelCase — "accessToken", "expiresIn" —
            //    which JsonSerializerDefaults.Web already matches. Adding the snake_case attributes
            //    here would override the naming policy and break it.
            var isOidc = this.GetAuthenticationType().IsSingleSignOnOpenIDConnect();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Text.Json.Serialization")
                .AddClass($"AccessTokenResponse", @class =>
                {
                    @class.AddProperty("string", "AccessToken", p => ApplyJsonPropertyName(p, isOidc, "access_token"));
                    @class.AddProperty("string", "RefreshToken", p => ApplyJsonPropertyName(p, isOidc, "refresh_token"));
                    @class.AddProperty("string?", "TokenType", p => ApplyJsonPropertyName(p, isOidc, "token_type"));
                    @class.AddProperty("DateTime?", "ExpiresIn", p =>
                    {
                        ApplyJsonPropertyName(p, isOidc, "expires_in");
                        p.AddAttribute("[JsonConverter(typeof(NullableExpiresInConverter))]");
                    });

                    if (isOidc)
                    {
                        @class.AddProperty("string?", "Scope", p => ApplyJsonPropertyName(p, isOidc, "scope"));
                    }

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

        private static void ApplyJsonPropertyName(CSharpProperty property, bool isOidc, string snakeCaseName)
        {
            if (isOidc)
            {
                property.AddAttribute($"[JsonPropertyName(\"{snakeCaseName}\")]");
            }
        }

        public override bool CanRunTemplate()
        {
            var securityType = ExecutionContext.MetadataManager.GetAuthenticationType(ExecutionContext.GetApplicationConfig().Id);

            //3 Templates need this
            // JWT Auth Service 
            // OICD Auth Service
            // PersistentAuthenticationStateProviderTemplate
            return base.CanRunTemplate() && (!ExecutionContext.GetSettings().GetBlazor().RenderMode().IsInteractiveServer() || securityType.IsBearerTokenJWT() || securityType.IsSingleSignOnOpenIDConnect());
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