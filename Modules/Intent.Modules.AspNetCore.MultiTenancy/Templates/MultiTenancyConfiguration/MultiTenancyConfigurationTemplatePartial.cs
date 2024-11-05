using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Multitenancy;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.Win32.SafeHandles;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenancyConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MultiTenancyConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.MultiTenancyConfiguration";
        private readonly List<MultitenantConnectionStringRegistrationRequest> _connectionRequests = [];

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public MultiTenancyConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FinbuckleMultiTenant(outputTarget));
            AddNugetDependency(NugetPackages.FinbuckleMultiTenantAspNetCore(outputTarget));
            FulfillsRole(TemplateRoles.Distribution.WebApi.MultiTenancyConfiguration);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Finbuckle.MultiTenant.Stores")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass("MultiTenancyConfiguration", @class =>
                {
                    @class
                        .Static()
                        .AddMethod("IServiceCollection", "ConfigureMultiTenancy", method => method
                            .Static()
                            .AddParameter("IServiceCollection", "services", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            .AddStatement(new CSharpMethodChainStatement($"services.AddMultiTenant<{GetTenantClass()}>()"), statement =>
                            {
                                var methodChainStatement = (CSharpMethodChainStatement)statement;
                                methodChainStatement.AddMetadata("add-multi-tenant", true);
                                methodChainStatement.WithoutSemicolon();

                                switch (ExecutionContext.Settings.GetMultitenancySettings().Store().AsEnum())
                                {
                                    case MultitenancySettings.StoreOptionsEnum.InMemory:
                                        methodChainStatement.AddChainStatement("WithInMemoryStore(SetupInMemoryStore) // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#in-memory-store");
                                        break;
                                    case MultitenancySettings.StoreOptionsEnum.Efcore:
                                        methodChainStatement.AddChainStatement($"WithEFCoreStore<{this.GetMultiTenantStoreDbContextName()}, {GetTenantClass()}>() // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#efcore-store", s =>
                                            s.AddMetadata("with-ef-core-store", true));
                                        break;
                                    case MultitenancySettings.StoreOptionsEnum.Configuration:
                                        methodChainStatement.AddChainStatement("WithConfigurationStore() // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#configuration-store");
                                        break;
                                    case MultitenancySettings.StoreOptionsEnum.HttpRemote:
                                        methodChainStatement.AddChainStatement("WithHttpRemoteStore(configuration[\"Finbuckle:MultiTenant:Stores:HttpRemoteEndpointTemplate\"]!) // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Stores#http-remote-store");
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                switch (ExecutionContext.Settings.GetMultitenancySettings().Strategy().AsEnum())
                                {
                                    case MultitenancySettings.StrategyOptionsEnum.Header:
                                        methodChainStatement.AddChainStatement(
                                            "WithHeaderStrategy(\"X-Tenant-Identifier\"); // See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#header-strategy");
                                        break;
                                    case MultitenancySettings.StrategyOptionsEnum.Claim:
                                        methodChainStatement.AddChainStatement(
                                            "WithClaimStrategy(); // default claim value with type __tenant__. See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#claim-strategy");
                                        break;
                                    case MultitenancySettings.StrategyOptionsEnum.Host:
                                        methodChainStatement.AddChainStatement(
                                            "WithHostStrategy(); // default pattern is __tenant__.* (e.g. https://tenantidentifier.example.com). See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#host-strategy");
                                        break;
                                    case MultitenancySettings.StrategyOptionsEnum.Route:
                                        methodChainStatement.AddChainStatement(
                                            $"WithRouteStrategy(\"{this.ExecutionContext.GetSettings().GetMultitenancySettings().RouteStrategyParameter()}\"); // example https://www.example.com/tenantidentifier/home/). See https://www.finbuckle.com/MultiTenant/Docs/v6.12.0/Strategies#route-strategy");
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            })
                            .AddStatement("return services;")
                        )
                        .AddMethod("void", "UseMultiTenancy", method =>
                        {
                            method
                                .Static()
                                .AddParameter("IApplicationBuilder", "app", p => p.WithThisModifier())
                                .AddStatement("app.UseMultiTenant();");

                            if (!ExecutionContext.Settings.GetMultitenancySettings().Store().IsConfiguration())
                            {
                                method.AddStatement("InitializeStore(app.ApplicationServices);");
                            }
                        });

                    if (ExecutionContext.Settings.GetMultitenancySettings().Store().IsInMemory())
                    {
                        @class
                            .AddMethod("void", "SetupInMemoryStore", method => method
                                .AddAttribute("[IntentManaged(Mode.Fully, Body = Mode.Ignore)]")
                                .Private()
                                .Static()
                                .AddParameter($"InMemoryStoreOptions<{GetTenantClass()}>", "options")
                                .AddStatement("// configure in memory store:")
                                .AddStatement("options.IsCaseSensitive = false;")
                            );
                    }

                    if (!ExecutionContext.Settings.GetMultitenancySettings().Store().IsConfiguration())
                    {
                        @class
                            .AddMethod("void", "InitializeStore", method =>
                            {
                                method
                                    .AddAttribute("[IntentManaged(Mode.Fully, Body = Mode.Ignore)]")
                                    .Static()
                                    .AddParameter("IServiceProvider", "sp")
                                    .AddStatement("var scopeServices = sp.CreateScope().ServiceProvider;")
                                    .AddStatement($"var store = scopeServices.GetRequiredService<IMultiTenantStore<{GetTenantClass()}>>();", s => s
                                        .AddMetadata("get-multi-tenant-store", true)
                                        .SeparatedFromNext());

                                var tenantList = GetDefaultTenants();
                                foreach (var tenant in tenantList.Tenants)
                                {
                                    method
                                        .AddStatement($"store.TryAddAsync(new {GetTenantClass()}() {{ {string.Join(",", tenant.Select(kvp => $"{kvp.Key} = \"{kvp.Value}\""))} }}).Wait();", s => s
                                            .AddMetadata($"add-{tenant["Identifier"]}", true));
                                }
                                /*
                                method
                                    .AddStatement($"store.TryAddAsync(new {GetTenantClass()}() {{ Id = \"sample-tenant-1\", Identifier = \"tenant1\", Name = \"Tenant 1\" {GetConnectionStrings("tenant1")} }}).Wait();", s => s
                                        .AddMetadata("add-tenant1", true))
                                    .AddStatement($"store.TryAddAsync(new {GetTenantClass()}() {{ Id = \"sample-tenant-2\", Identifier = \"tenant2\", Name = \"Tenant 2\" {GetConnectionStrings("tenant2")} }}).Wait();", s => s
                                        .AddMetadata("add-tenant2", true))*/
                            });
                    }
                });
            ExecutionContext.EventDispatcher.Subscribe<MultitenantConnectionStringRegistrationRequest>(Handle);
        }

        private string GetTenantClass()
        {
            if (_connectionRequests.Any())
            {
                return this.GetTenantExtendedInfoName();
            }
            else
            {
                return "TenantInfo";
            }
        }

        private void Handle(MultitenantConnectionStringRegistrationRequest @event)
        {
            _connectionRequests.Add(@event);
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            switch (ExecutionContext.Settings.GetMultitenancySettings().Store().AsEnum())
            {
                case MultitenancySettings.StoreOptionsEnum.Configuration:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Finbuckle:MultiTenant:Stores:ConfigurationStore", GetDefaultTenants()));
                    break;
                case MultitenancySettings.StoreOptionsEnum.Efcore:
                    break;
                case MultitenancySettings.StoreOptionsEnum.HttpRemote:
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("Finbuckle:MultiTenant:Stores:HttpRemoteEndpointTemplate", "https://example.com/{__tenant__}"));
                    break;
                case MultitenancySettings.StoreOptionsEnum.InMemory:
                    break;
                default:
                    break;
            }
        }

        private TenantList GetDefaultTenants()
        {
            var result = new TenantList(
                new[]
                {
                    new Dictionary<string, string>
                    {
                        { "Id" , "sample-tenant-1" },
                        { "Identifier" , "tenant1" },
                        { "Name" , "Tenant 1" },
                        { "ConnectionString" , "Tenant1Connection" }
                    },
                    new Dictionary<string, string>
                    {
                        { "Id" , "sample-tenant-2" },
                        { "Identifier" , "tenant2" },
                        { "Name" , "Tenant 2" },
                        { "ConnectionString" , "Tenant2Connection" }
                    }
                });
            if (_connectionRequests.Any())
            {
                foreach (var tenant in result.Tenants)
                {
                    foreach (var connection in _connectionRequests)
                    {
                        tenant.Add(connection.Name.ToCSharpIdentifier(), connection.ConnectionStringTemplate.Replace("{tenant}", tenant["Identifier"]));
                    }
                }
            }

            return result;
        }

        internal record TenantList(IEnumerable<Dictionary<string, string>> Tenants);

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig() => CSharpFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}