using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Exceptions;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies;

public class CallDomainServiceInteractionStrategy : IInteractionStrategy
{
    private ICSharpFileBuilderTemplate _template;
    private CSharpClassMappingManager _csharpMapping;

    public bool IsMatch(IElement interaction)
    {
        return interaction.IsServiceInvocationTargetEndModel() && Intent.Modelers.Domain.Api.OperationModelExtensions.IsOperationModel(interaction.TypeReference.Element);
    }

    public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
    {
        var interaction = (IAssociationEnd)interactionElement;
        var @class = method.Class;
        _template = (ICSharpFileBuilderTemplate)@class.File.Template;
        _csharpMapping = method.GetMappingManager();
        _csharpMapping.AddMappingResolver(new CallServiceOperationMappingResolver(_template));
        try
        {
            var statements = new List<CSharpStatement>();
            var serviceModel = ((IElement)interaction.TypeReference.Element).ParentElement;
            if (interaction.Mappings.Any() is false || !_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.DomainServices.Interface, serviceModel, out var serviceInterfaceTemplate))
            {
                return;
            }

            // So that the mapping system can resolve the name of the operation from the interface itself:
            _template.AddTypeSource(serviceInterfaceTemplate.Id);

            string? serviceField = @class.InjectService(_template.GetTypeName(serviceInterfaceTemplate));

            var methodInvocation = _csharpMapping.GenerateCreationStatement(interaction.Mappings.First());
            CSharpStatement invoke = new CSharpAccessMemberStatement(serviceField, methodInvocation);

            var invStatement = methodInvocation as CSharpInvocationStatement;
            if (invStatement?.IsAsyncInvocation() == true)
            {
                invStatement.AddArgument("cancellationToken");
                invoke = new CSharpAwaitExpression(invoke);
            }

            var operationModel = interaction.TypeReference.Element;
            if (operationModel.TypeReference.Element != null)
            {
                var variableName = interaction.Name.ToLocalVariableName();
                _csharpMapping.SetFromReplacement(interaction, variableName);
                _csharpMapping.SetToReplacement(interaction, variableName);

                statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), invoke));

                //TrackedEntities.Add(interaction.Id, new EntityDetails((IElement)operationModel.TypeReference.Element, variableName, null, false, null, operationModel.TypeReference.IsCollection));
            }
            else if (invStatement?.Expression.Reference is ICSharpMethodDeclaration methodDeclaration &&
                     (methodDeclaration.ReturnTypeInfo.GetTaskGenericType() is CSharpTypeTuple || methodDeclaration.ReturnTypeInfo is CSharpTypeTuple))
            {
                var tuple = (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo.GetTaskGenericType() ?? (CSharpTypeTuple)methodDeclaration.ReturnTypeInfo;
                var declaration = new CSharpDeclarationExpression(tuple.Elements.Select(s => s.Name.ToLocalVariableName()).ToList());
                statements.Add(new CSharpAssignmentStatement(declaration, invoke));
            }
            else
            {
                statements.Add(invoke);
            }

            //WireupDomainServicesForOperations(handlerClass, callServiceOperation, statements);

            method.AddStatements(statements);
        }
        catch (Exception ex)
        {
            throw new ElementException(interaction, $"An error occurred while generating the interaction logic: {ex.Message}\nSee inner exception for more details.", ex);
        }
    }
}

[IntentManaged(Mode.Ignore)]
public class CallServiceOperationMappingResolver : IMappingTypeResolver
{
    private readonly ICSharpFileBuilderTemplate _template;

    public CallServiceOperationMappingResolver(ICSharpFileBuilderTemplate template)
    {
        _template = template;
    }

    public ICSharpMapping ResolveMappings(MappingModel mappingModel)
    {
        if (mappingModel.Model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(mappingModel, _template);
        }

        if (mappingModel.Model.SpecializationType == "DTO-Field")
        {
            return new ObjectInitializationMapping(mappingModel, _template);
        }
        return null;
    }
}