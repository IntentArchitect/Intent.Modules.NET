using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Scalar.NET11.Bearer.Application.Interfaces
{
    /// <summary>
    /// In-memory widget store exercising the OpenAPI shapes generated on .NET 11 (route parameters hidden from body, HTTP QUERY, binary upload, JSON merge patch).
    /// </summary>
    public interface IWidgetService
    {
        Task<Guid> CreateWidget(WidgetDto dto, CancellationToken cancellationToken = default);
        /// <summary>
        /// The route carries {id} and WidgetDto also has Id; the OpenAPI body schema must not show id.
        /// </summary>
        Task UpdateWidget(Guid id, WidgetDto dto, CancellationToken cancellationToken = default);
        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body.
        /// </summary>
        Task<List<WidgetDto>> SearchWidgets(WidgetSearchCriteriaDto criteria, CancellationToken cancellationToken = default);
        /// <summary>
        /// Binary upload: stores the uploaded bytes in memory and returns a new id.
        /// </summary>
        Task<Guid> UploadWidgetImage(Stream content, string? filename, string? contentType, long? contentLength, CancellationToken cancellationToken = default);
        /// <summary>
        /// JSON merge patch (RFC 7396) of a widget's name.
        /// </summary>
        Task<WidgetDto> PatchWidget(Guid id, PatchWidgetDto dto, CancellationToken cancellationToken = default);
    }
}