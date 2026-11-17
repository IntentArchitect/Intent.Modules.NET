using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.SearchWidgets
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SearchWidgetsQueryHandler : IRequestHandler<SearchWidgetsQuery, List<WidgetDto>>
    {
        [IntentManaged(Mode.Merge)]
        public SearchWidgetsQueryHandler()
        {
        }

        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<WidgetDto>> Handle(SearchWidgetsQuery request, CancellationToken cancellationToken)
        {
            return InMemoryWidgetStore.Widgets.Values
                .Where(w => string.IsNullOrEmpty(request.Name) || w.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
