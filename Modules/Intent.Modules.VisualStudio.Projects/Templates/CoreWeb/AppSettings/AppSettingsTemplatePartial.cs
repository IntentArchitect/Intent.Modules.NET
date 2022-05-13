using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AppSettingsTemplate : IntentTemplateBase<AppSettingsModel, AppSettingsDecorator>
    {

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.VisualStudio.Projects.CoreWeb.AppSettings";
        private readonly IList<AppSettingRegistrationRequest> _appSettings = new List<AppSettingRegistrationRequest>();
        private readonly IList<ConnectionStringRegistrationRequest> _connectionStrings = new List<ConnectionStringRegistrationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public AppSettingsTemplate(IOutputTarget outputTarget, AppSettingsModel model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<AppSettingRegistrationRequest>(HandleAppSetting);
            ExecutionContext.EventDispatcher.Subscribe<ConnectionStringRegistrationRequest>(HandleConnectionString);
        }

        public override string GetCorrelationId()
        {
            return $"{TemplateId}-{Model.RuntimeEnvironment?.Name ?? "Default"}#{OutputTarget.Id}";
        }

        public override string RunTemplate()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = TransformText();
            }

            var json = JsonConvert.DeserializeObject<JObject>(content);

            foreach (var appSetting in _appSettings)
            {
                json.SetFieldValue(appSetting.Key, appSetting.Value, false);
            }

            foreach (var connectionString in _connectionStrings)
            {
                var configConnectionStrings = json["ConnectionStrings"];
                if (configConnectionStrings == null)
                {
                    configConnectionStrings = new JObject();
                    json["ConnectionStrings"] = configConnectionStrings;
                }

                configConnectionStrings[connectionString.Name] ??= connectionString.ConnectionString;
            }

            var appSettings = new AppSettingsEditor(json);

            foreach (var decorator in GetDecorators())
            {
                decorator.UpdateSettings(appSettings);
            }

            return JsonConvert.SerializeObject(json, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var (runtimeEnvironment, dependsOn) = !string.IsNullOrWhiteSpace(Model.RuntimeEnvironment?.Name)
                ? ($".{Model.RuntimeEnvironment.Name.ToPascalCase()}", "appsettings.json")
                : (null, null);

            return new TemplateFileConfig(
                fileName: $"appsettings{runtimeEnvironment}",
                fileExtension: "json")
                    .WithDependsOn(dependsOn);
        }

        private void HandleAppSetting(AppSettingRegistrationRequest @event)
        {
            if (Model.RuntimeEnvironment?.Name != @event.RuntimeEnvironment)
            {
                return;
            }

            if (!@event.IsApplicableTo(this))
            {
                return;
            }

            @event.MarkHandled();

            if (_appSettings.Any(x => x.Key == @event.Key && x.Value != @event.Value))
            {
                return;
            }

            _appSettings.Add(@event);
        }

        private void HandleConnectionString(ConnectionStringRegistrationRequest @event)
        {
            if (Model.RuntimeEnvironment?.Name != @event.RuntimeEnvironment)
            {
                return;
            }

            if (!@event.IsApplicableTo(this))
            {
                return;
            }

            @event.MarkHandled();

            if (_connectionStrings.Any(x => x.Name == @event.Name && x.ConnectionString != @event.ConnectionString))
            {
                throw new Exception($"Misconfiguration in [{GetType().Name}]: ConnectionString with name [{@event.Name}] already defined with different value to [{@event.ConnectionString}].");
            }

            _connectionStrings.Add(@event);
        }
    }
}
