using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenancyConfiguration;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenantStoreDbContext;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.TenantExtendedInfo;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AspNetCoreIntegrationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.AspNetCore.MultiTenancy.AspNetCoreIntegrationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private delegate void ApplyConfiguration(bool hasMultipleConnectionStrings);

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.FindTemplateInstance<ICSharpFileBuilderTemplate>("App.Startup")
                ?.CSharpFile.AfterBuild(file =>
                {
                    file.Classes.First().FindMethod("ConfigureServices")
                        ?.AddStatement("services.ConfigureMultiTenancy(Configuration);");

                    var statement = file.Classes.First().FindMethod("Configure")
                        .FindStatement(s => s.GetText(string.Empty).Contains("app.UseRouting()"))
                        ?.InsertBelow("app.UseMultiTenancy();");

                    if (statement == null)
                    {
                        throw new("app.UseRouting() was not configured");
                    }
                });

            var configurations = new List<(string ConnectionStringName, ApplyConfiguration ApplyConfiguration)>();
            foreach (var tryGetConfiguration in new[] { TryGetEfCoreConfiguration, TryGetMongoDbConfiguration })
            {
                if (!tryGetConfiguration(application, out var connectionStringName, out var applyConfiguration))
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

        private static bool TryGetEfCoreConfiguration(IApplication application, out string connectionStringName, out ApplyConfiguration applyConfiguration)
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
            applyConfiguration = hasMultipleConnectionStrings =>
            {
                if (!application.Settings.GetMultitenancySettings().DataIsolation().IsSeparateDatabase())
                {
                    return;
                }

                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
                if (template == null)
                {
                    return;
                }

                template.AddNugetDependency(new NugetPackageInfo("Finbuckle.MultiTenant", "6.5.1"));
                template.AddNugetDependency(new NugetPackageInfo("Finbuckle.MultiTenant.EntityFrameworkCore", "6.5.1"));

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

                    method?.FindAndReplaceStatement(x => x.HasMetadata("is-connection-string"), $"tenantInfo{indexer}.ConnectionString");

                    method.FindStatement(x => x.GetText(string.Empty).StartsWith("options.Use"))
                        .InsertAbove($@"var tenantInfo = {castTo}sp.GetService<{template.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new {template.UseType("Finbuckle.MultiTenant.MultiTenantException")}(""Failed to resolve tenant info."");");
                });
            };

            return true;
        }

        private static bool TryGetMongoDbConfiguration(IApplication application, out string connectionStringName, out ApplyConfiguration applyConfiguration)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Configuration.MongoDb.MultiTenancy");
            if (template == null)
            {
                connectionStringName = default;
                applyConfiguration = default;
                return false;
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
                        method.Parameters[0] = new($"InMemoryStoreOptions<{extendedInfoTypeName}>", "options");
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
}