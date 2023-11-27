using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.FactoryExtensions;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.Services.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GraphQLServiceDispatchExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.HotChocolate.GraphQL.Dispatch.Services.GraphQLServiceDispatchExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var queryTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(QueryTypeTemplate.TemplateId));
            var mutationTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(MutationTypeTemplate.TemplateId));
            foreach (var template in queryTypeTemplates.Concat(mutationTypeTemplates))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IGraphQLResolverModel>("model", out var model) &&
                            model.MappedElement?.IsOperationModel() == true)
                        {
                            if (template.Id == MutationTypeTemplate.TemplateId && model.Parameters.Count() > 1)
                            {
                                method.AddAttribute($"[{template.UseType("UseMutationConvention")}]");
                            }
                            var serviceType = template.GetTypeName(TemplateRoles.Application.Services.Interface, model.MappedElement.ParentElement);
                            method.AddParameter(serviceType, "service", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                            method.AddStatement($"{(method.ReturnType == "Task" ? "" : "return")} await service.{model.MappedElement.Name.ToPascalCase()}({string.Join(", ", model.Parameters.Select(x => GetServiceParameterExpression(x)))});");
                        }
                    }
                }, 200);
            }

            var dtoTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Contracts.Dto));
            foreach (var template in dtoTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IGraphQLResolverModel>("model", out var model) &&
                            model.MappedElement?.IsOperationModel() == true)
                        {
                            var serviceType = template.GetTypeName(TemplateRoles.Application.Services.Interface, ((IElement)model.MappedElement).ParentElement);
                            method.AddParameter(serviceType, "service", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                            method.AddStatement($"return await service.{model.MappedElement.Name.ToPascalCase()}({string.Join(", ", model.Parameters.Select(x => GetPropertyAssignmentValue(template, model, x)))});");
                        }
                    }
                }, 200);
            }
        }

        private string GetServiceParameterExpression(IGraphQLParameterModel x)
        {
            if (x.TypeReference.IsCollection)
            {
                return $"{x.Name.ToParameterName()}.ToList()";
            }
            return x.Name.ToParameterName();
        }

        private static string GetPropertyAssignmentValue(ICSharpFileBuilderTemplate template, IGraphQLResolverModel model, IGraphQLParameterModel parameter)
        {
            var dtoModel = template is ITemplateWithModel templateWithModel
                ? templateWithModel.Model as DTOModel
                : null;
            return dtoModel != null && model.GetMatchingInDtoParameters(dtoModel).Any(p => p.Equals(parameter))
                ? parameter.Name.ToPascalCase()
                : parameter.Name.ToParameterName();
        }
    }
}