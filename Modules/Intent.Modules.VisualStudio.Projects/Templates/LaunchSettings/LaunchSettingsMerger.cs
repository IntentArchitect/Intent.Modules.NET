#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Intent.Modules.Common.CSharp.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates.LaunchSettings
{
    /// <summary>
    /// Everything <see cref="LaunchSettingsJsonTemplate"/> collected during a Software Factory run that
    /// needs to end up in launchSettings.json.
    /// </summary>
    internal record LaunchSettingsRegistrations(
        IEnumerable<KeyValuePair<string, Profile>> Profiles,
        IReadOnlyList<EnvironmentVariableRegistrationRequest> EnvironmentVariables,
        bool HttpPortRequired,
        int HttpPort,
        string? DefaultLaunchUrlPath);

    /// <summary>
    /// Merges <see cref="LaunchSettingsRegistrations"/> into launchSettings.json. An existing file is only
    /// ever added to: it is edited as a JSON tree rather than round-tripped through the typed
    /// <see cref="LaunchSettings"/> classes, so unknown members (e.g. <c>$schema</c>), property order,
    /// values and formatting all survive, and it is returned byte-for-byte when there is nothing to add.
    /// </summary>
    internal static class LaunchSettingsMerger
    {
        /// <summary>
        /// Every path a <c>DefaultLaunchUrlPathRequest</c> is raised with (Swashbuckle, Scalar). A launchUrl
        /// pointing at one of these was put there by Intent and is kept in sync when the default changes;
        /// any other launchUrl is the developer's own and is never touched.
        /// </summary>
        private static readonly string[] IntentManagedLaunchUrlPaths = ["swagger", "scalar/v1"];

        private static readonly JsonSerializer Serializer = JsonSerializer.Create(Converter.Settings);

        public static string Merge(
            string? existingContent,
            Func<LaunchSettings> createDefaultLaunchSettings,
            LaunchSettingsRegistrations registrations)
        {
            if (string.IsNullOrWhiteSpace(existingContent))
            {
                return CreateNew(createDefaultLaunchSettings(), registrations);
            }

            var original = Parse(existingContent);
            var merged = (JObject)original.DeepClone();

            ApplyRegistrations(merged, registrations);

            return JToken.DeepEquals(original, merged)
                ? existingContent
                : Serialize(merged, existingContent);
        }

        public static JObject Parse(string content)
        {
            using var reader = new JsonTextReader(new StringReader(content))
            {
                // Keep date- and number-like values exactly as they were written
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Decimal
            };

            return JObject.Load(reader);
        }

        private static string CreateNew(LaunchSettings launchSettings, LaunchSettingsRegistrations registrations)
        {
            var json = JObject.FromObject(launchSettings, Serializer);

            ApplyRegistrations(json, registrations);

            // Back through the typed model so that a new file keeps the property order and formatting it
            // has always been generated with.
            return json.ToObject<LaunchSettings>(Serializer)!.ToJson();
        }

        private static void ApplyRegistrations(JObject launchSettings, LaunchSettingsRegistrations registrations)
        {
            var profiles = AddMissingProfiles(launchSettings, registrations.Profiles);
            if (profiles == null)
            {
                return;
            }

            if (registrations.HttpPortRequired)
            {
                AddMissingHttpApplicationUrls(profiles, registrations.HttpPort);
            }

            if (!string.IsNullOrWhiteSpace(registrations.DefaultLaunchUrlPath))
            {
                SyncIntentManagedLaunchUrls(profiles, registrations.DefaultLaunchUrlPath);
            }

            AddMissingEnvironmentVariables(profiles, registrations.EnvironmentVariables);
        }

        private static JObject? AddMissingProfiles(JObject launchSettings, IEnumerable<KeyValuePair<string, Profile>> registeredProfiles)
        {
            var profilesToken = launchSettings["profiles"];
            if (profilesToken is not null and not JObject && profilesToken.Type != JTokenType.Null)
            {
                // Not something we understand, so not ours to replace
                return null;
            }

            var profiles = profilesToken as JObject;
            foreach (var (name, profile) in registeredProfiles)
            {
                if (profiles?.ContainsKey(name) == true)
                {
                    // A same-named profile is the developer's, even when it lacks fields we would generate
                    continue;
                }

                if (profiles == null)
                {
                    profiles = new JObject();
                    launchSettings["profiles"] = profiles;
                }

                profiles.Add(name, JObject.FromObject(profile, Serializer));
            }

            return profiles;
        }

        private static void AddMissingHttpApplicationUrls(JObject profiles, int httpPort)
        {
            foreach (var profile in profiles.PropertyValues().OfType<JObject>())
            {
                if (profile["applicationUrl"] is not JValue { Type: JTokenType.String } applicationUrl)
                {
                    continue;
                }

                var urls = ((string)applicationUrl.Value!).Split(';');
                if (!urls.Any(url => HasScheme(url, Uri.UriSchemeHttps)) ||
                    urls.Any(url => HasScheme(url, Uri.UriSchemeHttp)))
                {
                    continue;
                }

                applicationUrl.Value = string.Join(";", urls.Append($"http://localhost:{httpPort}"));
            }
        }

        private static bool HasScheme(string url, string scheme)
        {
            return url.TrimStart().StartsWith($"{scheme}://", StringComparison.OrdinalIgnoreCase);
        }

        private static void SyncIntentManagedLaunchUrls(JObject profiles, string defaultLaunchUrlPath)
        {
            // We don't have a way to tell a Web API's profile from a Web App's, so only "*.Api" profiles
            // are kept in sync (e.g. after swapping Swashbuckle for Scalar).
            foreach (var property in profiles.Properties())
            {
                if (!property.Name.EndsWith(".Api", StringComparison.Ordinal) ||
                    property.Value is not JObject profile ||
                    profile["launchUrl"] is not JValue { Type: JTokenType.String } launchUrl)
                {
                    continue;
                }

                var updatedLaunchUrl = ReplaceIntentManagedPath((string)launchUrl.Value!, defaultLaunchUrlPath);
                if (updatedLaunchUrl != null)
                {
                    launchUrl.Value = updatedLaunchUrl;
                }
            }
        }

        private static string? ReplaceIntentManagedPath(string launchUrl, string defaultLaunchUrlPath)
        {
            if (Uri.TryCreate(launchUrl, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                if (!string.IsNullOrEmpty(uri.Query) ||
                    !string.IsNullOrEmpty(uri.Fragment) ||
                    !IsOutdatedIntentManagedPath(uri.AbsolutePath, defaultLaunchUrlPath))
                {
                    return null;
                }

                return new UriBuilder(uri) { Path = $"/{defaultLaunchUrlPath}" }.Uri.ToString();
            }

            // Relative, e.g. "swagger"
            if (!IsOutdatedIntentManagedPath(launchUrl, defaultLaunchUrlPath))
            {
                return null;
            }

            return launchUrl.StartsWith('/') ? $"/{defaultLaunchUrlPath}" : defaultLaunchUrlPath;
        }

        private static bool IsOutdatedIntentManagedPath(string path, string defaultLaunchUrlPath)
        {
            var trimmedPath = path.Trim('/');

            return !trimmedPath.Equals(defaultLaunchUrlPath.Trim('/'), StringComparison.OrdinalIgnoreCase) &&
                IntentManagedLaunchUrlPaths.Contains(trimmedPath, StringComparer.OrdinalIgnoreCase);
        }

        private static void AddMissingEnvironmentVariables(JObject profiles, IEnumerable<EnvironmentVariableRegistrationRequest> environmentVariables)
        {
            foreach (var environmentVariable in environmentVariables)
            {
                var targetProfiles = environmentVariable.TargetProfiles ?? profiles.Properties().Select(p => p.Name).ToArray();
                foreach (var profileName in targetProfiles)
                {
                    if (profiles[profileName] is not JObject profile)
                    {
                        continue;
                    }

                    var variables = profile["environmentVariables"];
                    if (variables == null || variables.Type == JTokenType.Null)
                    {
                        variables = new JObject();
                        profile["environmentVariables"] = variables;
                    }

                    if (variables is JObject variablesObject && !variablesObject.ContainsKey(environmentVariable.Key))
                    {
                        variablesObject.Add(environmentVariable.Key, new JValue(environmentVariable.Value as string));
                    }
                }
            }
        }

        private static string Serialize(JObject launchSettings, string originalContent)
        {
            var leadingWhitespace = originalContent[..(originalContent.Length - originalContent.TrimStart().Length)];
            var trailingWhitespace = originalContent[originalContent.TrimEnd().Length..];
            var (indentChar, indentation) = DetectIndentation(originalContent);

            using var stringWriter = new StringWriter(CultureInfo.InvariantCulture) { NewLine = DetectNewLine(originalContent) };
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.IndentChar = indentChar;
                jsonWriter.Indentation = indentation;

                launchSettings.WriteTo(jsonWriter);
            }

            return leadingWhitespace + stringWriter + trailingWhitespace;
        }

        private static string DetectNewLine(string content)
        {
            var index = content.IndexOf('\n');
            if (index < 0)
            {
                return Environment.NewLine;
            }

            return index > 0 && content[index - 1] == '\r' ? "\r\n" : "\n";
        }

        private static (char IndentChar, int Indentation) DetectIndentation(string content)
        {
            // The first indented line is one level deep, so its leading whitespace is one indent
            foreach (var line in content.Split('\n').Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var width = line.Length - line.TrimStart(' ', '\t').Length;
                if (width > 0)
                {
                    return (line[0], width);
                }
            }

            return (' ', 2);
        }
    }
}
