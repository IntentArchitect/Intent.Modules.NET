using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDb.Templates.CosmosDbUnitOfWorkInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Templates.ApplicationCosmosDbContext
{
	[IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
	public partial class ApplicationCosmosDbContextTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
	{
		public const string TemplateId = "Intent.CosmosDb.ApplicationCosmosDbContext";

		[IntentManaged(Mode.Fully, Body = Mode.Ignore)]
		public ApplicationCosmosDbContextTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
		{
			AddNugetDependency(NugetPackages.MicrosoftAzureCosmos);

			CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
				.AddUsing("System.Threading")
				.AddUsing("System.Threading.Tasks")
				.AddUsing("Microsoft.Azure.Cosmos")
				.AddClass($"ApplicationCosmosDbContext", @class =>
				{
					@class.WithBaseType("CosmosDbContext");
					@class.ImplementsInterface(GetTypeName(CosmosDbUnitOfWorkInterfaceTemplate.TemplateId));
					@class.AddConstructor(ctor =>
					{
						ctor.Static();
						ctor.AddStatement($"ApplyConfigurationsFromAssembly(typeof({@class.Name}).Assembly);");
					});
					@class.AddConstructor(ctor =>
					{
						ctor.AddParameter("CosmosClient", "cosmosClient");
						ctor.AddParameter("string", "databaseName");
						ctor.AddParameter("CosmosDbContextOptions", "options", param => param.WithDefaultValue("null"));
						ctor.CallsBase(b =>
						{
							b.AddArgument("cosmosClient");
							b.AddArgument("databaseName");
							b.AddArgument("options");
						});
						ctor.AddStatement("AcceptAllChangesOnSave = true;");
						ctor.AddStatement("AddCommand(() => null);");
					});
					@class.AddMethod("Task<int>", $"SaveChangesAsync", method =>
					{
						method.Async();
						method.AddParameter("CancellationToken", "cancellationToken");
						method.AddStatement("return (await base.SaveChangesAsync(cancellationToken)).Results.Count;");
					});
				});
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
