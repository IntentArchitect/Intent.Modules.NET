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

namespace Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenantMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBMultiTenantMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBMultiTenantMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBMultiTenantMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpClass mainClass = null;
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Finbuckle.MultiTenant.Abstractions")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultiTenantMiddleware", @class =>
                {
                    mainClass = @class;
                    @class.AddField(this.GetCosmosDBMultiTenantClientProviderName(), "_clientProvider", f => f.PrivateReadOnly());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("RequestDelegate", "next", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("IServiceProvider", "serviceProvider", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                        ctor.AddParameter("ICosmosClientProvider", "clientProvider");
                        ctor.AddStatement($"_clientProvider = ({this.GetCosmosDBMultiTenantClientProviderName()})clientProvider;");
                    });
                })
                .AddClass("CosmosDBMultiTenantMiddlewareExtensions", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IApplicationBuilder", "UseCosmosMultiTenantMiddleware", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IApplicationBuilder", "builder", p => p.WithThisModifier());
                        method.AddStatement("return builder.UseMiddleware<CosmosDBMultiTenantMiddleware>();");
                    });
                });

            // The "Invoke" method is added inside OnBuild (deferred to the Build phase, after all templates across
            // all modules are registered) because it needs GetTypeName to resolve
            // Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo (no ProjectReference to that module - cross-
            // module lookup by TemplateId). Calling GetTypeName in the constructor throws if that module's template
            // hasn't been registered yet, since registration order across modules isn't guaranteed. This template
            // only runs for separate-database multi-tenancy (see CanRunTemplate), where that module always registers
            // AddMultiTenant<TenantExtendedInfo>() - Finbuckle only DI-registers the accessor closed over that exact
            // type, so resolving IMultiTenantContextAccessor<TenantInfo> (the base type) would fail at runtime.
            // The class itself is added eagerly above (not deferred) so that CSharpFile.GetConfig() - called before
            // OnBuild runs - sees at least one type; deferring the whole file body left CSharpFile with zero types
            // at config time and threw "Either a file must use top level statements or at least one type must be
            // specified for C# file".
            CSharpFile.OnBuild(file =>
            {
                var tenantInfoTypeName = this.GetTypeName("Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo");

                mainClass.AddMethod("Task", "Invoke", method =>
                {
                    method
                        .Async()
                        .AddParameter("HttpContext", "context");
                    method.AddStatements($@"using (var scope = _serviceProvider.CreateScope())
                        {{
                        var tenant = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor<{tenantInfoTypeName}>>().MultiTenantContext?.TenantInfo;
                        var cosmosClientOptionsProvider = scope.ServiceProvider.GetRequiredService<ICosmosClientOptionsProvider>();

                        using (_clientProvider.SetLocalState(tenant, cosmosClientOptionsProvider))
                        {{
                        await _next(context);
                        }}

                        }}".ConvertToStatements());
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
