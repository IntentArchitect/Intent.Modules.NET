using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Intent.AspNetCore.ODataQuery.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AspNetCore.ODataQuery.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class QueryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.ODataQuery.QueryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var queryModels = application.MetadataManager.Services(application).GetQueryModels()
                .Where(qm => qm.HasODataQuery())
                .ToArray();

            foreach (var queryModel in queryModels)
            {
                var queryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Query, queryModel.Id);
                if (!queryTemplate.ExecutionContext.Settings.GetODataQuerySettings().AllowSelectOption() && queryModel.GetODataQuery().EnableSelect())
                {
                    Logging.Log.Warning($"`Select enabled` for OData Query : {queryModel.Name} but `Select` is disabled at an API level.");
                    return;
                }
                queryTemplate.AddNugetDependency(NugetPackages.MicrosoftAspNetCoreOData(queryTemplate.OutputTarget));

                UpdateQuery(application, queryTemplate, queryModel, queryModel.GetODataQuery().EnableSelect());
                UpdateHandler(application, queryModel, queryModel.GetODataQuery().EnableSelect());
                UpdateController(application, queryModel, queryModel.GetODataQuery().EnableSelect());

            }
        }

        private void UpdateController(IApplication application, QueryModel queryModel, bool enableSelect)
        {

            var controllerTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.AspNetCore.Controllers.Controller", queryModel.InternalElement.ParentElement.Id);
            if (controllerTemplate == null) return;
            controllerTemplate?.CSharpFile.OnBuild(file =>
            {
                controllerTemplate.AddUsing("Microsoft.AspNetCore.OData.Query");
                var @class = file.Classes.First();
                var method = @class.FindMethod(m => m.HasMetadata("modelId") && m.GetMetadata<string>("modelId") == queryModel.Id);

                if (enableSelect)
                {
                    method.WithReturnType("Task<IActionResult>");
                }
                var dtoModel = queryModel.TypeReference.Element.AsDTOModel() ?? throw new Exception($"Expected DTO return type OData Query. {queryModel.Name} found {controllerTemplate.GetTypeName(queryModel.TypeReference)}");

                var odataParam = $"ODataQueryOptions<{controllerTemplate.GetTypeName(dtoModel.InternalElement)}>";
                if (method.Parameters.LastOrDefault()?.Type == "CancellationToken")
                {
                    method.InsertParameter(method.Parameters.Count - 1, odataParam, "oDataOptions");
                }
                else
                {
                    method.AddParameter(odataParam, "oDataOptions");
                }
                method.InsertStatement(0, $"ValidateODataOptions(oDataOptions{(enableSelect ? ", true" : "")});");
                var dispatchStatement = method.FindStatement(stmt => stmt.ToString().Contains("_mediator.Send"));
                dispatchStatement.Replace(new CSharpStatement(dispatchStatement.ToString().Replace("), cancellationToken", $"{(queryModel.Properties.Count > 0 ? ", " : "")}oDataOptions.ApplyTo), cancellationToken")));

                if (@class.FindMethod("ValidateODataOptions") == null)
                {
                    @class.AddMethod("void", "ValidateODataOptions", method =>
                    {
                        method
                            .Private()
                            .AddGenericParameter("TDto", out var genericArg)
                            .AddParameter($"ODataQueryOptions<{genericArg}>", "options")
                            .AddParameter("bool", "enableSelect", p => p.WithDefaultValue("false"));
                        method.AddStatement($"var settings = new {controllerTemplate.UseType("Microsoft.AspNetCore.OData.Query.Validator.ODataValidationSettings")}();");
                        method.AddIfStatement("!enableSelect", stmt =>
                        {
                            stmt.AddStatement("settings.AllowedQueryOptions = AllowedQueryOptions.All & ~AllowedQueryOptions.Select;");
                        });
                        method.AddStatement("options.Validate(settings);");
                    });
                }

            }, 100);
        }

        private void UpdateQuery(IApplication application, ICSharpFileBuilderTemplate template, QueryModel queryModel, bool enableSelect)
        {
            template?.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                template.AddUsing("System");
                template.AddUsing("System.Linq");
                if (enableSelect)
                {
                    var requestInterface = @class.Interfaces.FirstOrDefault(x => x.StartsWith("IRequest"));
                    if (requestInterface == null) return;
                    int index = @class.Interfaces.IndexOf(requestInterface);
                    file.AddUsing("System.Collections");
                    @class.Interfaces[index] = "IRequest<IEnumerable>";
                }
                var constructor = @class.Constructors.First();
                var dtoModel = queryModel.TypeReference.Element.AsDTOModel() ?? throw new Exception($"Expected DTO return type OData Query. {queryModel.Name} found {template.GetTypeName(queryModel.TypeReference)}");
                constructor.AddParameter($"Func<IQueryable<{template.GetTypeName(dtoModel.InternalElement)}>, IQueryable>", "transform", param => param.IntroduceProperty(prop => prop.ReadOnly()));
            }, 100);
        }

        private void UpdateHandler(IApplication application, QueryModel queryModel, bool enableSelect)
        {
            if (enableSelect)
            {
                var handlerTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.MediatR.QueryHandler", queryModel.Id);
                handlerTemplate?.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First(x => x.HasMetadata("handler"));
                    var requestInterface = @class.Interfaces.FirstOrDefault(x => x.StartsWith("IRequestHandler"));
                    if (requestInterface == null) return;
                    int index = @class.Interfaces.IndexOf(requestInterface);

                    @class.Interfaces[index] = @class.Interfaces[index].Substring(0, @class.Interfaces[index].LastIndexOf(',')) + ", IEnumerable>";

                    var method = @class.FindMethod("Handle");
                    if (method == null) return;
                    handlerTemplate.AddUsing("System.Collections");
                    method.WithReturnType("Task<IEnumerable>");
                }, 100);
            }
        }

    }
}