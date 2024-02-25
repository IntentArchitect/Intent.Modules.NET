using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.Consumers.MediatRConsumer
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MediatRConsumerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.Consumers.MediatRConsumer";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MediatRConsumerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ConsumerHelper.AddConsumerDependencies(this);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            ConsumerHelper.AddConsumerClass(this,
                baseName: "MediatR",
                configureClass: (@class, tMessage) =>
                {
                },
                configureConsumeMethod: (@class, method, tMessage) =>
                {
                    method.AddStatement(
                        $$"""
                        if (context.Message is not {{this.GetMapperRequestInterfaceName()}} mapperRequest)
                        {
                            throw new Exception("Message type must inherit from IMapperRequest in order to proceed");
                        }
                        var request = mapperRequest.CreateRequest();
                        """);

                    method.AddStatement($"var sender = _serviceProvider.GetRequiredService<{UseType("MediatR.ISender")}>();",
                        stmt => stmt.AddMetadata("handler", "instantiate").SeparatedFromPrevious());
                    method.AddStatement($"var response = await sender.Send(request, context.CancellationToken);",
                        stmt => stmt.AddMetadata("handler", "execute"));

                    method.AddStatement(
                        $$"""
                          var mappedResponse = {{this.GetResponseMappingFactoryName()}}.CreateResponseMessage(request, response);
                          await context.RespondAsync(mappedResponse);
                          """, stmt => stmt.SeparatedFromPrevious());
                },
                applyStandardUnitOfWorkLogic: false);
            ConsumerHelper.AddConsumerDefinitionClass(this,
                baseName: "MediatR");
        }

        public override bool CanRunTemplate()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);
            var relevantCommands = services.GetElementsOfType("Command")
                .Where(p => p.HasStereotype(Constants.MessageTriggered) && ExecutionContext.TemplateExists(TemplateRoles.Application.Handler.Command, p.Id));
            var relevantQueries = services.GetElementsOfType("Query")
                .Where(p => p.HasStereotype(Constants.MessageTriggered) && ExecutionContext.TemplateExists(TemplateRoles.Application.Handler.Query, p.Id));
            return relevantCommands.Concat(relevantQueries).Any();
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