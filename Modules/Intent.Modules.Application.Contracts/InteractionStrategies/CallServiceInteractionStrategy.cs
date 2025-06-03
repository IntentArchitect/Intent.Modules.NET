using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Exceptions;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.InteractionStrategies
{
    public class CallServiceInteractionStrategy : IInteractionStrategy
    {
        private ICSharpFileBuilderTemplate _template;
        private CSharpClassMappingManager _csharpMapping;

        public bool IsMatch(IAssociationEnd interaction)
        {
            return interaction.IsServiceInvocationTargetEndModel() && interaction.TypeReference.Element.IsOperationModel();
        }

        public IEnumerable<CSharpStatement> GetStatements(CSharpClass handlerClass, IAssociationEnd interaction, CSharpClassMappingManager csharpMapping)
        {
            _template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;
            _csharpMapping = csharpMapping;
            try
            {
                var statements = new List<CSharpStatement>();
                var serviceModel = ((IElement)interaction.TypeReference.Element).ParentElement;
                if (interaction.Mappings.Any() is false || !_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.Interface, serviceModel, out var serviceInterfaceTemplate))
                {
                    return Array.Empty<CSharpStatement>();
                }

                // So that the mapping system can resolve the name of the operation from the interface itself:
                _template.AddTypeSource(serviceInterfaceTemplate.Id);

                string? serviceField = handlerClass.InjectService(_template.GetTypeName(serviceInterfaceTemplate));

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

                return statements;
            }
            catch (Exception ex)
            {
                throw new ElementException(interaction, $"An error occurred while generating the interaction logic: {ex.Message}\nSee inner exception for more details.", ex);
            }
        }
    }
}
