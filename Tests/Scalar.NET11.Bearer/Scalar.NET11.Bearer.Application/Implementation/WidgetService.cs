using Intent.RoslynWeaver.Attributes;
using Scalar.NET11.Bearer.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Scalar.NET11.Bearer.Application.Implementation
{
    /// <summary>
    /// In-memory widget store exercising the OpenAPI shapes generated on .NET 11 (route parameters hidden from body, HTTP QUERY, binary upload, JSON merge patch).
    /// </summary>
    [IntentManaged(Mode.Merge)]
    public class WidgetService : IWidgetService
    {
        [IntentManaged(Mode.Merge)]
        public WidgetService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> CreateWidget(WidgetDto dto, CancellationToken cancellationToken = default)
        {
            var id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id;
            InMemoryWidgetStore.Widgets[id] = new WidgetDto { Id = id, Name = dto.Name };
            return id;
        }

        /// <summary>
        /// The route carries {id} and WidgetDto also has Id; the OpenAPI body schema must not show id.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task UpdateWidget(Guid id, WidgetDto dto, CancellationToken cancellationToken = default)
        {
            InMemoryWidgetStore.Widgets[id] = new WidgetDto { Id = id, Name = dto.Name };
        }

        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<WidgetDto>> SearchWidgets(
            WidgetSearchCriteriaDto criteria,
            CancellationToken cancellationToken = default)
        {
            return InMemoryWidgetStore.Widgets.Values
                .Where(w => string.IsNullOrEmpty(criteria.Name) || w.Name.Contains(criteria.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Binary upload: stores the uploaded bytes in memory and returns a new id.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> UploadWidgetImage(
            Stream content,
            string? filename,
            string? contentType,
            long? contentLength,
            CancellationToken cancellationToken = default)
        {
            using var buffer = new MemoryStream();
            await content.CopyToAsync(buffer, cancellationToken);
            var id = Guid.NewGuid();
            InMemoryWidgetStore.Images[id] = buffer.ToArray();
            return id;
        }

        /// <summary>
        /// JSON merge patch (RFC 7396) of a widget's name.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<WidgetDto> PatchWidget(Guid id, PatchWidgetDto dto, CancellationToken cancellationToken = default)
        {
            if (!InMemoryWidgetStore.Widgets.TryGetValue(id, out var existing))
            {
                return null!;
            }

            var target = new PatchWidgetDto { Name = existing.Name, PatchExecutor = dto.PatchExecutor };
            await dto.PatchExecutor.ApplyToAsync(target, cancellationToken);

            var patched = new WidgetDto { Id = id, Name = target.Name ?? existing.Name };
            InMemoryWidgetStore.Widgets[id] = patched;
            return patched;
        }
    }
}
