using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates.DbContextConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DbContextConfigurationTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.DependencyInjection.EntityFrameworkCore.DbContextConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DbContextConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DbContextConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetProperties()
        {
            var properties = new List<string>();

            switch (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                    properties.Add("public string? DefaultSchemaName { get; set; }");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    properties.Add("public string? DefaultContainerName { get; set; }");
                    properties.Add("public string? PartitionKey { get; set; }");
                    break;
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                default:
                    break;
            }

            const string newLine = @"
        ";
            return string.Join(newLine, properties);
        }
    }
}