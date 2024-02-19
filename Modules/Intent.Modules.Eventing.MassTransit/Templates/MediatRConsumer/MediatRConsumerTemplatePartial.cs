using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.UnitOfWork.Persistence.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using OutboxPatternType = Intent.Modules.Eventing.MassTransit.Settings.EventingSettings.OutboxPatternOptionsEnum;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MediatRConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MediatRConsumerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.MediatRConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MediatRConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ConsumerHelper.AddConsumerDependencies(this);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            ConsumerHelper.AddConsumerClass(this,
                file: CSharpFile,
                baseName: "MediatR",
                configureClass: (@class, tMessage) =>
                {
                    @class.GenericTypeConstraints.First(p => p.GenericTypeParameter == tMessage).AddType(UseType("MediatR.IBaseRequest"));
                },
                configureConsumeMethod: (@class, method, tMessage) =>
                {
                    method.AddStatement($"var sender = _serviceProvider.GetRequiredService<{UseType("MediatR.ISender")}>();",
                        stmt => stmt.AddMetadata("handler", "instantiate"));
                    method.AddStatement($"var response = await sender.Send(context.Message, context.CancellationToken);",
                        stmt => stmt.AddMetadata("handler", "execute"));
                },
                applyStandardUnitOfWorkLogic: false);
            ConsumerHelper.AddConsumerDefinitionClass(this,
                file: CSharpFile,
                baseName: "MediatR");
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