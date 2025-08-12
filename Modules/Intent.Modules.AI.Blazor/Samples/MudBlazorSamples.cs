using Intent.Engine;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using Intent.Modules.AI.Blazor.Tasks.Helpers;
namespace Intent.Modules.AI.Blazor.Samples
{
    internal class MudBlazorSampleTemplates
    {

        public static IEnumerable<ICodebaseFile>  LoadMudBlazorSamples()
        {
            var asm = typeof(MudBlazorSampleTemplates).Assembly;
            var prefix = $"Intent.Modules.AI.Blazor.Samples.MudBlazor.";

            foreach (var name in asm.GetManifestResourceNames().Where(n => n.StartsWith(prefix, StringComparison.Ordinal)))
            {
                using var stream = asm.GetManifestResourceStream(name)
                    ?? throw new InvalidOperationException($"Resource not found: {name}");

                // UTF-8, BOM-aware, and throw if bytes aren’t valid text (helps skip binaries)
                using var reader = new StreamReader(
                    stream,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
                    detectEncodingFromByteOrderMarks: true
                );

                string content;
                try
                {
                    content = reader.ReadToEnd();
                }
                catch (DecoderFallbackException)
                {
                    // Not text — skip it (or change to yield Base64, etc.)
                    continue;
                }

                var fileName = ToPathLikeName(name, prefix);
                yield return new CodebaseFile(fileName, content);
            }
        }

        private static string ToPathLikeName(string resourceName, string prefix)
        {
            // Turns "...MudBlazor.Components.Buttons.razor" into "Components/Buttons.razor"
            var tail = resourceName.Substring(prefix.Length);
            var lastDot = tail.LastIndexOf('.');
            return lastDot >= 0
                ? tail[..lastDot].Replace('.', '/') + tail[lastDot..]
                : tail;
        }        
    }
}
