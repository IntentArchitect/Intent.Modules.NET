using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies
{
    public class ODataGetAllImplementationStrategy : ICrudImplementationStrategy
    {
        private readonly CSharpTemplateBase<QueryModel> _template;
        private readonly IApplication _application;
        private readonly Lazy<StrategyData> _matchingElementDetails;
        private const string ODataQueryStereoType = "ODataQuery";

        public ODataGetAllImplementationStrategy(CSharpTemplateBase<QueryModel> template, IApplication application)
        {
            _template = template;
            _application = application;
            _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
        }
        public void BindToTemplate(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy());
        }

        public bool IsMatch()
        {
            if (_template.Model.TypeReference.Element == null || 
                !_template.Model.TypeReference.IsCollection || 
                !_template.Model.HasStereotype(ODataQueryStereoType))
            {
                return false;
            }

            return _matchingElementDetails.Value.IsMatch;
        }

        public void ApplyStrategy()
        {
            var config = _matchingElementDetails.Value;
            var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
            var ctor = @class.Constructors.First();
            var repository = config.Repository;
            ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            if (config.AllowsSelect)
            {
                handleMethod.AddStatement($"return await {repository.FieldName}.FindAllProjectToWithTransformationAsync(filterExpression: null, transform: request.Transform, cancellationToken: cancellationToken);");
            }
            else
            {
                handleMethod.AddStatement($"return await {repository.FieldName}.FindAllProjectToAsync(filterExpression: null, filterProjection: request.Transform, cancellationToken: cancellationToken);");
            }
        }

        private StrategyData GetMatchingElementDetails()
        {
            if (!_template.Model.TypeReference?.IsCollection == true)
            {
                return NoMatch;
            }

            if (!_template.Model.HasStereotype(ODataQueryStereoType))
            {
                return NoMatch;
            }

            var returnDto = _template.Model.TypeReference?.Element.AsDTOModel();
            if (returnDto?.Mapping == null)
            {
                return NoMatch;
            }

            var foundEntity = returnDto.Mapping.Element.AsClassModel() ?? throw new Exception("Expected ClassModel as Query Mapping in ODataGetAllImplementationStrategy");
            var dtoToReturn = _application.MetadataManager.Services(_application)
                .GetDTOModels().SingleOrDefault(x =>
                    x.Id == _template.Model.TypeReference.Element.Id && x.IsMapped &&
                    x.Mapping.ElementId == foundEntity.Id);

            if (dtoToReturn == null)
            {
                return NoMatch;
            }

            var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, nestedCompOwner != null ? nestedCompOwner : foundEntity, out var repositoryInterface))
            {
                return NoMatch;
            }

            var repository = new RequiredService(type: repositoryInterface,
                name: repositoryInterface.Substring(1).ToCamelCase());
            var allowsSelect = _template.Model.GetStereotype(ODataQueryStereoType).GetProperty<bool>("Enable Select", false);

            return new StrategyData(true, foundEntity, dtoToReturn, repository, allowsSelect);
        }

        private static readonly StrategyData NoMatch = new StrategyData(false, null, null, null, false);

        private class StrategyData
        {
            public StrategyData(bool isMatch, ClassModel foundEntity, DTOModel dtoToReturn, RequiredService repository, bool allowsSelect)
            {
                IsMatch = isMatch;
                FoundEntity = foundEntity;
                DtoToReturn = dtoToReturn;
                Repository = repository;
                AllowsSelect = allowsSelect;
            }

            public bool IsMatch { get; }
            public ClassModel FoundEntity { get; }
            public DTOModel DtoToReturn { get; }
            public RequiredService Repository { get; }
            public bool AllowsSelect { get; }
        }
    }
}
