using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
using Intent.Modules.Modelers.Services.GraphQL.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modelers.Services.CQRS.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GraphQLDtoResolverExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.HotChocolate.GraphQL.GraphQLDtoResolverExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dtoTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Contracts.Dto));
            foreach (var template in dtoTemplates)
            {
                var dtoModel = template is ITemplateWithModel templateWithModel
                    ? templateWithModel.Model as DTOModel
                    : null;
                if (dtoModel != null)
                {
                    var extensionModel = new DTOExtensionModel(dtoModel.InternalElement);
                    foreach (var resolver in extensionModel.Resolvers.Select(x => new GraphQLResolverModel(x)).ToList<IGraphQLResolverModel>())
                    {
                        template.CSharpFile.OnBuild(file =>
                        {
                            var @class = file.Classes.First();
                            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<{template.GetTypeName(resolver.TypeReference)}>", resolver.Name.ToPascalCase(), method =>
                            {
                                method.AddMetadata("model", resolver);
                                method.Async();
                                foreach (var parameter in resolver.Parameters.Where(x => !resolver.GetMatchingInDtoParameters(dtoModel).Contains(x)))
                                {
                                    method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToCamelCase());
                                }

                                if (resolver.Mapping?.Element.IsClassModel() == true && 
                                    resolver.Parameters.Count() == 1 &&
                                    resolver.GetMatchingInDtoParameters(dtoModel).Count() == 1 &&
                                    resolver.TypeReference.Element.AsDTOModel()?.Mapping.Element.IsClassModel() == true)
                                {
                                    method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                    method.AddParameter(template.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, resolver.Mapping.ElementId), "repository", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                    method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                    method.AddStatement($"var entity = await repository.FindByIdAsync({resolver.Parameters.First().Name.ToPascalCase()}, cancellationToken);");
                                    method.AddStatement($"return entity.MapTo{resolver.TypeReference.Element.Name.ToPascalCase()}(mapper);");
                                }
                                //else if (resolver.Mapping?.Element.IsClassModel() == true &&
                                //         resolver.TypeReference.Element.AsDTOModel()?.Mapping.Element.IsClassModel() == true)
                                //{
                                //    method.AddParameter(template.UseType("CancellationToken"), "cancellationToken");
                                //    method.AddParameter(template.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, resolver.Mapping.ElementId), "repository", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                //    method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                //    method.AddStatement($"var entity = await repository.FindByIdAsync({resolver.Parameters.First().Name.ToPascalCase()}, cancellationToken);");
                                //    method.AddStatement($"return entity.MapTo{resolver.TypeReference.Element.Name.ToPascalCase()}(mapper);");
                                //}
                            });
                        }, 199);
                    }
                }
            }
        }
    }

    public static class ResolverModelExtensions
    {
        public static IEnumerable<IGraphQLParameterModel> GetMatchingInDtoParameters(this IGraphQLResolverModel resolver, DTOModel dtoModel)
        {
            return resolver.Parameters.Where(x => dtoModel.Fields.Any(f => f.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase))).ToList();
        }
    }
}