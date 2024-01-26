using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.DtoContract
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DtoContractTemplateRegistration : FilePerModelTemplateRegistration<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DtoContractTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DtoContractTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DTOModel model)
        {
            return new DtoContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            return _metadataManager.GetServicesAsProxyModels(application).SelectMany(p => GetReferencedDTOModels(p, true)).Distinct()
            .ToList();
        }

        internal IEnumerable<DTOModel> GetReferencedDTOModels(IServiceProxyModel proxy, bool includeReturnTypes)
        {
            return (from x in DeepGetDistinctReferencedElements(proxy.GetMappedEndpoints().Select(ep => ep.InternalElement), includeReturnTypes).Where(delegate (IElement x)
            {
                string specializationTypeId = x.SpecializationTypeId;
                bool flag = ((specializationTypeId == "d4e577cd-ad05-4180-9a2e-fff4ddea0e1e" || specializationTypeId == "85fba0e9-9161-4c85-a603-a229ef312beb") ? true : false);
                return !flag;
            })
                    select new DTOModel(x)).ToList();
        }

        private ISet<IElement> DeepGetDistinctReferencedElements(IEnumerable<IElement> elements, bool includeReturnTypes = true)
        {
            HashSet<IElement> hashSet = new HashSet<IElement>();
            Stack<IElement> stack = new Stack<IElement>(elements);
            while (stack.Any())
            {
                IElement element = stack.Pop();
                bool flag;
                switch (element.SpecializationType)
                {
                    case "DTO":
                    case "Command":
                    case "Query":
                        flag = true;
                        break;
                    default:
                        flag = false;
                        break;
                }

                bool flag2 = flag;
                if ((includeReturnTypes || !flag2) && element.TypeReference?.Element is IElement element2 && hashSet.Add(element2))
                {
                    foreach (IElement childElement in element2.ChildElements)
                    {
                        stack.Push(childElement);
                    }
                }

                foreach (ITypeReference item in element.TypeReference?.GenericTypeParameters ?? new List<ITypeReference>())
                {
                    if (!(item?.Element is IElement element3) || !hashSet.Add(element3))
                    {
                        continue;
                    }

                    foreach (IElement childElement2 in element3.ChildElements)
                    {
                        stack.Push(childElement2);
                    }
                }

                if (flag2)
                {
                    hashSet.Add(element);
                }

                foreach (IElement childElement3 in element.ChildElements)
                {
                    stack.Push(childElement3);
                }
            }

            return hashSet;
        }
        internal IEnumerable<EnumModel> GetReferencedEnumModels(IServiceProxyModel proxy)
        {
            return (from x in DeepGetDistinctReferencedElements(proxy.GetMappedEndpoints().Select(o => o.InternalElement))
                    where x.SpecializationTypeId == "85fba0e9-9161-4c85-a603-a229ef312beb"
                    select new EnumModel(x)).ToList();
        }
    }
}