using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctions.HostJson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class HostJsonTemplate : IntentTemplateBase<object>
    {
        private readonly Dictionary<string, (HostSettingRegistrationRequest Request, string StackTrace)> _registrationRequestsByKey = new();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.AzureFunctions.HostJson";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public HostJsonTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<HostSettingRegistrationRequest>(Handle);
        }

        private void Handle(HostSettingRegistrationRequest @event)
        {
            if (!string.IsNullOrWhiteSpace(@event.ForProjectWithRole) &&
                !OutputTarget.GetProject().HasRole(@event.ForProjectWithRole))
            {
                return;
            }

            @event.MarkHandled();

            if (_registrationRequestsByKey.TryGetValue(@event.Key, out var value))
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

            _registrationRequestsByKey.Add(@event.Key, (@event, Environment.StackTrace));
        }

        public override string GetCorrelationId()
        {
            return $"{TemplateId}#{OutputTarget.Id}";
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var config = new TemplateFileConfig(
                fileName: "host",
                fileExtension: "json");

            config.CustomMetadata.Add("ItemType", "None");
            config.CustomMetadata.Add("CopyToOutputDirectory", "PreserveNewest");

            return config;
        }

        public override string RunTemplate()
        {
            var jsonText = File.Exists(FileMetadata.GetFullLocationPath())
                ? File.ReadAllText(FileMetadata.GetFullLocationPath())
                : File.Exists(GetExistingFilePath())
                    ? File.ReadAllText(GetExistingFilePath())
                    : TransformText();

            var json = JsonConvert.DeserializeObject<JObject>(jsonText);

            foreach (var item in _registrationRequestsByKey)
            {
                Apply(item.Value.Request, json);
            }

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }

        private void Apply(HostSettingRegistrationRequest request, JToken jToken)
        {
            if (!request.IsApplicableTo(this))
            {
                return;
            }

            var @object = jToken;
            var field = default(string);

            var split = request.Key.Split(':');
            for (var index = 0; index < split.Length; index++)
            {
                field = split[index];

                // Don't do the last entry
                if (index == split.Length - 1)
                {
                    break;
                }

                @object[field] ??= new JObject();
                @object = @object[field];
            }

            // Don't overwrite if already existed
            if (@object[field!] != null)
            {
                return;
            }

            @object[field!] = request.Value switch
            {
                bool primitive => primitive,
                byte primitive => primitive,
                sbyte primitive => primitive,
                char primitive => primitive,
                decimal primitive => primitive,
                double primitive => primitive,
                float primitive => primitive,
                int primitive => primitive,
                uint primitive => primitive,
                long primitive => primitive,
                ulong primitive => primitive,
                short primitive => primitive,
                ushort primitive => primitive,
                string primitive => primitive,
                _ => JObject.FromObject(request.Value)
            };
        }
    }
}