using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantClientProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBMultiTenantClientProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBMultiTenantClientProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBMultiTenantClientProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpClass mainClass = null;
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Data.Common")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Azure.CosmosRepository.Options")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultiTenantClientProvider", @class =>
                {
                    mainClass = @class;
                    AddNugetDependency(NugetPackages.FinbuckleMultiTenant(outputTarget));

                    @class.ImplementsInterfaces(new string[] { "ICosmosClientProvider", "IDisposable" });

                    @class.AddField("AsyncLocal<ScopedData?>", "_scopedData", f => f.PrivateReadOnly().WithAssignment("new AsyncLocal<ScopedData?>()"));
                    @class.AddField("Dictionary<string, ConnectionInfo>", "_clients", f => f.PrivateReadOnly().WithAssignment("new Dictionary<string, ConnectionInfo>()"));
                    @class.AddField("object", "_lock", f => f.PrivateReadOnly().WithAssignment("new object()"));
                    @class.AddField("string", "_defaultDatabaseName", f => f.PrivateReadOnly());
                    @class.AddField("string", "_defaultContainerName", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddStatement("var repositoryOptions = new RepositoryOptions();");
                        ctor.AddStatement("configuration.GetSection(\"RepositoryOptions\").Bind(repositoryOptions);");
                        ctor.AddStatement("_defaultContainerName = repositoryOptions.ContainerId;");
                        ctor.AddStatement("_defaultDatabaseName = repositoryOptions.DatabaseId;");
                    });

                    // Typed as the base ITenantInfo - the app's DI accessor may be closed over a custom TTenantInfo
                    // (e.g. TenantExtendedInfo for separate-database multi-tenancy), but ITenantInfo is always a
                    // valid supertype reference here; no cross-module type is needed at this level.
                    @class.AddProperty("ITenantInfo?", "Tenant", p => p.WithoutSetter().Getter.WithImplementation("_scopedData?.Value?.Tenant"));
                    @class.AddProperty("CosmosClient", "CosmosClient", p => p.WithoutSetter().Getter.WithImplementation("GetClient()"));

                    @class.AddMethod("string", "GetDatabase", method =>
                    {
                        method.AddStatement("return GetTenantConnectionInfo().Database;");
                    });
                    @class.AddMethod("string", "GetDefaultContainer", method =>
                    {
                        method.AddStatement("return GetTenantConnectionInfo().DefaultContainer;");
                    });
                    @class.AddMethod("CosmosClient", "GetClient", method =>
                    {
                        method.Private();
                        method.AddStatement("return GetTenantConnectionInfo().Client;");
                    });
                    // GetTenantConnectionInfo is added later, in CSharpFile.OnBuild below - its body needs to cast
                    // `tenantInfo` to the app's concrete extended tenant type to reach `.CosmosDbConnection` (base
                    // ITenantInfo/TenantInfo lost `ConnectionString` at Finbuckle 9.x), resolved via a cross-module
                    // GetTypeName lookup (Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo, no
                    // ProjectReference to that module). GetTypeName must not be called here in the constructor -
                    // other modules' templates are not guaranteed to be registered yet at construction time, and it
                    // throws if the owning template hasn't been registered. This template only runs for
                    // separate-database multi-tenancy (see CanRunTemplate); `MultiTenancyFactoryExtension` registers
                    // CosmosDB as a connection-string requester so TenantExtendedInfo always generates a named
                    // `CosmosDbConnection` property (not a bare `ConnectionString`) regardless of which other
                    // separate-database modules are also installed.
                    @class.AddMethod("void", "GetValuesFromConnectionString", method =>
                    {
                        method
                            .Private()
                            .AddParameter("string", "connectionString")
                            .AddParameter("string?", "database", p => p.WithOutParameterModifier())
                            .AddParameter("string?", "defaultContainer", p => p.WithOutParameterModifier());
                        method.AddStatements(@$"database = null;
			defaultContainer = null;
			if (connectionString == null)
			{{
				throw new ArgumentNullException(nameof(connectionString));
			}}

			var builder = new DbConnectionStringBuilder {{ ConnectionString = connectionString }};
			if (builder.TryGetValue(""Database"", out object dbValue))
			{{
				database = dbValue.ToString();
			}}
			if (builder.TryGetValue(""Container"", out object containerValue))
			{{
				defaultContainer = containerValue.ToString();
			}}".ConvertToStatements());
                    });
                    @class.AddMethod("void", "Dispose", method =>
                    {
                        method.AddForEachStatement("connection", "_clients.Values", stmt =>
                        {
                            stmt.AddStatement("connection.Client.Dispose();");
                        });
                    });
                    @class.AddMethod("Task<T>", "UseClientAsync<T>", method =>
                    {
                        method
                            .AddParameter("Func<CosmosClient, Task<T>>", "consume")
                            .WithExpressionBody("consume.Invoke(GetClient())");
                    });

                    @class.AddMethod("IDisposable", "SetLocalState", method =>
                    {
                        method
                            .AddParameter("ITenantInfo?", "tenant")
                            .AddParameter("ICosmosClientOptionsProvider", "cosmosClientOptionsProvider")
                            ;
                        method.AddStatement("_scopedData.Value ??= new ScopedData(tenant, cosmosClientOptionsProvider, () => _scopedData.Value = null);");
                        method.AddStatement("return _scopedData.Value;");
                    });
                    @class.AddNestedClass("ConnectionInfo", nested =>
                    {
                        nested.Private();
                        nested.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("CosmosClient", "client", p => p.IntroduceProperty(x => x.ReadOnly()));
                            ctor.AddParameter("string", "database", p => p.IntroduceProperty(x => x.ReadOnly()));
                            ctor.AddParameter("string", "defaultContainer", p => p.IntroduceProperty(x => x.ReadOnly()));
                        });
                    });
                    @class.AddNestedClass("ScopedData", nested =>
                    {
                        nested
                            .Private()
                            .ImplementsInterface("IDisposable");
                        nested.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("ITenantInfo?", "tenant", p => p.IntroduceProperty(x => x.ReadOnly()));
                            ctor.AddParameter("ICosmosClientOptionsProvider", "cosmosClientOptionsProvider");
                            ctor.AddParameter("Action", "disposeAction", p => p.IntroduceReadonlyField());
                            ctor.AddStatement("ClientOptions = cosmosClientOptionsProvider.ClientOptions;");
                        });
                        nested.AddProperty("CosmosClientOptions", "ClientOptions", p => p.WithoutSetter());
                        nested.AddMethod("void", "Dispose", m => m.WithExpressionBody("_disposeAction()"));
                    });
                });

            CSharpFile.OnBuild(file =>
            {
                var tenantInfoTypeName = this.GetTypeName("Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo");
                mainClass.AddMethod("ConnectionInfo", "GetTenantConnectionInfo", method =>
                {
                    method.Private();
                    method.AddStatements(@$"if (_scopedData.Value == null || _scopedData.Value.Tenant == null || _scopedData.Value.Tenant.Id == null)
			{{
				throw new ArgumentNullException(""Tenant info not found, unable to determine which database to access"");
			}}
			var tenantInfo = ({tenantInfoTypeName})_scopedData.Value.Tenant;
			if (!_clients.TryGetValue(tenantInfo.Id, out var connectionInfo))
			{{
				lock (_lock)
				{{
					if (!_clients.TryGetValue(tenantInfo.Id, out connectionInfo))
					{{
						string[] settings = tenantInfo.CosmosDbConnection.Split("";"");
						var clientOptions = _scopedData.Value.ClientOptions;
						var client = new CosmosClient(tenantInfo.CosmosDbConnection, clientOptions);
						GetValuesFromConnectionString(tenantInfo.CosmosDbConnection, out var database, out var defaultContainer);
						if (database == null)
						{{
							database = _defaultDatabaseName;
						}}
						if (defaultContainer == null)
						{{
							defaultContainer = _defaultContainerName;
						}}
						connectionInfo = new ConnectionInfo(client, database, defaultContainer);
						_clients.Add(tenantInfo.Id, connectionInfo);
					}}
				}}
			}}
			return connectionInfo;".ConvertToStatements());
                });
            }, 1000);
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(ExecutionContext.Settings);
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
