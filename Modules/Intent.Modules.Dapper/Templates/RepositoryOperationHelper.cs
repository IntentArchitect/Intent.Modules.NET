using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Dapper.Templates;

public static class RepositoryOperationHelper
{
	public static void ApplyMethods(ICSharpFileBuilderTemplate? template, CSharpClass? @class, RepositoryModel repositoryModel)
	{
		if (template is null || @class is null)
		{
			return;
		}

		foreach (var operationModel in repositoryModel.Operations)
		{
			var isAsync = operationModel.Name.EndsWith("Async");
			@class.AddMethod(GetReturnType(template, operationModel.ReturnType), operationModel.Name, method =>
			{
				method.AddMetadata("model", operationModel);
				method.RepresentsModel(operationModel);
				if (isAsync)
				{
					method.Async();
				}

				method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

				foreach (var parameterModel in operationModel.Parameters)
				{
					method.AddParameter(template.GetTypeName(parameterModel.TypeReference), parameterModel.Name);
				}

				if (isAsync)
				{
					method.AddOptionalCancellationTokenParameter();
				}
			});
		}
	}

	public static void ApplyMethods(ICSharpFileBuilderTemplate? template, CSharpInterface? @interface, RepositoryModel repositoryModel)
	{
		if (template is null || @interface is null)
		{
			return;
		}

		foreach (var operationModel in repositoryModel.Operations)
		{
			var isAsync = operationModel.Name.EndsWith("Async");
			@interface.AddMethod(GetReturnType(template, operationModel.ReturnType), operationModel.Name, method =>
			{
				method.AddMetadata("model", operationModel);
				method.RepresentsModel(operationModel);
				if (isAsync)
				{
					method.Async();
				}

				foreach (var parameterModel in operationModel.Parameters)
				{
					method.AddParameter(template.GetTypeName(parameterModel.TypeReference), parameterModel.Name);
				}

				if (isAsync)
				{
					method.AddOptionalCancellationTokenParameter();
				}
			});
		}
	}

	private static string GetReturnType(ICSharpFileBuilderTemplate template, ITypeReference? returnType)
	{
		if (returnType is null)
		{
			return "void";
		}

		return template.GetTypeName(returnType, "System.Collections.Generic.List<{0}>");
	}
}