using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctions.LocalSettingsJson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class LocalSettingsJsonTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.AzureFunctions.LocalSettingsJson";

        private readonly Dictionary<string, (AppSettingRegistrationRequest Request, string StackTrace)> _registrationRequestsByKey = new();
        private readonly Dictionary<string, (ConnectionStringRegistrationRequest Request, string StackTrace)> _connectionStringRequestByName = new();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public LocalSettingsJsonTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(Handle);
            ExecutionContext.EventDispatcher.Subscribe<ConnectionStringRegistrationRequest>(Handle);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var config = new TemplateFileConfig(
                fileName: "local.settings",
                fileExtension: "json");

            config.CustomMetadata.Add("ItemType", "None");
            config.CustomMetadata.Add("CopyToOutputDirectory", "PreserveNewest");
            config.CustomMetadata.Add("CopyToPublishDirectory", "Never");

            return config;
        }

        public override string GetCorrelationId()
        {
            return $"{TemplateId}#{OutputTarget.Id}";
        }

        private void Handle(AppSettingRegistrationRequest @event)
        {
            if (!@event.IsApplicableTo(this))
            {
                return;
            }

            if (_registrationRequestsByKey.TryGetValue(@event.Key, out var value) && !Equals(value.Request.Value, @event.Value))
            {
                Logging.Log.Warning($"A request already existed for {@event.Key}{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"Existing item's stack trace:{Environment.NewLine}" +
                                    $"{value.StackTrace}{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"Incoming item's stack trace:{Environment.NewLine}" +
                                    $"{Environment.StackTrace}");
                return;
            }

            _registrationRequestsByKey.TryAdd(@event.Key, (@event, Environment.StackTrace));
        }

        private void Handle(ConnectionStringRegistrationRequest @event)
        {
            if (!@event.IsApplicableTo(this))
            {
                return;
            }

            var key = $"ConnectionStrings:{@event.Name}";
            if (_connectionStringRequestByName.TryGetValue(key, out var value) && !Equals(value.Request.ConnectionString, @event.ConnectionString))
            {
                Logging.Log.Warning($"A request already existed for {key}{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"Existing item's stack trace:{Environment.NewLine}" +
                                    $"{value.StackTrace}{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"Incoming item's stack trace:{Environment.NewLine}" +
                                    $"{Environment.StackTrace}");
                return;
            }

            _connectionStringRequestByName.TryAdd(key, (@event, Environment.StackTrace));
        }

        public override string RunTemplate()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = TransformText();
            }

            var json = JsonConvert.DeserializeObject<JObject>(content);
            var valuesObj = (JObject)(json["Values"] ??= new JObject());

            var preExisting = valuesObj.Properties().Select(x => x.Name).ToArray();

            foreach (var (key, (request, _)) in _registrationRequestsByKey)
            {
                if (preExisting.Any(property => property == request.Key || property.StartsWith($"{request.Key}:")))
                {
                    continue;
                }

                if (request.Value is not null and not string &&
                    !request.Value.GetType().IsPrimitive)
                {
                    foreach (var item in Deconstruct(request.Key, request.Value))
                    {
                        valuesObj[item.Key] = item.Value != null
                            ? JToken.FromObject(item.Value)
                            : null;
                    }

                    continue;
                }

                valuesObj[key] = request.Value != null
                    ? JToken.FromObject(request.Value)
                    : null;
            }

            foreach (var (key, (request, _)) in _connectionStringRequestByName)
            {
                if (request.ConnectionString == null)
                {
                    continue;
                }

                valuesObj[key] ??= JToken.FromObject(request.ConnectionString);
            }

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }

        private static IEnumerable<(string Key, object Value)> Deconstruct(string key, object value)
        {
            var stack = new Stack<string>();
            var items = new List<(string Key, object Value)>();

            Populate(
                name: key,
                element: JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(value)).RootElement);

            return items;

            void Populate(string name, JsonElement element)
            {
                stack.Push(name);

                switch (element.ValueKind)
                {
                    case JsonValueKind.Object:
                        foreach (var childElement in element.EnumerateObject())
                        {
                            Populate(childElement.Name, childElement.Value);
                        }
                        break;
                    case JsonValueKind.Array:
                        var arrayElements = element.EnumerateArray().ToArray();
                        for (var i = 0; i < arrayElements.Length; i++)
                        {
                            Populate(i.ToString(CultureInfo.InvariantCulture), arrayElements[i]);
                        }
                        break;
                    case JsonValueKind.Null:
                        items.Add((string.Join(":", stack.Reverse()), null));
                        break;
                    case JsonValueKind.String:
                        items.Add((string.Join(":", stack.Reverse()), element.GetString()));
                        break;
                    case JsonValueKind.Number when element.GetRawText().Contains("."):
                        items.Add((string.Join(":", stack.Reverse()), element.GetDecimal()));
                        break;
                    case JsonValueKind.Number:
                        items.Add((string.Join(":", stack.Reverse()), element.GetInt64()));
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        items.Add((string.Join(":", stack.Reverse()), element.GetBoolean()));
                        break;
                    case JsonValueKind.Undefined:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                stack.Pop();
            }
        }
    }
}