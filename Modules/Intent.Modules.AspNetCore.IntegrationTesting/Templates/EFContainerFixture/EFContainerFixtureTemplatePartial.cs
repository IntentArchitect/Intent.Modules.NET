using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.EFContainerFixture
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EFContainerFixtureTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.EFContainerFixture";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EFContainerFixtureTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"EFContainerFixture", @class =>
                {
                    var dbStrategy = GetDbStrategy(outputTarget);

                    AddUsing("System.Reflection");
                    AddUsing("Microsoft.Extensions.DependencyInjection");
                    AddUsing("Microsoft.Extensions.Options");
                    foreach (var clause in dbStrategy.Usings)
                    {
                        AddUsing(clause);
                    }
                    foreach (var nuGet in dbStrategy.NuGetPackages)
                    {
                        AddNugetDependency(nuGet);
                    }
                    @class.AddField(dbStrategy.FieldType, dbStrategy.FieldName, f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatements(dbStrategy.FieldInitialization);
                    });

                    @class.AddMethod("void", "ConfigureTestServices", method =>
                    {
                        method.AddParameter("IServiceCollection", "services");
                        method.AddStatements(@$"var descriptor = services.SingleOrDefault(s => s.ServiceType  == typeof( DbContextOptions<{GetTypeName(TemplateRoles.Infrastructure.Data.DbContext)}>));
                if (descriptor is not null)
                {{
                    services.Remove(descriptor);
                }}
".ConvertToStatements());
                        method.AddStatement("services.AddDbContext<ApplicationDbContext>((sp, options) => { });", s => s.AddMetadata("db-context-reconfigure", "true"));

                        method.AddStatements(@"
                //Schema Creation
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

".ConvertToStatements());

                        @class.AddMethod("void", "OnHostCreation", method =>
                        {
                            method
                                .AddParameter("IServiceProvider", "services");
                        });

                        @class.AddMethod("void", "InitializeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatements(dbStrategy.InitializeStatements);
                        });

                        @class.AddMethod("Task", "DisposeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatements(dbStrategy.DisposeStatements);
                        });
                    });
                })
                .OnBuild(f =>
                {
                    var dbStrategy = GetDbStrategy(outputTarget);

                    var @class = f.TypeDeclarations.First();
                    var method = @class.FindMethod("ConfigureTestServices");
                    var statement = method.FindStatement(s => s.HasMetadata("db-context-reconfigure"));

                    if (this.TryGetTemplate<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection", out var containerTemplate))
                    {
                        var regMethod = containerTemplate.CSharpFile.Classes.First().FindMethod("AddInfrastructure");
                        if (regMethod == null)
                        {
                            return;
                        }
                        CSharpStatement dbContextStatement = null;
                        foreach (var line in regMethod.Statements)
                        {
                            if (line.GetText("").Trim().StartsWith("services.AddDbContext<ApplicationDbContext>"))
                            {
                                dbContextStatement = line;
                                break;
                            }
                        }

                        if (containerTemplate is IDeclareUsings declareUsings)
                        {
                            foreach (var @using in declareUsings.DeclareUsings())
                            {
                                AddUsing(@using);
                            }
                        }

                        var connectionStringText = (dbContextStatement as IHasCSharpStatements)?.FindStatement(x => x.HasMetadata("is-connection-string")).GetText("");
                        string dbContextReconfigure = dbContextStatement.GetText("").Replace(connectionStringText, dbStrategy.ConnectionExpression).Replace("                               ", "            ");
                        method.InsertStatements(method.Statements.IndexOf((CSharpStatement)statement), dbContextReconfigure.ConvertToStatements().ToList());
                        statement.Remove();
                    }

                }, 10000);
        }

        private DbStrategy GetDbStrategy(IOutputTarget outputTarget)
        {
            var databaseProvider = ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider();
            return databaseProvider.AsEnum() switch
            {
                DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer => GetSqlServerStrategy(outputTarget),
                DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql => GetPostgresStrategy(outputTarget),
                DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SQLLite => GetSqliteStrategy(),
                _ => throw new FriendlyException(
                    $"Integration Testing patterns do not currently support the '{databaseProvider.Value}' database provider. " +
                    "Change the Database Provider in Database Settings to SQL Server, PostgreSQL or SQLite, or uninstall Intent.AspNetCore.IntegrationTesting."),
            };
        }

        private DbStrategy GetPostgresStrategy(IOutputTarget outputTarget)
        {
            var containerInitialization = $@"{DbStrategy.ContainerFieldName} = new PostgreSqlBuilder()
        .WithImage(""postgres:14.7"")
        .WithDatabase(""db"")
        .WithUsername(""postgres"")
        .WithPassword(""postgres"")
        .WithCleanUp(true)
        .Build();"
                .ConvertToStatements();

            return DbStrategy.ForContainer(
                containerType: "PostgreSqlContainer ",
                usings: new() { "Testcontainers.PostgreSql", "Microsoft.EntityFrameworkCore" },
                nuGetPackages: new() { NugetPackages.TestcontainersPostgreSql(outputTarget) },
                containerInitialization: containerInitialization
                );
        }

        private DbStrategy GetSqlServerStrategy(IOutputTarget outputTarget)
        {
            var containerInitialization = $@"{DbStrategy.ContainerFieldName} = new MsSqlBuilder()
                .WithImage(""mcr.microsoft.com/mssql/server:2022-latest"")
                .WithPassword(""Strong_password_123!"")
                .Build();"
                .ConvertToStatements();

            return DbStrategy.ForContainer(
                containerType: "MsSqlContainer",
                usings: new() { "Testcontainers.MsSql", "Microsoft.EntityFrameworkCore" },
                nuGetPackages: new() { NugetPackages.TestcontainersMsSql(outputTarget) },
                containerInitialization: containerInitialization
                );
        }

        /// <summary>
        /// SQLite runs in-process, so there is no container to start — the fixture instead owns a
        /// <c>SqliteConnection</c> which is held open for its lifetime. An in-memory SQLite database
        /// exists only while a connection to it is open, so the connection itself (rather than a
        /// connection string) is what gets handed to EF Core; were EF to open and close its own
        /// connection per operation, the schema created below would be discarded before the first test ran.
        /// <para>
        /// No NuGet package is declared here: <c>Microsoft.EntityFrameworkCore.Sqlite</c> is added to the
        /// Infrastructure project by Intent.EntityFrameworkCore whenever this provider is selected, and
        /// flows transitively to the test project through its project reference.
        /// </para>
        /// </summary>
        private static DbStrategy GetSqliteStrategy()
        {
            var connectionInitialization = $@"{DbStrategy.ConnectionFieldName} = new SqliteConnection(""Filename=:memory:"");"
                .ConvertToStatements();

            return DbStrategy.ForConnection(
                connectionType: "SqliteConnection",
                usings: new() { "Microsoft.Data.Sqlite", "Microsoft.EntityFrameworkCore" },
                nuGetPackages: new(),
                connectionInitialization: connectionInitialization
                );
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ContainerHelper.RequireRdbmsEFContainer(this);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}