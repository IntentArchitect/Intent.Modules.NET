﻿using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.Application.DomainInteractions
{
    public enum ActionType
    { 
        Query,
        Delete,
        Update
    }

    public class QueryActionContext
    {
        private readonly ICSharpClass _handlerClass;
        private readonly ClassModel _entity;
        private readonly IAssociationEnd _associationEnd;
        private readonly IElement _serviceEndPoint;
        private readonly IElement? _returnType;
        private readonly ICSharpTemplate _template;
        private readonly ICSharpClassMethodDeclaration _method;
        private readonly ActionType _actionType;
        private readonly bool _isPaginated;

        public QueryActionContext(ICSharpClassMethodDeclaration method, ActionType actionType, IAssociationEnd queryAction)
        {
            _method = method;
            _actionType = actionType;
            _template = method.File.Template;
            _handlerClass = method.Class;
            _entity = queryAction.TypeReference.Element.AsClassModel();
            if (_actionType == ActionType.Update && _entity == null) 
            {
                _entity = OperationModelExtensions.AsOperationModel(queryAction.TypeReference.Element).ParentClass;
            }
            _associationEnd = queryAction;
            _serviceEndPoint = queryAction.Association.SourceEnd.TypeReference.Element as IElement;
            _returnType = _serviceEndPoint.TypeReference?.Element as IElement;
            if (_returnType?.Name == "PagedResult")
            {
                _isPaginated = true;
                _returnType = _serviceEndPoint.TypeReference.GenericTypeParameters.FirstOrDefault()?.Element as IElement;
            }
            if (IsConfiguredForProjections())
            {
                //We doing this because the CRUD modules are generally compatible with AutoMapper module v4 this Feature requires V5.
                //Because this feature may need a major version update not forcing the dependency for everyone (in 'imod' spec)
                AssertAutoMapperVersion();
            }
        }

        private void AssertAutoMapperVersion()
        {
            var autoMapperModule = _template.ExecutionContext.InstalledModules.FirstOrDefault(m => m.ModuleId == "Intent.Application.AutoMapper");
            if (autoMapperModule == null) 
            {
                throw new ElementException(_associationEnd,"Default Query Implementation - 'Project To' install module 'Intent.Application.AutoMapper' v 5.1.4-pre.0 or higher");
            }

            var verionText = autoMapperModule.Version.Split('.');
            if (verionText.Length >= 3) 
            {
                var current = new Version(int.Parse(verionText[0]), int.Parse(verionText[1]), int.Parse(verionText[2][0].ToString()));
                if (current >= new Version(5, 1, 4))
                {
                    return;
                }
            }
            throw new ElementException(_associationEnd, "Default Query Implementation - 'Project To' update module 'Intent.Application.AutoMapper' to v 5.1.4-pre.0 or higher");
        }

        public ActionType ActionType { get { return _actionType; } }

        public IAssociationEnd AssociationEnd
        {
            get
            {
                return _associationEnd;
            }
        }

        public ICSharpClassMethodDeclaration Method
        {
            get
            {
                return _method;
            }
        }

        public ICSharpClass HandlerClass
        {
            get
            {
                return _handlerClass;
            }
        }

        public ClassModel FoundEntity
        {
            get
            {
                return _entity;
            }
        }

        public IElement? ReturnType
        {
            get
            {
                return _returnType;
            }
        }

        public bool IsPaginated
        {
            get
            {
                return _isPaginated;
            }
        }


        public bool ImplementWithProjections()
        {
            return ActionType == ActionType.Query && IsConfiguredForProjections() && QueryIsOnlyAction() && ReturnTypeIsMappableFromEntity();
        }

        public bool QueryIsOnlyAction()
        {            
            return _serviceEndPoint.AssociatedElements.Count() == 1 && !_serviceEndPoint.ChildElements.Any(c => c.SpecializationTypeId == "405a2857-b911-431f-8142-719a0e9f15f3");// Processing Action
        }

        public bool ReturnTypeIsMappableFromEntity()
        {
            if (_returnType != null && _returnType.SpecializationType is "DTO")
            {
                var dto = _returnType.AsDTOModel();
                if (dto.IsMapped && dto.Mapping.Element.IsClassModel())
                {
                    return _entity.Id == dto.Mapping.Element.Id;
                }
            }
            return false;
        }

        public string GetDtoProjectionReturnType()
        {
            if (ReturnType != null)
            {
                if (_template.TryGetTypeName("Application.Contract.Dto", _returnType, out var returnDto))
                {
                    return returnDto;
                }
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
