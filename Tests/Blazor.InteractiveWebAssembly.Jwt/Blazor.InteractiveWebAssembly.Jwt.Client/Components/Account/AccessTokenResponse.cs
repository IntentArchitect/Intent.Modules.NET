using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Authentication.Templates.Client.AccessTokenResponseTemplate", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt.Client.Components.Account
{
    public class AccessTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string? TokenType { get; set; }
        [JsonConverter(typeof(NullableExpiresInConverter))]
        public DateTime? ExpiresIn { get; set; }
        public class NullableExpiresInConverter : JsonConverter<DateTime?>
        {
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // JSON null → null
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                // Number → seconds
                if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetInt64(out var seconds))
                        return DateTime.UtcNow.AddSeconds(seconds);

                    throw new JsonException("expiresIn number is not Int64.");
                }

                // String (ISO date, seconds, empty, or null-like)
                if (reader.TokenType == JsonTokenType.String)
                {
                    var raw = reader.GetString();

                    // "" or "null" → null
                    if (string.IsNullOrWhiteSpace(raw) || raw.Equals("null", StringComparison.OrdinalIgnoreCase))
                        return null;

                    // ISO timestamp
                    if (DateTimeOffset.TryParse(raw, out var dto))
                        return dto.UtcDateTime;

                    // seconds as string
                    if (long.TryParse(raw, out var seconds))
                        return DateTime.UtcNow.AddSeconds(seconds);

                    throw new JsonException($"Cannot parse expiresIn value: {raw}");
                }

                throw new JsonException(
                    $"Unexpected token type for expiresIn: {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
            {
                if (value == null)
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

                writer.WriteNumberValue(seconds);
            }
        }
    }
}