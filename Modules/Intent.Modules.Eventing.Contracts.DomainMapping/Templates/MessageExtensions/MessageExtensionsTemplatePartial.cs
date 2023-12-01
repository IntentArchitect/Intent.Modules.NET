using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates.MessageExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageExtensionsTemplate : CSharpTemplateBase<MessageModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.DomainMapping.MessageExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageExtensionsTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventEnumTemplate.TemplateId);
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource(IntegrationEventDtoTemplate.TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.Events);

            CSharpFile = new CSharpFile($"{Model.InternalElement.Package.Name.ToPascalCase()}", this.GetFolderPath())
                .AddClass($"{Model.Name.RemoveSuffix("Event")}EventExtensions", @class =>
                {
                    @class.Static();
                    var messageTemplate = GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, model);
                    @class.AddMethod(GetTypeName(model.InternalElement), $"MapTo{messageTemplate.ClassName}", method =>
                    {
                        method.Static();
                        method.AddParameter(GetTypeName(model.GetMapFromDomainMapping()), "projectFrom", param => param.WithThisModifier());

                        var domainEntity = ((IElement)model.GetMapFromDomainMapping().Element).AsClassModel();
                        if (domainEntity != null)
                        {
                            var codeLines = new CSharpStatementAggregator();
                            codeLines.Add($"return new {GetTypeName(model.InternalElement)}");
                            codeLines.Add(new CSharpStatementBlock()
                                .AddStatements(MappingExtensionHelper.GetPropertyAssignments("projectFrom", domainEntity.Attributes.Select(x => x.InternalElement), model.Properties.Select(x => x.InternalElement), this))
                                .WithSemicolon());
                            method.AddStatements(codeLines.ToList());
                        }

                        var domainEvent = ((IElement)model.GetMapFromDomainMapping().Element).AsDomainEventModel();
                        if (domainEvent != null)
                        {
                            var codeLines = new CSharpStatementAggregator();
                            codeLines.Add($"return new {GetTypeName(model.InternalElement)}");
                            codeLines.Add(new CSharpStatementBlock()
                                .AddStatements(MappingExtensionHelper.GetPropertyAssignments("projectFrom", domainEvent.Properties.Select(x => x.InternalElement), model.Properties.Select(x => x.InternalElement), this))
                                .WithSemicolon());
                            method.AddStatements(codeLines.ToList());
                        }
                    });
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