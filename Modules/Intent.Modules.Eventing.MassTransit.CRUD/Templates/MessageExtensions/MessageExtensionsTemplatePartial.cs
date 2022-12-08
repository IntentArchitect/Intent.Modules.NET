using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.CRUD.Api;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.CRUD.Templates.MessageExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class MessageExtensionsTemplate : CSharpTemplateBase<MessageModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Modules.Eventing.MassTransit.CRUD.MessageExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageExtensionsTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource("Domain.Enum");
            
            CSharpFile = new CSharpFile($"{Model.InternalElement.Package.Name.ToPascalCase()}", this.GetFolderPath())
                .OnBuild(file =>
                {
                    file.AddClass($"{Model.Name.RemoveSuffix("Event")}EventExtensions", @class =>
                    {
                        @class.Static();
                        var messageTemplate = GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, model);
                        @class.AddMethod(GetTypeName(model.InternalElement), $"MapTo{messageTemplate.ClassName}", method =>
                        {
                            method.Static();
                            method.AddParameter(GetTypeName(model.GetMapFromDomainMapping()), "projectFrom", param => param.WithThisModifier());
                            
                            var codeLines = new CSharpStatementAggregator();
                            codeLines.Add($"return new {GetTypeName(model.InternalElement)}");
                            codeLines.Add(new CSharpStatementBlock()
                                .AddStatements(model.Properties.Select(x => $"{x.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)} = projectFrom.{string.Join(".", x.InternalElement.MappedElement.Path.Select(y=>y.Name))},"))
                                .WithSemicolon());
                            method.AddStatements(codeLines.ToList());
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Event")}EventExtensions",
                @namespace: $"{Model.InternalElement.Package.Name.ToPascalCase()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public CSharpFile CSharpFile { get; }
    }
}