using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDb.Templates.ApplicationCosmosDbContext;
using Intent.Modules.CosmosDb.Templates.CosmosDbUnitOfWorkInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDb.FactoryExtensions
{
	[IntentManaged(Mode.Fully)]
	public class DependencyInjectionFactoryExtension : FactoryExtensionBase
	{
		public override string Id => "Intent.CosmosDb.DependencyInjectionFactoryExtension";

		public override int Order => 0;

		protected override void OnAfterTemplateRegistrations(IApplication application)
		{
			var dbContext = application.FindTemplateInstance<ICSharpTemplate>(TemplateDependency.OnTemplate(ApplicationCosmosDbContextTemplate.TemplateId));
			if (dbContext == null)
			{
				return;
			}

			var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
			if (dependencyInjection == null)
			{
				return;
			}

			dependencyInjection.CSharpFile.OnBuild(file =>
			{
				file.Classes.First().FindMethod("AddInfrastructure")
					.AddStatement(new CSharpInvocationStatement($"services.AddScoped<{dependencyInjection.GetTypeName(dbContext.Id)}>")
							.AddArgument(new CSharpLambdaBlock("provider")
								.AddStatement($@"var connectionString = configuration.GetConnectionString(""CosmosDbConnection"");")
								.AddStatement($@"var endpointUri = new Uri(connectionString);")
								.AddStatement($@"var cosmosClient = new CosmosClient(endpointUri, configuration[""CosmosDb:AuthKey""]);")
								.AddStatement($@"return new {dependencyInjection.GetTypeName(dbContext.Id)}(cosmosClient.GetDatabase(configuration[""CosmosDb:DatabaseName""]));"))
							.WithArgumentsOnNewLines(),
						stmt => stmt.AddMetadata("application-cosmosdb-context", true));
			});

			application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest("CosmosDbConnection", $"https://localhost:8081", string.Empty));

			application.EventDispatcher.Publish(ServiceConfigurationRequest
				.ToRegister("AddCosmosDbUnitOfWork")
				.ForConcern("Infrastructure")
				.RequiresUsingNamespaces("Intent.Modules.CosmosDb.Common.CosmosDb", "Intent.Modules.CosmosDb.UnitOfWork.Abstractions.Extensions"));
			application.EventDispatcher.Publish(ServiceConfigurationRequest
				.ToRegister($"AddCosmosDbUnitOfWork<{dbContext.ClassName}>")
				.ForConcern("Infrastructure")
				.RequiresUsingNamespaces("Intent.Modules.CosmosDb.Common.CosmosDb", "Intent.Modules.CosmosDb.UnitOfWork.Abstractions.Extensions"));
			application.EventDispatcher.Publish(ContainerRegistrationRequest
				.ToRegister(dbContext)
				.ForInterface(dbContext.GetTemplate<IClassProvider>(CosmosDbUnitOfWorkInterfaceTemplate.TemplateId))
				.ForConcern("Infrastructure")
				.WithResolveFromContainer());
			application.EventDispatcher.Publish(ContainerRegistrationRequest
				.ToRegister(dbContext)
				.ForInterface("ICosmosDbContext")
				.RequiresUsingNamespaces("Intent.Modules.CosmosDb.Common.CosmosDb")
				.ForConcern("Infrastructure")
				.WithResolveFromContainer());
		}
	}
}
