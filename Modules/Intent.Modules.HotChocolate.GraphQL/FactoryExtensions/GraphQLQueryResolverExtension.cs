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
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationResolver;

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
            var queryTypeTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(QueryResolverTemplate.TemplateId));
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
                                template.TryGetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, model.MappedElement.Id, out var repositoryInterface))
                            {
                                method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken");
                                method.AddParameter(repositoryInterface, "repository", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));

                                if (template.TryGetTemplate<IIntentTemplate>(TemplateFulfillingRoles.Application.Mappings, model.TypeReference.Element.Id, out var _))
                                {
                                    method.AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.AddAttribute($"[{template.UseType("HotChocolate.Service")}]"));

                                    if (model.Parameters.Count() == 1 &&
                                        model.Parameters.Single().Name.Equals("id", StringComparison.InvariantCultureIgnoreCase) &&
                                        model.TypeReference.Element.AsDTOModel()?.Mapping.Element.IsClassModel() == true)
                                    {
                                        method.AddStatement($"var entity = await repository.FindByIdAsync({model.Parameters.First().Name.ToCamelCase()}, cancellationToken);");
                                        method.AddStatement($"return entity.MapTo{model.TypeReference.Element.Name.ToPascalCase()}(mapper);");
                                    }
                                    else if (!model.Parameters.Any() &&
                                             model.TypeReference.Element.AsDTOModel()?.Mapping.Element.IsClassModel() == true)
                                    {
                                        method.AddStatement($"var entities = await repository.FindAllAsync(cancellationToken);");
                                        method.AddStatement($"return entities.MapTo{model.TypeReference.Element.Name.ToPascalCase()}List(mapper);");
                                    }
                                }
                            }
                        }
                    }
                }, 200);
            }
        }
    }
}