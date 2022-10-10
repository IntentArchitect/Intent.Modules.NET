using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreDbContextDecorator : DecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.DependencyInjection.EntityFrameworkCore.EntityFrameworkCoreDbContextDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly DbContextTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityFrameworkCoreDbContextDecorator(DbContextTemplate template, IApplication application)
        {
            _template = template;
            _application = application;

            _template.CSharpFile.OnBuild(file =>
            {
                if (!application.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
                {
                    file.AddUsing("Microsoft.Extensions.Options");
                }

                var @class = file.Classes.First();


                if (!_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
                {
                    @class.Constructors.Single().AddParameter($"IOptions<{_template.GetDbContextConfigurationName()}>", $"dbContextConfig", param =>
                    {
                        param.IntroduceReadonlyField();
                    });

                    @class.Methods.SingleOrDefault(x => x.Name == "OnModelCreating") 
                        ?.AddStatements(GetOnModelCreatingStatements(), statements => statements.FirstOrDefault()?.SeparatedFromPrevious());

                    @class.AddMethod("void", "EnsureDbCreated", method =>
                    {
                        method.WithComments(@"
/// <summary>
/// If configured to do so, a check is performed to see
/// whether the database exist and if not will create it
/// based on this container configuration.
/// </summary>");
                        method.AddStatements(@"
if (_dbContextConfig.Value.EnsureDbCreated == true)
{
    Database.EnsureCreated();
}");
                    });
                }
            });


        }
        public string GetOnModelCreatingStatements()
        {
            switch (_template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum())
            {
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql:
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer:
                    return @"
if (!string.IsNullOrWhiteSpace(_dbContextConfig.Value?.DefaultSchemaName))
{
    modelBuilder.HasDefaultSchema(_dbContextConfig.Value?.DefaultSchemaName);
}";
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos:
                    return @"
if (!string.IsNullOrWhiteSpace(_dbContextConfig.Value?.DefaultContainerName))
{
    modelBuilder.HasDefaultContainer(_dbContextConfig.Value?.DefaultContainerName);
}";
                case DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}