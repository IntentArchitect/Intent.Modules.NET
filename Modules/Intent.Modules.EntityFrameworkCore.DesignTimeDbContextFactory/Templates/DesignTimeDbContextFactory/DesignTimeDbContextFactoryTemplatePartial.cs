using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory.Templates.DesignTimeDbContextFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
    partial class DesignTimeDbContextFactoryTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFrameworkCore.DesignTimeDbContextFactory.DesignTimeDbContextFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DesignTimeDbContextFactoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationJson(OutputTarget));
            AddNugetDependency(NugetPackages.MicrosoftExtensionsConfigurationFileExtensions(OutputTarget));
            CSharpFile = new CSharpFile($"{this.GetNamespace()}", $"{this.GetFolderPath()}")
                .AddUsing("System.IO")
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddUsing("Microsoft.EntityFrameworkCore.Design")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass("DesignTimeDbContextFactory", @class =>
                {
                    @class.XmlComments.AddStatements(@$"
/// <summary>
/// In the event that one cannot run EF Core CLI commands due to Startup app constraints,
/// having this class present will bypass your startup app and rather look at an appsettings.json file
/// locally for connection-string info to construct an <see cref=""{GetDbContextName()}""/>. 
/// </summary>");
                    @class.Interfaces.Add($"IDesignTimeDbContextFactory<{GetDbContextName()}>");
                    @class.AddMethod(GetDbContextName(), "CreateDbContext", method =>
                    {
                        method.AddParameter("string[]", "args");
                        method.XmlComments.AddStatements(@"
/// <inheritdoc />
/// <param name=""args"">
/// This is optional but will only accept 1 parameter which is the name of the connection string to lookup
/// in a local appsettings.json file. By default this will use ""DefaultConnection"".
/// </param>");
                        method.AddStatement($@"var optionsBuilder = new DbContextOptionsBuilder<{GetDbContextName()}>();");
                        method.AddStatements(new[]
                        {
                            new CSharpStatement(@"IConfigurationRoot configuration = new ConfigurationBuilder()"),
                            new CSharpStatement(@".SetBasePath(Directory.GetCurrentDirectory())").Indent(),
                            new CSharpStatement(@".AddJsonFile(""appsettings.json"")").Indent(),
                            new CSharpStatement(@".Build();").Indent()
                        });
                        method.AddStatement("var connStringName = args.FirstOrDefault();");
                        method.AddStatements(new[]
                        {
                            new CSharpStatement(@"if (string.IsNullOrEmpty(connStringName))"),
                            new CSharpStatementBlock()
                                .AddStatement(@"connStringName = ""DefaultConnection"";")
                        });
                        method.AddStatement("var connectionString = configuration.GetConnectionString(connStringName);");

                        switch (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
                        {
                            case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                                method.AddStatement("optionsBuilder.UseInMemoryDatabase(connStringName);");
                                break;
                            case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                                method.AddStatement("optionsBuilder.UseSqlServer(connectionString);");
                                break;
                            case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                                method.AddStatement("optionsBuilder.UseNpgsql(connectionString);");
                                break;
                            case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql:
                                method.AddStatement(@"optionsBuilder.UseMySql(connectionString, ServerVersion.Parse(""8.0""));");
                                break;
                            case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                            default:
                                // NO OP
                                break;
                        }

                        method.AddInvocationStatement($"return new {GetDbContextName()}", stmt =>
                        {
                            stmt.AddMetadata("return-statement", true);
                            stmt.AddArgument("optionsBuilder.Options", arg => arg.AddMetadata("options", true));
                        });
                    });
                });
        }

        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DesignTimeDbContextFactory",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetDbContextName()
        {
            return GetTypeName("Infrastructure.Data.DbContext");
        }
    }
}