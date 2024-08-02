using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AspNetCore.MultiTenancy.Api;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenancyConfiguration;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenantStoreDbContext;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.TenantExtendedInfo;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.FactoryExtensions;

[IntentManaged(Mode.Fully, Body = Mode.Merge)]
public class AspNetCoreIntegrationExtension : FactoryExtensionBase
{
    public override string Id => "Intent.Modules.AspNetCore.MultiTenancy.AspNetCoreIntegrationExtension";
    private readonly IMetadataManager _metadataManager;

    public AspNetCoreIntegrationExtension(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    [IntentManaged(Mode.Ignore)] public override int Order => 0;

    private delegate void ApplyConfiguration(bool hasMultipleConnectionStrings);

    protected override void OnAfterTemplateRegistrations(IApplication application)
    {
        var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
        template?.CSharpFile.OnBuild(_ =>
        {
            template.StartupFile.AddServiceConfiguration(ctx => $"{ctx.Services}.ConfigureMultiTenancy({ctx.Configuration});");
        }, 100);

        template?.CSharpFile.AfterBuild(_ =>
        {
            template.StartupFile.ConfigureApp((statements, ctx) =>
            {
                var useRoutingStatement = statements.FindStatement(x => x.ToString()!.Contains(".UseRouting()"));
                if (useRoutingStatement == null)
                {
                    throw new("app.UseRouting() was not configured");
                }

                useRoutingStatement.InsertBelow($"{ctx.App}.UseMultiTenancy();");
            });
        }, 10);

        var configurations = new List<(string ConnectionStringName, ApplyConfiguration ApplyConfiguration)>();
        foreach (var tryGetConfiguration in new[] { TryGetEfCoreConfiguration, TryGetMongoDbConfiguration })
        {
            if (!tryGetConfiguration(application, _metadataManager, out var connectionStringName, out var applyConfiguration))
            {
                continue;
            }

            configurations.Add((connectionStringName, applyConfiguration));
        }

        var hasMultipleConfigurations = configurations.Count > 1;
        if (hasMultipleConfigurations)
        {
            var tenantInfoTemplate = application.FindTemplateInstance<TenantExtendedInfoTemplate>(TenantExtendedInfoTemplate.TemplateId);
            tenantInfoTemplate.SetCanRun(true);

            UpdateMultiTenancyConfigurationTemplate(application, configurations.Select(x => x.ConnectionStringName).ToArray());
            UpdateMultiTenantStoreDbContextTemplate(application);
        }

        foreach (var configuration in configurations)
        {
            configuration.ApplyConfiguration(hasMultipleConfigurations);
        }
    }

    private static bool TryGetEfCoreConfiguration(IApplication application, IMetadataManager metadataManager, out string connectionStringName, out ApplyConfiguration applyConfiguration)
    {
        var efDbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Data.DbContext");
        if (efDbContext == null)
        {
            connectionStringName = default;
            applyConfiguration = default;
            return false;
        }

        const string connectionStringNameInternal = "EntityFrameworkCore";

        connectionStringName = connectionStringNameInternal;
        applyConfiguration =
            application.Settings.GetMultitenancySettings().DataIsolation().AsEnum() switch
            {
                MultitenancySettings.DataIsolationOptionsEnum.SeparateDatabase =>
                    hasMultiConnStr => GetSeparateDatabaseDataIsolationConfiguration(hasMultiConnStr, application, connectionStringNameInternal),
                MultitenancySettings.DataIsolationOptionsEnum.SharedDatabase =>
                    _ => GetSharedDatabaseDataIsolationConfiguration(application, efDbContext, metadataManager),
                _ => throw new ArgumentOutOfRangeException()
            };

        return true;
    }

    private static void GetSharedDatabaseDataIsolationConfiguration(
        IApplication application,
        ICSharpFileBuilderTemplate dbContextTemplate,
        IMetadataManager metadataManager)
    {
        if (!application.Settings.GetMultitenancySettings().DataIsolation().IsSharedDatabase())
        {
            return;
        }

        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
        if (template == null)
        {
            return;
        }

        template.AddNugetDependency(NugetPackages.FinbuckleMultiTenant(template.OutputTarget));
        template.AddNugetDependency(NugetPackages.FinbuckleMultiTenantEntityFrameworkCore(template.OutputTarget));

        dbContextTemplate.CSharpFile.AfterBuild(file =>
        {
            file.AddUsing("System.Threading");
            file.AddUsing("System.Threading.Tasks");
            file.AddUsing("Finbuckle.MultiTenant");
            file.AddUsing("Finbuckle.MultiTenant.EntityFrameworkCore");

            var priClass = file.Classes.First();
            priClass.ImplementsInterface("IMultiTenantDbContext");

            var ctor = priClass.Constructors.First();
            ctor.AddParameter("ITenantInfo", "tenantInfo");
            ctor.AddStatement("TenantInfo = tenantInfo;");

            priClass.AddProperty("ITenantInfo", "TenantInfo", prop => prop.PrivateSetter());
            priClass.AddProperty("TenantMismatchMode", "TenantMismatchMode", prop => prop.WithInitialValue("TenantMismatchMode.Throw"));
            priClass.AddProperty("TenantNotSetMode", "TenantNotSetMode", prop => prop.WithInitialValue("TenantNotSetMode.Throw"));

            var syncSave = dbContextTemplate.GetSaveChangesMethod();

            var asyncSave = dbContextTemplate.GetSaveChangesAsyncMethod();

            syncSave.FindStatement(stmt => stmt.GetText("").Contains("return"))
                .InsertAbove("this.EnforceMultiTenant();");
            asyncSave.FindStatement(stmt => stmt.GetText("").Contains("return"))
                .InsertAbove("this.EnforceMultiTenant();");
        });


        var entityTypeConfigTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.EntityTypeConfiguration"));
        foreach (var entityTypeTemplate in entityTypeConfigTemplates)
        {
            entityTypeTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Finbuckle.MultiTenant.EntityFrameworkCore");

                var priClass = file.Classes.First();
                var configMethod = priClass.FindMethod("Configure");
                if (!configMethod.HasMetadata("model"))
                {
                    return;
                }
                var classModel = configMethod.GetMetadata<IElement>("model").AsClassModel();
                if (classModel.HasMultiTenant())
                {
                    configMethod.AddStatement("builder.IsMultiTenant();");
                }
            });
        }
        if (entityTypeConfigTemplates.Any())//We are dealing with EF
        {
            var problem = metadataManager.Domain(application).GetClassModels()
                .Where(x => x.InternalElement.Package.AsDomainPackageModel()?.HasStereotype("Relational Database") == true &&
                    !x.InternalElement.AsClassModel().IsAggregateRoot() &&
                    x.HasMultiTenant() &&
                    !x.HasStereotype("Table") // has Table stereotype
                    ).FirstOrDefault();
            if (problem != null)
            {
                throw new ElementException(problem.InternalElement, "Composite/Owned entities cannot be have the  `Multi Tenant` stereotype. Either remove the stereotype or apply the `Table` stereotype it.");
            }
        }
    }

    private static void GetSeparateDatabaseDataIsolationConfiguration(bool hasMultipleConnectionStrings, IApplication application, string connectionStringNameInternal)
    {
        if (!application.Settings.GetMultitenancySettings().DataIsolation().IsSeparateDatabase())
        {
            return;
        }

        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
        if (template == null)
        {
            return;
        }

        template.AddNugetDependency(NugetPackages.FinbuckleMultiTenant(template.OutputTarget));
        template.AddNugetDependency(NugetPackages.FinbuckleMultiTenantEntityFrameworkCore(template.OutputTarget));

        template.CSharpFile.AfterBuild(file =>
        {
            var (castTo, indexer) = hasMultipleConnectionStrings
                ? ($"({template.GetTypeName(TenantExtendedInfoTemplate.TemplateId)})", $"[\"{connectionStringNameInternal}\"]")
                : (string.Empty, string.Empty);

            var method = file.Classes.First().FindMethod("AddInfrastructure");
            if (method == null)
            {
                return;
            }

            method.FindAndReplaceStatement(x => x.HasMetadata("is-connection-string"), $"tenantInfo{indexer}.ConnectionString");

            method.FindStatement(x => x.GetText(string.Empty).StartsWith("options.Use"))
                .InsertAbove(
                    $@"var tenantInfo = {castTo}sp.GetService<{template.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new Finbuckle.MultiTenant.MultiTenantException(""Failed to resolve tenant info."");");
        });
    }

    private static bool TryGetMongoDbConfiguration(IApplication application, IMetadataManager metadataManager, out string connectionStringName, out ApplyConfiguration applyConfiguration)
    {
        var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Configuration.MongoDb.MultiTenancy");
        if (template == null)
        {
            connectionStringName = default;
            applyConfiguration = default;
            return false;
        }

        if (application.Settings.GetMultitenancySettings().DataIsolation().IsSharedDatabase())
        {
            throw new Exception("Shared Database Data Isolation Mode is not supported for Mongo DB Multi-Tenancy.");
        }

        const string connectionStringNameInternal = "MongoDb";

        connectionStringName = connectionStringNameInternal;
        applyConfiguration = hasMultipleConnectionStrings =>
        {
            template.CSharpFile.AfterBuild(file =>
            {
                if (!application.Settings.GetMultitenancySettings().DataIsolation().IsSeparateDatabase())
                {
                    return;
                }

                var (castTo, indexer) = hasMultipleConnectionStrings
                    ? ($"({template.GetTypeName(TenantExtendedInfoTemplate.TemplateId)})", $"[\"{connectionStringNameInternal}\"]")
                    : (string.Empty, string.Empty);

                var method = file.Classes.First().FindMethod("AddMongoDbMultiTenancy");

                method?.FindAndReplaceStatement(
                    x => x.HasMetadata("get-tenant-info"),
                    $"var tenantInfo = {castTo}x.GetService<ITenantInfo>();");
                method?.FindAndReplaceStatement(
                    x => x.HasMetadata("return-connection"),
                    $"return tenantInfo == null ? default : new MongoPerTenantConnection(tenantInfo{indexer});");
            });
        };

        return true;
    }

    private static void UpdateMultiTenancyConfigurationTemplate(
        IApplication application,
        IReadOnlyCollection<string> connectionStringNames)
    {
        var template = application.FindTemplateInstance<MultiTenancyConfigurationTemplate>(MultiTenancyConfigurationTemplate.TemplateId);
        var extendedInfoTypeName = template.GetTypeName(TenantExtendedInfoTemplate.TemplateId);

        template.DefaultTenants = new[]
        {
            new
            {
                Id = "sample-tenant-1",
                Identifier = "tenant1",
                Name = "Tenant 1",
                ConnectionStrings = connectionStringNames
                    .Select(connectionStringName => new
                    {
                        Name = connectionStringName,
                        Value = $"Tenant1{connectionStringName}Connection"
                    })
                    .ToArray()
            },
            new
            {
                Id = "sample-tenant-2",
                Identifier = "tenant2",
                Name = "Tenant 2",
                ConnectionStrings = connectionStringNames
                    .Select(connectionStringName => new
                    {
                        Name = connectionStringName,
                        Value = $"Tenant2{connectionStringName}Connection"
                    })
                    .ToArray()
            }
        };

        template.CSharpFile.AfterBuild(file =>
        {
            file.AddUsing("System.Collections.Generic");

            // ConfigureMultiTenancy
            {
                var method = file.Classes.First().FindMethod("ConfigureMultiTenancy");

                method?
                    .FindStatement(x => x.HasMetadata("add-multi-tenant"))?
                    .FindAndReplace("TenantInfo", extendedInfoTypeName);

                method?
                    .FindStatement(x => x.HasMetadata("with-ef-core-store"))?
                    .FindAndReplace("TenantInfo", extendedInfoTypeName);
            }

            // SetupInMemoryStore
            {
                var method = file.Classes.First().FindMethod("SetupInMemoryStore");
                if (method != null)
                {
                    method.Parameters[0] = new($"InMemoryStoreOptions<{extendedInfoTypeName}>", "options", file);
                }
            }

            // InitializeStore
            {
                var method = file.Classes.First().FindMethod("InitializeStore");

                method?
                    .FindStatement(x => x.HasMetadata("get-multi-tenant-store"))?
                    .FindAndReplace("TenantInfo", extendedInfoTypeName);

                method?
                    .FindStatement(x => x.HasMetadata("add-tenant1"))?
                    .FindAndReplace("TenantInfo", extendedInfoTypeName)?
                    .FindAndReplace($"ConnectionString = \"Tenant1Connection\"", ConnectionStrings("Tenant1"));

                method?
                    .FindStatement(x => x.HasMetadata("add-tenant2"))?
                    .FindAndReplace("TenantInfo", extendedInfoTypeName)
                    .FindAndReplace("ConnectionString = \"Tenant2Connection\"", ConnectionStrings("Tenant2"));

                string ConnectionStrings(string tenantName)
                {
                    var connectionStrings = connectionStringNames
                        .Select(connectionStringName =>
                            $"new TenantConnectionString {{ Name = \"{connectionStringName}\", Value = \"{tenantName}{connectionStringName}Connection\" }}");

                    return
                        $"ConnectionStrings = new List<TenantConnectionString> {{ {string.Join(", ", connectionStrings)} }}";
                }
            }
        });
    }

    private static void UpdateMultiTenantStoreDbContextTemplate(IApplication application)
    {
        var template = application.FindTemplateInstance<MultiTenantStoreDbContextTemplate>(MultiTenantStoreDbContextTemplate.TemplateId);
        if (template == null)
        {
            return;
        }

        var extendedInfoTypeName = template.GetTypeName(TenantExtendedInfoTemplate.TemplateId);

        template.CSharpFile.AfterBuild(file =>
        {
            file
                .AddUsing("Microsoft.EntityFrameworkCore.Metadata.Builders")
                .AddUsing("Microsoft.Extensions.Configuration")
                ;

            var @class = file.Classes.Single();
            @class.Constructors[0].AddParameter("IConfiguration", "configuration", p => p.IntroduceField(field => field.PrivateReadOnly()));
            @class.WithBaseType($"EFCoreStoreDbContext<{extendedInfoTypeName}>");

            @class
                .AddMethod("void", "OnModelCreating", method => method
                    .Protected()
                    .Override()
                    .AddParameter("ModelBuilder", "modelBuilder")
                    .AddStatement("ConfigureCustomTenantInfo(modelBuilder.Entity<TenantExtendedInfo>());")
                )
                .AddMethod("void", "ConfigureCustomTenantInfo", method => method
                    .Private()
                    .Static()
                    .AddParameter($"EntityTypeBuilder<{extendedInfoTypeName}>", "builder")
                    .AddStatement("builder.HasKey(x => x.Id);")
                    .AddStatement("builder.Property(ti => ti.Id).HasMaxLength(64);")
                    .AddStatement("builder.HasIndex(ti => ti.Identifier).IsUnique();")
                    .AddStatement(new CSharpMethodChainStatement("builder.Property(x => x.Identifier)")
                        .AddChainStatement("IsRequired()"), s => s.SeparatedFromPrevious())
                    .AddStatement(new CSharpMethodChainStatement("builder.Property(x => x.Name)")
                        .AddChainStatement("IsRequired()"), s => s.SeparatedFromPrevious())
                    .AddStatement("builder.OwnsMany(x => x.ConnectionStrings, ConfigureConnectionString);", s => s.SeparatedFromPrevious())
                )
                .AddMethod("void", "ConfigureConnectionString", method => method
                    .Private()
                    .Static()
                    .AddParameter($"OwnedNavigationBuilder<{extendedInfoTypeName}, TenantConnectionString>", "builder")
                    .AddStatement(new CSharpMethodChainStatement("builder.WithOwner()")
                        .AddChainStatement("HasForeignKey(x => x.CustomTenantInfoId)"))
                    .AddStatement("builder.HasKey(x => x.Id);", s => s.SeparatedFromPrevious())
                    .AddStatement(new CSharpMethodChainStatement("builder.Property(x => x.CustomTenantInfoId)")
                        .AddChainStatement("IsRequired()"), s => s.SeparatedFromPrevious())
                    .AddStatement(new CSharpMethodChainStatement("builder.Property(x => x.Name)")
                        .AddChainStatement("IsRequired()")
                        .AddChainStatement("HasMaxLength(64)"), s => s.SeparatedFromPrevious())
                    .AddStatement("builder.Property(x => x.Value);", s => s.SeparatedFromPrevious())
                );
        });
    }
}