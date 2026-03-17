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
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts;
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

        private IEnumerable<ElementWrapper> GetRelevantElements()
        {
            var services = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id);
            var relevantCommands = services.GetElementsOfType("Command")
                .Where(p => p.HasStereotype(Constants.MessageTriggered));

            foreach (var command in relevantCommands)
            {
                yield return new ElementWrapper(command);
            }
            
            var relevantQueries = services.GetElementsOfType("Query")
                .Where(p => p.HasStereotype(Constants.MessageTriggered));

            foreach (var query in relevantQueries)
            {
                yield return new ElementWrapper(query);
            }
            
            var serviceProxies = ExecutionContext.MetadataManager.ServiceProxies(ExecutionContext.GetApplicationConfig().Id).GetServiceProxyModels();
            foreach (var proxyModel in serviceProxies)
            {
                if (proxyModel.InternalElement.MappedElement?.Element is not null)
                {
                    yield return new ElementWrapper((IElement)proxyModel.InternalElement.MappedElement.Element);
                }
            }
        }

        private record ElementWrapper(IElement InternalElement)
            : IHasFolder, IElementWrapper
        {
            public FolderModel Folder => InternalElement.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId 
                ? new FolderModel(InternalElement.ParentElement) 
                : null!;
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