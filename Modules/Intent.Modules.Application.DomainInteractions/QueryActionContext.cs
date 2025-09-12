using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.Application.DomainInteractions
{
    internal class QueryActionContext
    {
        private readonly IElement _serviceEndPoint;
        private readonly ICSharpTemplate _template;

        public QueryActionContext(ICSharpClassMethodDeclaration method, ActionType actionType, IAssociationEnd queryAction)
        {
            Method = method;
            ActionType = actionType;
            _template = method.File.Template;
            HandlerClass = method.Class;
            FoundEntity = queryAction.TypeReference.Element.AsClassModel();
            if (ActionType == ActionType.Update && FoundEntity == null)
            {
                FoundEntity = OperationModelExtensions.AsOperationModel(queryAction.TypeReference.Element).ParentClass;
            }
            AssociationEnd = queryAction;
            _serviceEndPoint = (IElement)queryAction.Association.SourceEnd.TypeReference.Element;
            ReturnType = _serviceEndPoint.TypeReference?.Element as IElement;
            if (ReturnType?.Name == "PagedResult")
            {
                IsPaginated = true;
                ReturnType = _serviceEndPoint.TypeReference!.GenericTypeParameters.FirstOrDefault()?.Element as IElement;
            }
            if (IsConfiguredForProjections())
            {
                //We're doing this because the CRUD modules are generally compatible with AutoMapper module v4 this Feature requires V5.
                //Because this feature may need a major version update not forcing the dependency for everyone (in 'imod' spec)
                AssertAutoMapperVersion();
            }
        }

        private void AssertAutoMapperVersion()
        {
            var autoMapperModule = _template.ExecutionContext.InstalledModules.FirstOrDefault(m => m.ModuleId == "Intent.Application.AutoMapper");
            if (autoMapperModule == null)
            {
                throw new ElementException(AssociationEnd, "Default Query Implementation - 'Project To' install module 'Intent.Application.AutoMapper' v 5.1.4-pre.0 or higher");
            }

            var versionText = autoMapperModule.Version.Split('.');
            if (versionText.Length >= 3)
            {
                var current = new Version(int.Parse(versionText[0]), int.Parse(versionText[1]), int.Parse(versionText[2][0].ToString()));
                if (current >= new Version(5, 1, 4))
                {
                    return;
                }
            }
            throw new ElementException(AssociationEnd, "Default Query Implementation - 'Project To' update module 'Intent.Application.AutoMapper' to v 5.1.4-pre.0 or higher");
        }

        public ActionType ActionType { get; }

        public IAssociationEnd AssociationEnd { get; }

        public ICSharpClassMethodDeclaration Method { get; }

        public ICSharpClass HandlerClass { get; }

        public ClassModel FoundEntity { get; }

        public IElement? ReturnType { get; }

        public bool IsPaginated { get; }

        public bool ImplementWithProjections()
        {
            return ActionType == ActionType.Query && IsConfiguredForProjections() && QueryIsOnlyAction() && ReturnTypeIsMappableFromEntity();
        }

        public bool QueryIsOnlyAction()
        {
            return _serviceEndPoint.AssociatedElements.Count() == 1 && _serviceEndPoint.ChildElements.All(c => c.SpecializationTypeId != "405a2857-b911-431f-8142-719a0e9f15f3");// Processing Action
        }

        public bool ReturnTypeIsMappableFromEntity()
        {
            if (ReturnType is not { SpecializationType: "DTO" })
            {
                return false;
            }

            var dto = ReturnType.AsDTOModel();
            if (dto.IsMapped && dto.Mapping.Element.IsClassModel())
            {
                return FoundEntity.Id == dto.Mapping.Element.Id;
            }

            return false;
        }

        public string GetDtoProjectionReturnType()
        {
            if (ReturnType != null &&
                _template.TryGetTypeName("Application.Contract.Dto", ReturnType!, out var returnDto))
            {
                return returnDto;
            }

            throw new Exception("Not a Dto Return type");
        }

        private bool IsConfiguredForProjections()
        {
            return _template
                        .ExecutionContext
                        .Settings
                        .GetGroup("0ecca7c5-96f7-449a-96b9-f65ba0a4e3ad")?//Domain Interaction Settings
                        .GetSetting("61ced3b4-e1d8-4274-b84d-d9b8e0c3143f")?//Default Query Implementation
                        .Value == "project-to";
        }

    }
}
