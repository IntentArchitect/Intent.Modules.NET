using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory.Templates.ApplicationSettingsJson
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApplicationSettingsJsonTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.DesignTimeDbContextFactory.ApplicationSettingsJson";

        private readonly IList<ConnectionStringRegistrationRequest> _connectionStrings = new List<ConnectionStringRegistrationRequest>();

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApplicationSettingsJsonTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ConnectionStringRegistrationRequest>(HandleConnectionString);
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"appsettings",
                fileExtension: "json"
            );
        }

        public override string TransformText()
        {
            if (!TryGetExistingFileContent(out var content))
            {
                content = "{}";
            }

            var json = JsonConvert.DeserializeObject<JObject>(content);

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

            return JsonConvert.SerializeObject(json, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        private void HandleConnectionString(ConnectionStringRegistrationRequest @event)
        {
            if (@event.RuntimeEnvironment is not (null or "" or "Development"))
            {
                return;
            }

            if (_connectionStrings.Any(x => x.Name == @event.Name && x.ConnectionString != @event.ConnectionString))
            {
                throw new Exception($"Misconfiguration in [{GetType().Name}]: ConnectionString with name [{@event.Name}] already defined with different value to [{@event.ConnectionString}].");
            }

            _connectionStrings.Add(@event);
        }
    }
}