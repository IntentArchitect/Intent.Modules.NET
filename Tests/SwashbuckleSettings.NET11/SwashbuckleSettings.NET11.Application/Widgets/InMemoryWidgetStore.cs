using System.Collections.Concurrent;

namespace SwashbuckleSettings.NET11.Application.Widgets
{
    /// <summary>
    /// Process-wide in-memory storage for the widget test endpoints. This app has no database by design.
    /// </summary>
    internal static class InMemoryWidgetStore
    {
        public static ConcurrentDictionary<Guid, WidgetDto> Widgets { get; } = new();
        public static ConcurrentDictionary<Guid, byte[]> Images { get; } = new();
    }
}
