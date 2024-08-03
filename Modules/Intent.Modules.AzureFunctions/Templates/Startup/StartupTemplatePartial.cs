using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.Startup
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class StartupTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AzureFunctions.Startup";

        private readonly IList<ServiceConfigurationRequest> _serviceConfigurations =
            new List<ServiceConfigurationRequest>();


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StartupTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ServiceConfigurationRequest>(HandleServiceConfigurationRequest);
            AddNugetDependency(NugetPackages.MicrosoftAzureFunctionsExtensions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Azure.Functions.Extensions.DependencyInjection")
                .AddClass($"Startup", @class =>
                {
                    @class.WithBaseType("FunctionsStartup");
                    @class.AddMethod("void", "Configure", method =>
                    {
                        method.Override();
                        method.AddParameter("IFunctionsHostBuilder", "builder");
                        method.AddStatement("var configuration = builder.GetContext().Configuration;");
                    });
                })
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    file.AddAssemblyAttribute($"FunctionsStartup(typeof({this.GetNamespace()}.{@class.Name}))");
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("Configure");
                    method.AddStatements(GetServiceConfigurationStatementList());

                    foreach (var request in GetRelevantServiceConfigurationRequests())
                    {
                        foreach (var templateDependency in request.TemplateDependencies)
                        {
                            var template = GetTemplate<IClassProvider>(templateDependency);
                            if (template != null)
                            {
                                AddUsing(template.Namespace);
                            }

                            AddTemplateDependency(templateDependency);
                        }

                        foreach (var @namespace in request.RequiredNamespaces)
                        {
                            AddUsing(@namespace);
                        }
                    }
                });
        }

        private void HandleServiceConfigurationRequest(ServiceConfigurationRequest request)
        {
            _serviceConfigurations.Add(request);
        }

        private IEnumerable<ServiceConfigurationRequest> GetRelevantServiceConfigurationRequests()
        {
            return _serviceConfigurations
                .Where(p => !p.IsHandled)
                .OrderBy(o => o.Priority)
                .ToArray();
        }

        private List<CSharpStatement> GetServiceConfigurationStatementList()
        {
            var statementList = new List<CSharpStatement>();

            statementList.AddRange(GetRelevantServiceConfigurationRequests()
                .Select(s =>
                {
                    foreach (var dependency in s.TemplateDependencies)
                    {
                        var classProvider = GetTemplate<IClassProvider>(dependency);

                        AddTemplateDependency(dependency);
                        AddUsing(classProvider.Namespace);
                    }
                    return new CSharpStatement($"builder.Services.{s.ExtensionMethodName}({GetExtensionMethodParameterList(s)});");
                }));

            return statementList;
        }

        private string GetExtensionMethodParameterList(ServiceConfigurationRequest request)
        {
            if (request.ExtensionMethodParameterList?.Any() != true)
            {
                return string.Empty;
            }

            var paramList = new List<string>();

            foreach (var param in request.ExtensionMethodParameterList)
            {
                switch (param)
                {
                    case ServiceConfigurationRequest.ParameterType.Configuration:
                        paramList.Add("configuration");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            paramName: nameof(request.ExtensionMethodParameterList),
                            actualValue: param,
                            message: "Type specified in parameter list is not known or supported");
                }
            }

            return string.Join(", ", paramList);
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