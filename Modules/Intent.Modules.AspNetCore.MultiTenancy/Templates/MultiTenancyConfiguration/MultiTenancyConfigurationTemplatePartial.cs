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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
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
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddUsing("Finbuckle.MultiTenant.Stores.InMemoryStore")
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
                                        .AddStatement($"store.TryAddAsync(new {GetTenantClass()}() {{ {string.Join(", ", tenant.Select(kvp => $"{kvp.Key} = \"{kvp.Value}\""))} }}).Wait();", s => s
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
            // Finbuckle v7+ removed ConnectionString from the base TenantInfo, so separate-database
            // data isolation always needs the extended tenant-info type to carry it (even with a
            // single connection string) -- not just when multiple connection strings are registered.
            if (_connectionRequests.Any() || ExecutionContext.Settings.GetMultitenancySettings().DataIsolation().IsSeparateDatabase())
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
                    { "Name" , "Tenant 1" }
                    },
                    new Dictionary<string, string>
                    {
                    { "Id" , "sample-tenant-2" },
                    { "Identifier" , "tenant2" },
                    { "Name" , "Tenant 2" }
                    }
                });

            // Mirrors GetTenantClass()/TenantExtendedInfoTemplate: named connection requests and the
            // generic ConnectionString are mutually exclusive properties on the generated tenant class,
            // so the seeded sample tenants must only reference whichever one actually exists.
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
            else if (ExecutionContext.Settings.GetMultitenancySettings().DataIsolation().IsSeparateDatabase())
            {
                var tenants = result.Tenants.ToList();
                tenants[0].Add("ConnectionString", "Tenant1Connection");
                tenants[1].Add("ConnectionString", "Tenant2Connection");
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

        public override TemplateMetadata TemplateMetadata => new TemplateMetadata(TemplateId, "2.0");

        public override ITemplateMigration[] Migrations => new ITemplateMigration[] { new AlignStaleTenantInfoReferencesMigration(GetTenantClass, _connectionRequests) };

        // Finbuckle v7+ removed ConnectionString from the base TenantInfo class (see GetTenantClass() above).
        // Apps generated before this module's Finbuckle 9.4.10 upgrade may have hand-locked
        // (Body = Ignore) InitializeStore code seeding e.g.
        // `new TenantInfo() { Id = "sample-tenant-1", Identifier = "tenant1", Name = "Tenant 1", ConnectionString = "Tenant1Connection" }`
        // directly against the (then-valid) base TenantInfo.ConnectionString property. That code never
        // regenerates on its own, so it breaks three different ways once an app upgrades past this version:
        //  - No extended tenant type and no named connection request (GetTenantClass() == "TenantInfo"):
        //    the ConnectionString initializer no longer compiles at all, and there is nowhere left to put
        //    it, so it's removed.
        //  - Extended tenant type, no named connection request (separate-database data isolation only --
        //    GetTenantClass() == GetTenantExtendedInfoName()): the extended type still carries
        //    ConnectionString, but the stale body still constructs the *base* TenantInfo and asks for
        //    IMultiTenantStore<TenantInfo>, which no longer matches what AddMultiTenant<TExtended>()
        //    registers. That compiles today (TenantInfo itself is untouched) but throws a DI resolution
        //    error at runtime. Retarget the stale type to the extended type instead of dropping
        //    ConnectionString, since the extended type still has it.
        //  - A named connection-string request exists (Cosmos/Mongo/MongoFramework/GoogleCloudStorage
        //    installed) -- TenantExtendedInfoTemplate then generates a *named* property (e.g.
        //    CosmosDbConnection) instead of ConnectionString on the extended type (mutually exclusive, see
        //    GetTenantClass()/GetDefaultTenants() above), even when the stale body already used the
        //    extended type. `ConnectionString` no longer compiles there either. Replace it with one
        //    assignment per registered named connection, substituting the tenant's own Identifier value
        //    for the "{tenant}" placeholder -- the same substitution GetDefaultTenants() uses for newly
        //    generated sample tenants.
        public class AlignStaleTenantInfoReferencesMigration : ITemplateMigration
        {
            private const string BaseTenantInfoTypeName = "TenantInfo";
            private readonly Func<string> _getTargetTenantClass;
            private readonly IReadOnlyList<MultitenantConnectionStringRegistrationRequest> _connectionRequests;

            public AlignStaleTenantInfoReferencesMigration(
                Func<string> getTargetTenantClass,
                IReadOnlyList<MultitenantConnectionStringRegistrationRequest> connectionRequests)
            {
                _getTargetTenantClass = getTargetTenantClass;
                _connectionRequests = connectionRequests;
            }

            public string Execute(string currentText)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(currentText);
                var root = syntaxTree.GetRoot();
                using var workspace = new AdhocWorkspace();
                var editor = new SyntaxEditor(root, workspace.Services);

                // The tenant class to reconcile against is the one the template is *about to
                // (re)generate* for the app's current settings/modules -- i.e. GetTenantClass() --
                // NOT whatever the file currently says. A template migration runs on the PREVIOUS
                // output *before* regeneration, so the file's own services.AddMultiTenant<T>() still
                // carries the OLD type argument at this point (e.g. "TenantInfo" in a genuine
                // Finbuckle-6-era file, where the base TenantInfo still had ConnectionString).
                // Parsing it here misclassified real upgrades: the stale <TenantInfo> forced the
                // shared-database "strip ConnectionString" path even for separate-database apps, so
                // the Body = Ignore InitializeStore kept IMultiTenantStore<TenantInfo> /
                // new TenantInfo() while AddMultiTenant<T> regenerated to the extended type -- a
                // file that compiles but throws a DI resolution error at runtime.
                var targetTenantClass = _getTargetTenantClass() ?? BaseTenantInfoTypeName;

                if (targetTenantClass != BaseTenantInfoTypeName)
                {
                    RetargetStaleTenantInfoReferences(root, editor, targetTenantClass);
                }

                ReconcileConnectionStringAssignments(root, editor, targetTenantClass);

                return editor.GetChangedRoot().ToFullString();
            }

            // Bare `TenantInfo` object creations and the `IMultiTenantStore<TenantInfo>` DI lookup are
            // stale references to a type this app no longer registers -- retype them to whatever
            // AddMultiTenant<T>() actually configures.
            private static void RetargetStaleTenantInfoReferences(SyntaxNode root, SyntaxEditor editor, string extendedTenantClass)
            {
                var objectCreations = root.DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .Where(o => o.Type.ToString() == BaseTenantInfoTypeName);

                foreach (var objectCreation in objectCreations)
                {
                    editor.ReplaceNode(objectCreation.Type, SyntaxFactory.ParseTypeName(extendedTenantClass).WithTriviaFrom(objectCreation.Type));
                }

                var storeTypeArguments = root.DescendantNodes()
                    .OfType<GenericNameSyntax>()
                    .Where(g => g.Identifier.Text == "IMultiTenantStore"
                        && g.TypeArgumentList.Arguments.Count == 1
                        && g.TypeArgumentList.Arguments[0].ToString() == BaseTenantInfoTypeName)
                    .SelectMany(g => g.TypeArgumentList.Arguments);

                foreach (var typeArgument in storeTypeArguments)
                {
                    editor.ReplaceNode(typeArgument, SyntaxFactory.ParseTypeName(extendedTenantClass).WithTriviaFrom(typeArgument));
                }
            }

            // Handles a stale `ConnectionString` assignment regardless of which type it's on (bare
            // TenantInfo, or an extended type that used to carry ConnectionString before a named
            // connection request claimed the property instead).
            private void ReconcileConnectionStringAssignments(SyntaxNode root, SyntaxEditor editor, string currentTenantClass)
            {
                var objectCreations = root.DescendantNodes()
                    .OfType<ObjectCreationExpressionSyntax>()
                    .Where(o => o.Initializer != null
                        && (o.Type.ToString() == BaseTenantInfoTypeName || o.Type.ToString() == currentTenantClass));

                foreach (var objectCreation in objectCreations)
                {
                    var initializer = objectCreation.Initializer!;
                    var connectionStringAssignment = initializer.Expressions
                        .OfType<AssignmentExpressionSyntax>()
                        .FirstOrDefault(a => a.Left is IdentifierNameSyntax { Identifier.Text: "ConnectionString" });

                    if (connectionStringAssignment == null)
                    {
                        continue;
                    }

                    if (_connectionRequests.Count > 0)
                    {
                        var tenantIdentifier = initializer.Expressions
                            .OfType<AssignmentExpressionSyntax>()
                            .FirstOrDefault(a => a.Left is IdentifierNameSyntax { Identifier.Text: "Identifier" } && a.Right is LiteralExpressionSyntax)
                            ?.Right is LiteralExpressionSyntax identifierLiteral
                            ? identifierLiteral.Token.ValueText
                            : null;

                        var existingPropertyNames = initializer.Expressions
                            .OfType<AssignmentExpressionSyntax>()
                            .Where(a => a != connectionStringAssignment && a.Left is IdentifierNameSyntax)
                            .Select(a => ((IdentifierNameSyntax)a.Left).Identifier.Text)
                            .ToHashSet();

                        var namedAssignments = _connectionRequests
                            .Where(request => !existingPropertyNames.Contains(request.Name.ToCSharpIdentifier()))
                            .Select(request => (ExpressionSyntax)SyntaxFactory.ParseExpression(
                                $"{request.Name.ToCSharpIdentifier()} = \"{(tenantIdentifier != null ? request.ConnectionStringTemplate.Replace("{tenant}", tenantIdentifier) : request.ConnectionStringTemplate)}\""));

                        var withoutConnectionString = initializer.Expressions.Where(e => e != connectionStringAssignment);
                        var newExpressions = SyntaxFactory.SeparatedList(withoutConnectionString.Concat(namedAssignments));
                        editor.ReplaceNode(initializer, initializer.WithExpressions(newExpressions));
                    }
                    else if (currentTenantClass == BaseTenantInfoTypeName)
                    {
                        var newExpressions = initializer.Expressions.Remove(connectionStringAssignment);
                        editor.ReplaceNode(initializer, initializer.WithExpressions(newExpressions));
                    }
                // else: an extended type with no named connection request still legitimately carries
                // ConnectionString -- leave it as-is.
                }
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
        }
    }
}
