using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IntegrationEventMessageTemplate : CSharpTemplateBase<MessageModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.IntegrationEventMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventMessageTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateId);
            AddTypeSource(IntegrationEventEnumTemplate.TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(IntegrationEventDtoTemplate.TemplateId);

            var completeNamespace = GetNamespace(model);

            CSharpFile = new CSharpFile(
                    @namespace: completeNamespace,
                    relativeLocation: this.GetFolderPath())
                .AddRecord($"{Model.Name.RemoveSuffix("Event")}Event", record =>
                {
                    // See this article on how to handle NRTs for DTOs
                    // https://github.com/dotnet/docs/issues/18099
                    var nullableMembers = Model.Properties
                        .Where(property => NeedsNullabilityAssignment(GetTypeInfo(property.TypeReference)))
                        .Select(x => $"{x.Name.ToPascalCase()} = null!;")
                        .ToArray();

                    if (nullableMembers.Any())
                    {
                        record.AddConstructor(ctor =>
                        {
                            ctor.AddStatements(nullableMembers);
                        });
                    }

                    foreach (var property in Model.Properties)
                    {
                        record.AddProperty(GetTypeName(property), property.Name.ToPascalCase(), p =>
                        {
                            p.Init();
                            p.RepresentsModel(property);
                        });
                    }
                });
        }
        
        private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !(typeInfo.IsPrimitive
                     || typeInfo.IsNullable
                     || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel()));
        }
        
        // Keep GetNamespace() in sync with:
        // - Intent.Modules.IaC.Terraform.Templates.Subscriptions.AzureEventGridTf.AzureEventGridTfTemplate
        private static string GetNamespace(MessageModel model)
        {
            var classNamespace = model.InternalElement.Package.Name.ToCSharpNamespace();
            var extendedNamespace = model.GetParentFolders().Where(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.Name))
                    {
                        return false;
                    }

                    return !x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider();
                })
                .Select(x => x.Name);
            var completeNamespace = string.Join(".", classNamespace.Split(".").Concat(extendedNamespace));
            return completeNamespace;
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