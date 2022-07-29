using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

        private readonly Dictionary<string, (AppSettingRegistrationRequest Request, string StackTrace)>
            _registrationRequestsByKey = new();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public LocalSettingsJsonTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(Handle);
        }

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

        public override string RunTemplate()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = TransformText();
            }

            var json = JsonConvert.DeserializeObject<JObject>(content);
            var valuesObj = json["Values"] ??= new JObject();

            foreach (var request in _registrationRequestsByKey)
            {
                valuesObj[request.Key] ??= new JValue(request.Value.Request.Value);
            }

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }
    }
}