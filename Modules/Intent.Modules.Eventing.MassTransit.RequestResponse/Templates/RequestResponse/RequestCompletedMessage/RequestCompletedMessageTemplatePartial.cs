using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.RequestCompletedMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RequestCompletedMessageTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.RequestResponse.RequestCompletedMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RequestCompletedMessageTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            var relevantElement = GetRelevantElements().FirstOrDefault();
            var @namespace = relevantElement is null ? string.Empty : ExtensionMethods.GetPackageOnlyNamespace(relevantElement);
            CSharpFile = new CSharpFile(@namespace, this.GetFolderPath())
                .AddClass("RequestCompletedMessage", @class =>
                {
                    @class.AddProperty(@class.Name, "Instance", prop => prop.Static().WithInitialValue($"new {@class.Name}()").WithoutSetter());
                })
                .AddClass("RequestCompletedMessage", @class =>
                {
                    @class.AddGenericParameter("T", out var typeT);
                    @class.AddConstructor(ctor => ctor.AddStatement("Payload = default!;"));
                    @class.AddConstructor(ctor => ctor.AddParameter(typeT, "payload", param => param.IntroduceProperty()));
                });
        }

        public override bool CanRunTemplate()
        {
            var relevantCommands = GetRelevantElements();
            return relevantCommands.Any();
        }

        private IEnumerable<HybridDtoModel> GetRelevantElements()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);
            var relevantCommands = services.GetElementsOfType("Command")
                .Where(p => p.HasStereotype(Constants.MessageTriggered));
            var relevantQueries = services.GetElementsOfType("Query")
                .Where(p => p.HasStereotype(Constants.MessageTriggered));

            var proxyMappedService = new MassTransitServiceProxyMappedService();

            var serviceProxies = this.ExecutionContext.MetadataManager
                .ServiceProxies(this.ExecutionContext.GetApplicationConfig().Id)
                .GetServiceProxyModels();
            var relevantProxyElements = serviceProxies.SelectMany(proxyModel => proxyMappedService.GetMappedEndpoints(proxyModel))
                .SelectMany(s => s.Inputs).Select(s => s.TypeReference?.Element).Where(p => p is not null).Cast<IElement>();

            return relevantCommands.Concat(relevantQueries).Concat(relevantProxyElements).Select(element => new HybridDtoModel(element));
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