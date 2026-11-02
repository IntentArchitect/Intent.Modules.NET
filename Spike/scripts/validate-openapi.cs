#:package Microsoft.OpenApi@3.11.0
// Validates an OpenAPI document with Microsoft.OpenApi 3.x, which reads OpenAPI 3.2 (Swashbuckle 10's 2.x cannot).
// Usage: dotnet run validate-openapi.cs -- <path-or-url-to-openapi.json>
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

var source = args[0];
Stream stream = source.StartsWith("http", StringComparison.OrdinalIgnoreCase)
    ? await new HttpClient().GetStreamAsync(source)
    : File.OpenRead(source);

var result = await OpenApiDocument.LoadAsync(stream, "json", new OpenApiReaderSettings());
Console.WriteLine($"Spec version: {result.Diagnostic?.SpecificationVersion}");
foreach (var error in result.Diagnostic?.Errors ?? [])
{
    Console.WriteLine($"ERROR {error.Pointer}: {error.Message}");
}
foreach (var warning in result.Diagnostic?.Warnings ?? [])
{
    Console.WriteLine($"WARN  {warning.Pointer}: {warning.Message}");
}

foreach (var (path, item) in result.Document?.Paths ?? [])
{
    var methods = item.Operations?.Keys.Select(k => k.Method) ?? [];
    Console.WriteLine($"  {path}: {string.Join(", ", methods)}");
}

return (result.Diagnostic?.Errors.Count ?? 0) == 0 ? 0 : 1;
