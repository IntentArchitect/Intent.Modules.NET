using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.AspNetCore.ODataQuery.Api;
using Intent.Modules.AspNetCore.ODataQuery.Settings;
using Intent.Utils;
using System;
using static Intent.Modules.Constants.TemplateFulfillingRoles.Repository;
using System.Reflection;
using Intent.Templates;

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
                var queryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Application.Query, queryModel.Id);
                if (!queryTemplate.ExecutionContext.Settings.GetODataQuerySettings().AllowSelectOption() && queryModel.GetODataQuery().EnableSelect())
                {
                    Logging.Log.Warning($"`Select enabled` for OData Query : {queryModel.Name} but `Select` is disabled at an API level.");
                    return;
                }

                UpdateQuery(application, queryTemplate, queryModel, queryModel.GetODataQuery().EnableSelect());
                UpdateHandler(application, queryModel, queryModel.GetODataQuery().EnableSelect());

                
            }
        }

        private void UpdateQuery(IApplication application, ICSharpFileBuilderTemplate template, QueryModel queryMdoel, bool enableSelect)
        {
            template?.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                if (enableSelect)
                {
                    var requestInterface = @class.Interfaces.FirstOrDefault(x => x.StartsWith("IRequest"));
                    if (requestInterface == null) return;
                    int index = @class.Interfaces.IndexOf(requestInterface);
                    file.AddUsing("System.Collections");
                    @class.Interfaces[index] = "IRequest<IEnumerable>";
                }
                var constructor = @class.Constructors.First();
                var dtoModel = queryMdoel.TypeReference.Element.AsDTOModel() ?? throw new Exception($"Expected DTO return type OData Query. {queryMdoel.Name} found {template.GetTypeName(queryMdoel.TypeReference)}");                
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
                    var @class = file.Classes.First();
                    var requestInterface = @class.Interfaces.FirstOrDefault(x => x.StartsWith("IRequestHandler"));
                    if (requestInterface == null) return;
                    int index = @class.Interfaces.IndexOf(requestInterface);

                    @class.Interfaces[index] = @class.Interfaces[index].Substring(0, @class.Interfaces[index].LastIndexOf(',')) + ", IEnumerable>";

                    var method = @class.FindMethod("Handle");
                    if (method == null) return;
                    handlerTemplate.AddUsing("System.Collections");
                    method.WithReturnType("IEnumerable");
                }, 100);
            }
        }

    }
}