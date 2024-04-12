using System.Collections.Generic;
using System.Text.Json.Serialization;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.ServiceInvocation.ProblemDetailsWithErrors", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Common.Exceptions
{
    public class ProblemDetailsWithErrors
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }
        [JsonPropertyName("errors")]
        public Dictionary<string, string[]> Errors { get; set; }
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}