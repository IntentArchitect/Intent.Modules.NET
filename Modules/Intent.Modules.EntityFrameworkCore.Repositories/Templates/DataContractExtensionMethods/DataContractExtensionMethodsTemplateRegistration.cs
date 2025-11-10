using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.DataContractExtensionMethods
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DataContractExtensionMethodsTemplateRegistration : FilePerModelTemplateRegistration<DataContractModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DataContractExtensionMethodsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DataContractExtensionMethodsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DataContractModel model)
        {
            return new DataContractExtensionMethodsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DataContractModel> GetModels(IApplication application)
        {
            var spModels = _metadataManager.Domain(application).GetRepositoryModels()
                .SelectMany(repository => repository.GetStoredProcedureModels())
                .ToArray();
            var dcModels = spModels
                .SelectMany(storedProcedure => storedProcedure.Parameters
                    .Where(parameter => parameter.ParameterType?.Element.IsDataContractModel() == true)
                    .Select(parameter => parameter.ParameterType.Element.AsDataContractModel()))
                .Concat(spModels
                    .Where(m => m.ReturnType?.Element.IsDataContractModel() == true)
                    .Select(m => m.ReturnType.Element.AsDataContractModel()))
                .Distinct()
                .ToList();
            return dcModels;
        }
    }
}