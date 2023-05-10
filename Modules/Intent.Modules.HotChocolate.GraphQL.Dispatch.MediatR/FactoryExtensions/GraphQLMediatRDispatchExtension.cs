using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.FactoryExtensions;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationResolver;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
using Intent.Modules.Modelers.Services.GraphQL.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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
            var queryTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(QueryResolverTemplate.TemplateId));
            var mutationTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(MutationResolverTemplate.TemplateId));
            foreach (var template in queryTypeTemplates.Concat(mutationTypeTemplates))
            {
                template.AddTypeSource(TemplateFulfillingRoles.Application.Query);
                template.AddTypeSource(TemplateFulfillingRoles.Application.Command);
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
                            method.AddStatement($"return await mediator.Send({queryRef}, cancellationToken);");
                        }
                    }
                }, 200);
            }

            var dtoTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Contracts.Dto));
            foreach (var template in dtoTemplates)
            {
                template.AddTypeSource(TemplateFulfillingRoles.Application.Query);
                template.AddTypeSource(TemplateFulfillingRoles.Application.Command);
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
            var dtoModel = template is ITemplateWithModel templateWithModel
                ? templateWithModel.Model as DTOModel
                : null;
            var mappedQuery = model.MappedElement;
            var queryType = template.GetTypeName(mappedQuery.AsTypeReference());
            if (mappedQuery.ChildElements.All(x => model.Parameters.Any(p => p.Mapping.ElementId == x.Id)))
            {
                if (model.Parameters.Any())
                {
                    return $"new {queryType} {{ {string.Join(", ", model.Parameters.Select(x => string.Join(".", x.Mapping.Path.Select(p => p.Name.ToPascalCase())) + " = " + GetPropertyAssignmentValue(template, model, x)))} }}";
                }
                else
                {
                    return $"new {queryType}()";
                }
            }
            else
            {
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