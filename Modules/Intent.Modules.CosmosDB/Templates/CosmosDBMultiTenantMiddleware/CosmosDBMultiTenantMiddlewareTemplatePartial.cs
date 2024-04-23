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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Finbuckle.MultiTenant")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultiTenantMiddleware", @class =>
                {
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

                    @class.AddMethod("Task", "Invoke", method =>
                    {
                        method
                            .Async()
                            .AddParameter("HttpContext", "context");
                        method.AddStatements(@"using (var scope = _serviceProvider.CreateScope())
			{
				var tenant = scope.ServiceProvider.GetService<TenantInfo>();
				var cosmosClientOptionsProvider = scope.ServiceProvider.GetRequiredService<ICosmosClientOptionsProvider>();

				using (_clientProvider.SetLocalState(tenant, cosmosClientOptionsProvider))
				{
					await _next(context);
				}

			}".ConvertToStatements());
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