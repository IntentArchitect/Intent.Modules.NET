using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.ServiceProtoFile
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ServiceProtoFileTemplateRegistration : FilePerModelTemplateRegistration<IElement>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceProtoFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceProtoFileTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IElement model)
        {
            return new ServiceProtoFileTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IElement> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).Elements
                .Where(x => x.SpecializationTypeId is MetadataIds.QueryElementTypeId or MetadataIds.CommandElementTypeId or OperationModel.SpecializationTypeId &&
                            x.HasStereotype(MetadataIds.ExposeWithGrpcStereotypeId))
                .GroupBy(x => x.ParentElement)
                .Select(x => x.Key);
        }
    }
}