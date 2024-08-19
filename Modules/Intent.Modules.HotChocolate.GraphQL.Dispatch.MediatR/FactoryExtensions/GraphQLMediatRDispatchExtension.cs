using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.FactoryExtensions;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modules.HotChocolate.GraphQL.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GraphQLMediatRDispatchExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.HotChocolate.GraphQL.Dispatch.MediatR.GraphQLMediatRDispatchExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var queryTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(QueryTypeTemplate.TemplateId));
            var mutationTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(MutationTypeTemplate.TemplateId));
            foreach (var template in queryTypeTemplates.Concat(mutationTypeTemplates))
            {
                template.AddTypeSource(TemplateRoles.Application.Query);
                template.AddTypeSource(TemplateRoles.Application.Command);
                template.AddNugetDependency(NugetPackages.HotChocolate(template.OutputTarget));
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IGraphQLResolverModel>("model", out var model) && (model.MappedElement?.IsQueryModel() == true || model.MappedElement?.IsCommandModel() == true))
                        {
                            var queryRef = EnsureAndGetQueryObject(template, method, model);
                            method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                            method.AddParameter(template.UseType("MediatR.ISender"), "mediator", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                            method.AddStatement($"{(method.ReturnType == "Task" ? "" : "return")} await mediator.Send({queryRef}, cancellationToken);");
                        }
                    }
                }, 200);
            }

            var dtoTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Contracts.Dto));
            foreach (var template in dtoTemplates)
            {
                template.AddTypeSource(TemplateRoles.Application.Query);
                template.AddTypeSource(TemplateRoles.Application.Command);
                template.AddNugetDependency(NugetPackages.HotChocolate(template.OutputTarget));
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IGraphQLResolverModel>("model", out var model) &&
                            (model.MappedElement?.IsQueryModel() == true || model.MappedElement?.IsCommandModel() == true))
                        {
                            var queryRef = EnsureAndGetQueryObject(template, method, model);
                            method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                            method.AddParameter(template.UseType("MediatR.ISender"), "mediator", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                            method.AddStatement($"return await mediator.Send({queryRef}, cancellationToken);");
                        }
                    }
                }, 200);
            }
        }

        private string EnsureAndGetQueryObject(ICSharpFileBuilderTemplate template, CSharpClassMethod method, IGraphQLResolverModel model)
        {
            var mappedQuery = model.MappedElement;
            var queryType = template.GetTypeName(mappedQuery.AsTypeReference());
            if (mappedQuery.ChildElements.All(x => model.Parameters.Any(p => p.MappedElement?.Id == x.Id)))
            {
                if (model.Parameters.Any())
                {
                    if (mappedQuery.IsCommandModel() || mappedQuery.IsQueryModel())
                    {
                        return $"new {queryType} ( {string.Join(", ", model.Parameters.Select(x => string.Join(".", x.MappedPath).ToParameterName() + " : " + GetPropertyAssignmentValue(template, model, x)))} )";
                    }
                    return $"new {queryType} {{ {string.Join(", ", model.Parameters.Select(x => string.Join(".", x.MappedPath) + " = " + GetPropertyAssignmentValue(template, model, x)))} }}";
                }
                else
                {
                    return $"new {queryType}()";
                }
            }
            else
            {
                if (mappedQuery.AsQueryModel()?.Properties.Count == 0 || mappedQuery.AsCommandModel()?.Properties.Count == 0)
                {
                    Logging.Log.Warning($"GraphQL operation {model.Name} has an empty complex type parameter [input: {model.Name}]. This may cause errors.");
                }
                method.AddParameter(queryType, "input");
                return "input";
            }
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