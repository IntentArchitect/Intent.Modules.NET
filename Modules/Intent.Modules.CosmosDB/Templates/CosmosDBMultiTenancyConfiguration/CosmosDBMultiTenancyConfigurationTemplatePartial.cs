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

namespace Intent.Modules.CosmosDB.Templates.CosmosDBMultiTenancyConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBMultiTenancyConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBMultiTenancyConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBMultiTenancyConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.Extensions.Options")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Azure.CosmosRepository.Options")
                .AddUsing("Microsoft.Azure.CosmosRepository.Providers")
                .AddClass($"CosmosDBMultiTenancyConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureCosmosSeperateDBMultiTenancy", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IServiceCollection", "services", param =>
                            {
                                param.WithThisModifier();
                            })
                            .AddParameter("IConfiguration", "configuration");
                        method.AddStatements(@$"services.AddSingleton<ICosmosClientProvider, {this.GetCosmosDBMultiTenantClientProviderName()}>();
			services.AddSingleton(typeof(ICosmosContainerProvider<>), typeof({this.GetCosmosDBMultitenantContainerProviderName()}<>));


			var basePostConfigureActions = services
				.Where(descriptor => descriptor.ServiceType == typeof(IPostConfigureOptions<RepositoryOptions>))
				.Select(descriptor => (IPostConfigureOptions<RepositoryOptions>)descriptor.ImplementationInstance)
				.ToList();

			services.AddSingleton<RepositoryOptions>(sp => new {this.GetCosmosDBMultiTenantRepositoryOptionsName()}(sp.GetRequiredService<ICosmosClientProvider>()));
			services.AddSingleton<IOptions<RepositoryOptions>>(sp =>
			{{
				var resultOptions = sp.GetRequiredService<RepositoryOptions>();
				foreach (var action in basePostConfigureActions)
				{{
					action.PostConfigure(Options.DefaultName, resultOptions);
				}}
				return Options.Create(resultOptions);
			}});
			services.AddSingleton<IOptionsMonitor<RepositoryOptions>>(sp => new {this.GetCosmosDBMultiTenantOptionsMonitorName()}(sp.GetRequiredService<RepositoryOptions>()));
			return services;".ConvertToStatements());
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