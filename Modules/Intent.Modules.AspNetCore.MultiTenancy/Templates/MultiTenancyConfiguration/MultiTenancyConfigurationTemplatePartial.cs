using System.Collections.Generic;
using System.Text.Json;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenancyConfiguration
{
    partial class MultiTenancyConfigurationTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public MultiTenancyConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency("Finbuckle.MultiTenant.AspNetCore", "6.5.1");
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            if (ExecutionContext.Settings.GetMultitenancySettings().Store().IsConfiguration())
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Finbuckle:MultiTenant:Stores:ConfigurationStore", JsonConvert.DeserializeObject(@"
{
    ""Tenants"": [
      {
        ""Id"": ""sample-tenant-1"",
        ""Identifier"": ""tenant1"",
        ""Name"": ""Tenant 1"",
        ""ConnectionString"": ""Tenant1Connection""
      },
      {
        ""Id"": ""sample-tenant-2"",
        ""Identifier"": ""tenant2"",
        ""Name"": ""Tenant 2"",
        ""ConnectionString"": ""Tenant2Connection""
      }
    ]
}")));
            }
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"MultiTenancyConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}