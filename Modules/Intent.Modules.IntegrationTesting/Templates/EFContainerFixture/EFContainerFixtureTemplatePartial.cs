using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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
                .AddClass($"RBBMSFixture", @class =>
                {
                    var dbStrategy = ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum() switch
                    {
                        DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer => GetSqlServerStrategy(),
                        DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql => GetPostgresStrategy(),
                        _ => throw new Exception($"Integration Testings patterns do not currently support : {ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().Value}"),
                    };
                    AddUsing("System.Reflection");
                    AddUsing("DotNet.Testcontainers.Builders");
                    AddUsing("DotNet.Testcontainers.Configurations");
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
                    @class.AddField(dbStrategy.ContainerType, "_dbContainer", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddStatements(dbStrategy.ContainerInitialization);
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
                        method.AddStatements(dbStrategy.DbContextRegistration);

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
                                .AddStatement("await _dbContainer.StartAsync();");
                        });

                        @class.AddMethod("Task", "DisposeAsync", method =>
                        {
                            method
                                .Async()
                                .AddStatement("await _dbContainer.StopAsync();");
                        });
                    });
                });
        }

        private DbStrategy GetPostgresStrategy()
        {
            var containerInitialization = @"_dbContainer = new PostgreSqlBuilder()
        .WithImage(""postgres:14.7"")
        .WithDatabase(""db"")
        .WithUsername(""postgres"")
        .WithPassword(""postgres"")
        .WithCleanUp(true)
        .Build();"
                .ConvertToStatements();

            var dbContextRegistration = @"services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {{
                    options.UseNpgsql(
                        _dbContainer.GetConnectionString(),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    options.UseLazyLoadingProxies();
                }});
".ConvertToStatements();

            return new DbStrategy(
                containerType: "PostgreSqlContainer ",
                usings: new() { "Testcontainers.PostgreSql" },
                nuGetPackages: new() { NugetPackages.TestcontainersPostgreSql },
                containerInitialization: containerInitialization,
                dbContextRegistration: dbContextRegistration
                );
        }

        private DbStrategy GetSqlServerStrategy()
        {
            var containerInitialization = @"_dbContainer = new MsSqlBuilder()
                .WithImage(""mcr.microsoft.com/mssql/server:2022-latest"")
                .WithPassword(""Strong_password_123!"")
                .Build();"
                .ConvertToStatements();

            var dbContextRegistration = @"services.AddDbContext<ApplicationDbContext>((sp, options) =>
                {{
                    options.UseSqlServer(
                        _dbContainer.GetConnectionString(),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    options.UseLazyLoadingProxies();
                }});
".ConvertToStatements();

            return new DbStrategy(
                containerType: "MsSqlContainer",
                usings: new() { "Testcontainers.MsSql", "Microsoft.EntityFrameworkCore" },
                nuGetPackages: new() { NugetPackages.TestcontainersMsSql },
                containerInitialization: containerInitialization,
                dbContextRegistration: dbContextRegistration
                );
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ContainerHelper.RequireRDBMSEFContainer(this);
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