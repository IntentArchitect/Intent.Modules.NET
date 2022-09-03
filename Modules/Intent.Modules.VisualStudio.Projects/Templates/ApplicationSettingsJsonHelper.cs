using System.Collections;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates;

internal static class ApplicationSettingsJsonHelper
{
    public static void SetFieldValue(this JToken jToken, string key, object value, bool allowReplacement)
    {
        var fieldName = default(string);

        var split = key.Replace("__", ":").Split(':');
        for (var index = 0; index < split.Length; index++)
        {
            fieldName = split[index];

            // Don't do the last entry
            if (index == split.Length - 1)
            {
                break;
            }

            jToken[fieldName] ??= new JObject();
            jToken = jToken[fieldName];
        }

        // Don't overwrite if already existed
        if (jToken[fieldName!] != null &&
            !allowReplacement)
        {
            return;
        }

        jToken[fieldName!] = value == null
            ? null
            : JToken.FromObject(value);
    }

    public static bool TryGetFieldValue(this JToken jToken, string key, out object value)
    {
        var split = key.Replace("__", ":").Split(':');

        foreach (var fieldName in split)
        {
            if (jToken[fieldName] == null)
            {
                value = default;
                return false;
            }

            jToken = jToken[fieldName];
        }

        value = jToken;
        return true;
    }
}