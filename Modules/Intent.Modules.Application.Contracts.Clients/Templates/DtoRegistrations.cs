using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    [Description("Intent Applications Contracts DTO")]
    public class DtoRegistrations : FilePerModelTemplateRegistration<ServiceProxyDTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DtoRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateIds.ClientDto;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, ServiceProxyDTOModel model)
        {
            return new DtoModelTemplate(project, model);
        }

        public override IEnumerable<ServiceProxyDTOModel> GetModels(IApplication application)
        {
            return _metadataManager.ServiceProxies(application).GetServiceProxyDTOModels();
        }
    }
}
