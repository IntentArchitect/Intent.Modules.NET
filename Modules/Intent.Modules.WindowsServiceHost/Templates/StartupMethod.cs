using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.WindowsServiceHost.Templates;

internal class StartupMethod : IStartupMethod
{
	private readonly CSharpLocalMethod _localMethod;
	private readonly CSharpClassMethod _classMethod;

	public static void CreateOn(
		CSharpTemplateBase<object> template,
		string returnType,
		string name,
		Action<IStartupMethod> configure,
		int? priority)
	{
		if (template is not ICSharpFileBuilderTemplate fileBuilderTemplate)
		{
			throw new Exception($"template must implement {nameof(ICSharpFileBuilderTemplate)}");
		}

		var netSettings = template.OutputTarget.GetProject()?.InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings();
		var usesTopLevelStatements = netSettings?.UseTopLevelStatements() == true;


		if (usesTopLevelStatements)
		{
			fileBuilderTemplate.CSharpFile.TopLevelStatements.AddLocalMethod(returnType, name, method =>
			{
				configure?.Invoke(new StartupMethod(localMethod: method));
			});
		}
		else
		{
			fileBuilderTemplate.CSharpFile.Classes.First().AddMethod(returnType: returnType, name: name, method =>
			{
				configure?.Invoke(new StartupMethod(classMethod: method));
			});
		}
	}

	private StartupMethod(CSharpClassMethod classMethod)
	{
		_classMethod = classMethod ?? throw new ArgumentNullException(nameof(classMethod));
	}

	private StartupMethod(CSharpLocalMethod localMethod)
	{
		_localMethod = localMethod ?? throw new ArgumentNullException(nameof(localMethod));
	}

	public IList<CSharpStatement> Statements => _classMethod?.Statements ?? _localMethod.Statements;

	public IStartupMethod Static()
	{
		_classMethod?.Static();
		_localMethod?.Static();
		return this;
	}

	public IStartupMethod Async()
	{
		_classMethod?.Async();
		_localMethod?.Async();
		return this;
	}

	public IStartupMethod AddParameter(string type, string name, Action<CSharpParameter> configure = null)
	{
		_classMethod?.AddParameter(type, name, configure);
		_localMethod?.AddParameter(type, name, configure);
		return this;
	}

	public IStartupMethod AddGenericParameter(string typeName)
	{
		_classMethod?.AddGenericParameter(typeName);
		_localMethod?.AddGenericParameter(typeName);
		return this;
	}

	public IStartupMethod AddGenericParameter(string typeName, out CSharpGenericParameter param)
	{
		param = default;
		_classMethod?.AddGenericParameter(typeName, out param);
		_localMethod?.AddGenericParameter(typeName, out param);
		return this;
	}

	public IStartupMethod AddGenericTypeConstraint(string genericParameterName, Action<CSharpGenericTypeConstraint> configure)
	{
		_classMethod?.AddGenericTypeConstraint(genericParameterName, configure);
		_localMethod?.AddGenericTypeConstraint(genericParameterName, configure);
		return this;
	}
}