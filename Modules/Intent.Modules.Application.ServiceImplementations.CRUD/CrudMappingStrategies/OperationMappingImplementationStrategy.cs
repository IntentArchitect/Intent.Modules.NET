using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Mapping.Resolvers;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.VisualBasic;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.CrudMappingStrategies
{
    public class OperationMappingImplementationStrategy : IImplementationStrategy
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public OperationMappingImplementationStrategy(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        public bool IsMatch(OperationModel operationModel)
        {
            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id);
            return method is not null;
        }

        public void BindToTemplate(ICSharpFileBuilderTemplate template, OperationModel operationModel)
        {

            if (operationModel.ReturnType?.Element != null && operationModel.ReturnType.Element.Name.Contains("PagedResult") && operationModel.Parameters.Any(x => x.Name.ToLower() == "orderby"))
            {
                AddOrderBy();

                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = _template.CSharpFile.Classes.First();
                    var method = @class.FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id);
                    if (method.ReturnTypeInfo.IsTask())
                    {
                        if (method.ReturnTypeInfo.GetTaskGenericType().ToString().StartsWith("PagedResult"))
                        {
                            string pagedResultTypeName = template.GetTypeName((IElement)operationModel.ReturnType.Element);
                            method.WithReturnType(method.ReturnType.Replace("PagedResult", pagedResultTypeName));
                        }
                    }
                }, 100);
            }
            template.CSharpFile.AfterBuild(_ => ApplyStrategy(operationModel));
        }
        private string AddOrderBy()
        {
            _template.AddNugetDependency(SharedNuGetPackages.SystemLinqDynamicCore);
            _template.AddUsing("static System.Linq.Dynamic.Core.DynamicQueryableExtensions");
            return "OrderBy";
        }


        public void ApplyStrategy(OperationModel operationModel)
        {
            // TODO: This can be added to the IsMatch operation, but that needs to be addressed carefully
            var interactions = operationModel.GetInteractions().ToList();
            if (interactions.Count <= 0) return;

            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddTypeSource(TemplateRoles.Domain.DataContract);

            //_template.AddUsing("System.Linq"); // Should not be a blanket add of this using clause.
            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id);
            method.Statements.Clear();
            method.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            var csharpMapping = method.GetMappingManager();

            if (operationModel.GetStereotype("Http Settings")?.TryGetProperty("Verb", out var verb) == true && verb.Value == "PATCH")
            {
                csharpMapping.AddMappingResolver(new EntityPatchMappingTypeResolver(_template));
            }

            csharpMapping.AddMappingResolver(new EntityCreationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new ServiceOperationMappingTypeResolver(_template));
            csharpMapping.AddMappingResolver(new CommandQueryMappingResolver(_template));
            csharpMapping.AddMappingResolver(new TypeConvertingMappingResolver(_template));

            csharpMapping.SetFromReplacement(operationModel, null); // Ignore the method itself

            method.ImplementInteractions(interactions);

            if (operationModel.TypeReference.Element != null)
            {
                var returnStatement = method.GetReturnStatements(operationModel.TypeReference);
                method.AddStatements(ExecutionPhases.Response, returnStatement);
            }
        }
    }
}