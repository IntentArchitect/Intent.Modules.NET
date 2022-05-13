using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Configuration;
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
            if (!@event.IsApplicableTo(this))
            {
                return;
            }

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
            if (!TryGetExistingFileContent(out var content))
            {
                content = TransformText();
            }

            var json = JsonConvert.DeserializeObject<JObject>(content);

            foreach (var request in _registrationRequestsByKey)
            {
                json.SetFieldValue(request.Key, request.Value, allowReplacement: false);
            }

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }
    }
}