using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates.MessageExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MessageExtensionsTemplateRegistration : FilePerModelTemplateRegistration<MessageModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MessageExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => MessageExtensionsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, MessageModel model)
        {
            return new MessageExtensionsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<MessageModel> GetModels(IApplication application)
        {
            var app = _metadataManager.Eventing(application).GetApplicationModels().SingleOrDefault();
            return _metadataManager.Eventing(application)
                .GetMessageModels()
                .Where(model => HasMappedDomainEntityPresent(app, model, application))
                .ToList();
        }

        private bool HasMappedDomainEntityPresent(ApplicationModel applicationModel, MessageModel messageModel, IApplication application)
        {
            if (applicationModel.PublishedMessages().All(p => p.Element.AsMessageModel().Id != messageModel.Id))
            {
                return false;
            }

            var domainMapping = messageModel.GetMapFromDomainMapping();
            if (domainMapping == null)
            {
                return false;
            }

            var domainClasses = _metadataManager.Domain(application).Elements;
            return domainClasses.Any(p => p.Id == domainMapping.ElementId);
        }
    }
}