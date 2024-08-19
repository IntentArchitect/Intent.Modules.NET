using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.SubscriptionType
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SubscriptionTypeTemplate : CSharpTemplateBase<GraphQLSubscriptionTypeModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.SubscriptionType";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SubscriptionTypeTemplate(IOutputTarget outputTarget, GraphQLSubscriptionTypeModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.HotChocolate(OutputTarget));
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.ValueObject);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddAttribute($"[{UseType("HotChocolate.Types.ExtendObjectType")}]", attr => attr.AddArgument($"{UseType("HotChocolate.Language.OperationType")}.Subscription"));

                    foreach (var subscription in Model.Subscriptions)
                    {
                        @class.AddMethod($"{GetTypeName(subscription)}", subscription.Name.ToPascalCase(), method =>
                        {
                            method.AddMetadata("model", subscription);
                            if (!string.IsNullOrWhiteSpace(subscription.Comment))
                            {
                                @method.AddAttribute("GraphQLDescription", attr => attr.AddArgument($@"""{subscription.Comment}"""));
                            }

                            @method.AddAttribute(UseType("HotChocolate.Types.Subscribe"));
                            if (subscription.EventMessage != null)
                            {
                                method.AddParameter(GetTypeName(subscription.EventMessage), subscription.EventMessage.Name.ToCamelCase(), param =>
                                {
                                    if (!string.IsNullOrWhiteSpace(subscription.EventMessage.Comment))
                                    {
                                        param.AddAttribute("GraphQLDescription", attr => attr.AddArgument($@"""{subscription.EventMessage.Comment}"""));
                                    }

                                    param.AddAttribute(UseType("HotChocolate.EventMessage"));
                                });
                                if (subscription.EventMessage.TypeReference.Element == subscription.TypeReference.Element)
                                {
                                    @method.AddStatement($"return {subscription.EventMessage.Name.ToCamelCase()};");
                                }
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