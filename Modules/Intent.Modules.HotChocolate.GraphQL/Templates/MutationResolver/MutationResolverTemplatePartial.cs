using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.HotChocolate.GraphQL.FactoryExtensions;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
using Intent.Modules.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.MutationResolver
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MutationResolverTemplate : CSharpTemplateBase<IGraphQLMutationTypeModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.MutationResolver";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MutationResolverTemplate(IOutputTarget outputTarget, IGraphQLMutationTypeModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("System.Collections.Generic.IReadOnlyList<{0}>"));
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddAttribute($"[{UseType("HotChocolate.Types.ExtendObjectType")}]", attr => attr.AddArgument("Name = \"Mutation\""));

                    foreach (var mutation in Model.Resolvers)
                    {
                        @class.AddMethod($"Task<{GetTypeName(mutation)}>", mutation.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", mutation);
                            method.Async();
                            foreach (var parameter in mutation.Parameters)
                            {
                                method.AddParameter(GetTypeName(parameter), parameter.Name.ToCamelCase());
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