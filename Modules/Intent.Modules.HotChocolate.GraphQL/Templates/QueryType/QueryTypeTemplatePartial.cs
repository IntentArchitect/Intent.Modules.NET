using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.QueryType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryTypeTemplate : CSharpTemplateBase<IGraphQLQueryTypeModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.QueryType";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryTypeTemplate(IOutputTarget outputTarget, IGraphQLQueryTypeModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.HotChocolate);
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("System.Collections.Generic.IReadOnlyList<{0}>"));
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddAttribute($"[{UseType("HotChocolate.Types.ExtendObjectType")}]", attr => attr.AddArgument($"{UseType("HotChocolate.Language.OperationType")}.Query"));

                    foreach (var resolver in Model.Resolvers)
                    {
                        if (resolver.TypeReference.Element == null)
                        {
                            Logging.Log.Warning($"GraphQL queries must define a return type. {Model.Name}.{resolver.Name} does not specify a return type. This operation may not show in the GraphQL schema for this reason.");
                        }
                        @class.AddMethod($"{GetTypeName(resolver)}", resolver.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", resolver);
                            if (!string.IsNullOrWhiteSpace(resolver.Description))
                            {
                                method.AddAttribute("GraphQLDescription", attr => attr.AddArgument($@"""{resolver.Description}"""));
                            }
                            if (!method.ReturnType.StartsWith("IQueryable"))
                            {
                                method.Async();
                            }
                            foreach (var parameter in resolver.Parameters)
                            {
                                if (parameter.TypeReference.Element.AsDTOModel()?.Fields.Count == 0)
                                {
                                    Logging.Log.Warning($"GraphQL query {Model.Name}.{resolver.Name} has an empty complex type parameter [{parameter.Name}: {parameter.TypeReference.Element.Name}]. This may cause errors.");
                                }
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToCamelCase(), param =>
                                {
                                    if (!string.IsNullOrWhiteSpace(parameter.Description))
                                    {
                                        param.AddAttribute("GraphQLDescription", attr => attr.AddArgument($@"""{parameter.Description}"""));
                                    }
                                });
                            }
                        });
                    }
                });
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