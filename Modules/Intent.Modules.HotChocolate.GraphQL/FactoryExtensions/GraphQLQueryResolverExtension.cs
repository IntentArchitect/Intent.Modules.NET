using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modules.HotChocolate.GraphQL.Models;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GraphQLQueryResolverExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.HotChocolate.GraphQL.GraphQLDtoResolverExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var queryTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(QueryTypeTemplate.TemplateId));
            foreach (var template in queryTypeTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IGraphQLResolverModel>("model", out var model) &&
                            model.MappedElement?.IsClassModel() == true)
                        {
                            if (model.MappedElement?.IsClassModel() == true &&
                                template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, model.MappedElement.Id, out var repositoryInterface))
                            {
                                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                method.AddParameter(repositoryInterface, "repository", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));

                                if (model.Parameters.Count() == 1 &&
                                    model.Parameters.Single().Name.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    method.AddStatement($"var entity = await repository.FindByIdAsync({model.Parameters.First().Name.ToCamelCase()}, cancellationToken);");
                                    if (model.TypeReference.Element.IsClassModel())
                                    {
                                        method.AddStatement("return entity;");
                                    }
                                    else if (ReturnsMappedDto(model, template))
                                    {
                                        method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                        method.AddStatement($"return entity.MapTo{model.TypeReference.Element.Name.ToPascalCase()}(mapper);");
                                    }
                                }
                                else if (model.TypeReference.IsCollection)
                                {
                                    method.AddStatement($"var entities = await repository.FindAllAsync(cancellationToken);");
                                    if (model.TypeReference.Element.IsClassModel())
                                    {
                                        method.AddStatement("return entities;");
                                    }
                                    else if (ReturnsMappedDto(model, template))
                                    {
                                        method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));
                                        method.AddStatement($"return entities.MapTo{model.TypeReference.Element.Name.ToPascalCase()}List(mapper);");
                                    }
                                }
                            }
                        }
                    }
                }, 200);
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