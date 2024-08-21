using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modules.HotChocolate.GraphQL.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

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
            var dtoTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Contracts.Dto));
            foreach (var template in dtoTemplates)
            {
                var dtoModel = template is ITemplateWithModel templateWithModel
                    ? templateWithModel.Model as DTOModel
                    : null;
                if (dtoModel != null)
                {
                    var extensionModel = new DTOExtensionModel(dtoModel.InternalElement);
                    template.CSharpFile.OnBuild(file =>
                    {
                        var @class = file.Classes.First();
                        foreach (var resolver in extensionModel.Resolvers.Select(x => new GraphQLResolverModel(x)).ToList<IGraphQLResolverModel>())
                        {
                            template.AddNugetDependency(NugetPackages.HotChocolate(template.OutputTarget));

                            @class.AddMethod($"{template.UseType("System.Threading.Tasks.Task")}<{template.GetTypeName(resolver.TypeReference)}>", resolver.Name.ToPascalCase(), method =>
                            {
                                method.AddMetadata("model", resolver);
                                if (!string.IsNullOrWhiteSpace(resolver.Description))
                                {
                                    @method.AddAttribute("GraphQLDescription", attr => attr.AddArgument($@"""{resolver.Description}"""));
                                }
                                method.Async();
                                foreach (var parameter in resolver.Parameters.Where(x => !resolver.GetMatchingInDtoParameters(dtoModel).Contains(x)))
                                {
                                    method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.Name.ToCamelCase(), param =>
                                    {
                                        if (!string.IsNullOrWhiteSpace(parameter.Description))
                                        {
                                            param.AddAttribute("GraphQLDescription", attr => attr.AddArgument($@"""{parameter.Description}"""));
                                        }
                                    });
                                }

                                if (resolver.MappedElement?.IsClassModel() == true &&
                                    template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, resolver.MappedElement.Id, out var repositoryInterface))
                                {
                                    method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                    method.AddParameter(repositoryInterface, "repository", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));

                                    if (resolver.Parameters.Count() == 1 &&
                                        !resolver.TypeReference.IsCollection &&
                                        resolver.GetMatchingInDtoParameters(dtoModel).Count() == 1)
                                    {
                                        method.AddStatement($"var entity = await repository.FindByIdAsync({resolver.Parameters.First().Name.ToPascalCase()}, cancellationToken);");
                                        if (resolver.TypeReference.Element.IsClassModel())
                                        {
                                            method.AddStatement("return entity;");
                                        }
                                        else if (ReturnsMappedDto(resolver, template))
                                        {
                                            method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                            method.AddStatement($"return entity.MapTo{resolver.TypeReference.Element.Name.ToPascalCase()}(mapper);");
                                        }
                                    }
                                    else if (resolver.TypeReference.IsCollection)
                                    {
                                        method.AddStatement($"var entities = await repository.FindAllAsync(cancellationToken);");
                                        if (resolver.TypeReference.Element.IsClassModel())
                                        {
                                            method.AddStatement("return entities;");
                                        }
                                        else if (ReturnsMappedDto(resolver, template))
                                        {
                                            method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                            method.AddStatement($"return entities.MapTo{resolver.TypeReference.Element.Name.ToPascalCase()}List(mapper);");
                                        }
                                    }
                                }
                            });
                        }
                    }, 199);
                }
            }
        }

        private static bool ReturnsMappedDto(IGraphQLResolverModel resolver, ICSharpFileBuilderTemplate template)
        {
            return resolver.TypeReference.Element.IsDTOModel() &&
                   template.TryGetTemplate<IIntentTemplate>(TemplateRoles.Application.Mappings, resolver.TypeReference.Element.Id, out var _) &&
                   resolver.TypeReference.Element.AsDTOModel()?.Mapping.Element == resolver.MappedElement;
        }
    }
}