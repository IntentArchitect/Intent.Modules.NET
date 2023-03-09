using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDb.Repositories.Templates.PagedList;
using Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Repositories.FactoryExtensions
{
	[IntentManaged(Mode.Fully, Body = Mode.Merge)]
	public class PagedListFactoryExtension : FactoryExtensionBase
	{
		public override string Id => "Intent.CosmosDb.Repositories.PagedListFactoryExtension";

		[IntentManaged(Mode.Ignore)]
		public override int Order => 0;

		protected override void OnAfterTemplateRegistrations(IApplication application)
		{
			var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Repository.PagedList)).ToArray();
			if (!templates.Any())
			{
				return;
			}
			var template = templates.Single(); // Registration should guarantee one template instance
			template.CSharpFile.AfterBuild(file =>
			{
				file.AddUsing("System.Linq");
				file.AddUsing("System.Threading.Tasks");
				file.AddUsing("Microsoft.Azure.Cosmos.Linq");
				var @class = file.Classes.First();
				var T = @class.GenericParameters.First();
				var pagedResultInterfaceName = template.GetTypeName(PagedResultInterfaceTemplate.TemplateId);
				@class.AddMethod($"async Task<{pagedResultInterfaceName}<{T}>>", "CreateAsync", method =>
				{
					method.Static();
					method.AddParameter($"IOrderedQueryable<{T}>", "source");
					method.AddParameter("int", "pageNo");
					method.AddParameter("int", "pageSize");
					method.AddParameter("CancellationToken", "cancellationToken", parm => parm.WithDefaultValue("default"));
					method.AddStatement("var count = await source.CountAsync(cancellationToken);");
					method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
					method.AddStatement($"var iterator = source.ToFeedIterator();");
					method.AddStatement($"while (skip > 0)");
					method.AddStatement("{");
					method.AddStatement("    if (!iterator.HasMoreResults)");
					method.AddStatement("    {");
					method.AddStatement($"        return new {@class.Name}<{T}>(count, pageNo, pageSize, Enumerable.Empty<{T}>());");
					method.AddStatement("    }");
					method.AddStatement($"    var feedResponse = await iterator.ReadNextAsync(cancellationToken);");
					method.AddStatement($"    skip -= feedResponse.Count;");
					method.AddStatement("}");
					method.AddStatement("var results = new List<T>();");
					method.AddStatement($"while (results.Count() < pageSize)");
					method.AddStatement("{");
					method.AddStatement("    if (!iterator.HasMoreResults)");
					method.AddStatement("    {");
					method.AddStatement($"        break;");
					method.AddStatement("    }");
					method.AddStatement($"    var feedResponse = await iterator.ReadNextAsync(cancellationToken);");
					method.AddStatement($"    results.AddRange(feedResponse);");
					method.AddStatement("}");
					method.AddStatement($"return new {@class.Name}<{T}>(count, pageNo, pageSize, results);");
				});
			});
		}
	}
}
