using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.UnitTesting.Settings;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using static Intent.Modules.UnitTesting.Settings.UnitTestSettings;

namespace Intent.Modules.UnitTesting.Templates;
internal static class TestHelpers
{
    public static string GetElementNamespace(this CSharpTemplateBase<IElement> template)
    {
        var wrapper = new ElementWrapper(template.Model);
        
        var additionalFolders = wrapper.ParentElement?.Name == null ?
            wrapper.Name.RemoveSuffix("Command", "Query", "Service", "EventHandler") :
            string.Empty;

        var extra = string.Join(".", wrapper.GetParentFolderNames(additionalFolders));
        
        return template.GetNamespace() + (string.IsNullOrWhiteSpace(extra) ? string.Empty : $".{extra}");
    }

    public static string GetTestElementNormalizedPath(this CSharpTemplateBase<IElement> template)
    {
        var wrapper = new ElementWrapper(template.Model);

        var additionalFolders = wrapper.ParentElement?.Name == null ?
            wrapper.Name.RemoveSuffix("Command", "Query", "Service", "EventHandler") :
            string.Empty;

        return string.Join("/", wrapper.GetParentFolderNames(additionalFolders));
    }

    public static string GetOperationNormalizedPath(this CSharpTemplateBase<IElement> template)
    {
        if (template.Model is not IElement model || model.SpecializationTypeId != SpecializationTypeIds.Service)
        {
            return template.GetFolderPath();
        }

        // Wrap the element to get IHasFolder functionality
        var wrapper = new ElementWrapper(model);
        
        // these have to be done as two seperate join calls, otherwise the incorrect overloaded Join method is called
        var parentFolders = wrapper.GetParentFolderNames().ToList();
        if(parentFolders.Count > 0)
        {
            return string.Join("/", parentFolders);
        }
        
        return string.Join("/", model.Name.Replace("Service", string.Empty));
    }

    public static void PopulateTestConstructor(ICSharpTemplate template, CSharpConstructor ctor, ITemplate handlerTemplate, 
        ICSharpFileBuilderTemplate csharpTemplate, SuccessTestDetails details)
    {
        var handlerCtorParams = GetHandlerConstructorParameters(csharpTemplate);

        if (handlerCtorParams.Count != 0)
        {
            ctor.AddStatement($"// Mock the parameters to the {details.SutType} constructor");
        }

        List<string> handleArguments = [];
        var mockFramework = template.ExecutionContext.Settings.GetUnitTestSettings().MockFramework().AsEnum();
        foreach (var handlerParam in handlerCtorParams)
        {
            var mockInitStatement = GetMockInstantiation(mockFramework, template, handlerParam.Type);
            ctor.AddStatement($"// _{handlerParam.Name}Mock = {mockInitStatement}");
            handleArguments.Add(GetMockParameterName(mockFramework, $"_{handlerParam.Name}Mock"));
        }

        if (handlerCtorParams.Count != 0)
        {
            ctor.AddStatement("");
        }

        var ctorParams = string.Join(", ", handleArguments);
        ctor.AddStatement($"// {details.VariableName} = new {template.GetTypeName(handlerTemplate)}({ctorParams});");
    }

    public static void AddDefaultSuccessTest(ICSharpTemplate template, CSharpClass @class, SuccessTestDetails details)
    {
        var methodName = details.MethodName;

        if (details.AssociationEnd != null && @class.Methods.All(m => m.Name != methodName))
        {
            @class.AddMethod(template.UseType("System.Threading.Tasks.Task"), methodName, method =>
            {
                method.AddAttribute(template.UseType("Xunit.Fact"));
                method.AddAttribute(template.UseType("Intent.RoslynWeaver.Attributes.IntentInitialGen"));
                method.Async();

                method.AddStatement("// Arrange");
                method.AddStatement($"// Create an instance of the {details.ArrangeType} here with relevant data for the test");
                method.AddStatement("");

                method.AddStatement("// Act");
                method.AddStatement($"// Invoke the {details.ActMethod}");
                method.AddStatement("");

                method.AddStatement("// Assert");
                method.AddStatement("// Check the outcomes of the test");
                method.AddStatement($"Assert.Fail($\"Implement unit test logic for test '{{nameof({methodName})}}'\");");
                method.AddStatement("");

                method.AddReturn("");
            });
        }
    }
    
    public record SuccessTestDetails(IAssociationEnd AssociationEnd, string MethodName, string ArrangeType, string ActMethod, string SutType, string VariableName)
    {
        public static SuccessTestDetails CreateCommandDetails(IElement model)
        {
            var association = model.AssociatedElements?.FirstOrDefault();
            
            var entityName = association?.TypeReference?.Element?.Name ?? "Entity";
            var action = GetAssociationAction(association);
            var actionVerb = GetActionVerb(action);
            var querySuffix = GetQuerySuffix(action, model);
            var methodName = $"Handle_{actionVerb}{entityName}{querySuffix}_Successfully";

            return new SuccessTestDetails(association, methodName, "command", "Handle method", "Handler", "_handler");
        }
        
        public static SuccessTestDetails CreateQueryDetails(IElement model)
        {
            var association = model.AssociatedElements?.FirstOrDefault();
            
            var entityName = association?.TypeReference?.Element?.Name ?? "Entity";
            var action = GetAssociationAction(association);
            var actionVerb = GetActionVerb(action);
            var querySuffix = GetQuerySuffix(action, model);
            var methodName = $"Handle_{actionVerb}{entityName}{querySuffix}_Successfully";

            return new SuccessTestDetails(association, methodName, "query", "Handle method", "Handler", "_handler");
        }

        public static SuccessTestDetails CreateIntegrationEventDetails(IElement model)
        {
            var association = model.AssociatedElements?.FirstOrDefault();
            
            var entityName = association?.TypeReference?.Element?.Name ?? "Message";
            var action = GetAssociationAction(association);
            var actionVerb = GetActionVerb(action);
            var querySuffix = GetQuerySuffix(action, model);
            var methodName = $"HandleAsync_{actionVerb}{entityName}{querySuffix}_Successfully";

            return new SuccessTestDetails(association, methodName, "event message", "HandleAsync method", "Handler", "_handler");
        }

        public static SuccessTestDetails CreateDomainEventDetails(IElement model)
        {
            var association = model.AssociatedElements?.FirstOrDefault();
            
            var entityName = association?.TypeReference?.Element?.Name ?? "DomainEvent";
            var action = GetAssociationAction(association);
            var actionVerb = GetActionVerb(action);
            var querySuffix = GetQuerySuffix(action, model);
            var methodName = $"Handle_{actionVerb}{entityName}{querySuffix}_Successfully";

            return new SuccessTestDetails(association, methodName, "domain event", "Handle method", "Handler", "_handler");
        }

        public static SuccessTestDetails CreateServiceDetails(IElement model)
        {
            var association = model.AssociatedElements?.FirstOrDefault();
            
            var entityName = association?.TypeReference?.Element?.Name ?? "Entity";
            var action = GetAssociationAction(association);
            var actionVerb = GetActionVerb(action);
            var querySuffix = GetQuerySuffix(action, model);
            var methodName = $"{model.Name}_{actionVerb}{entityName}{querySuffix}_Successfully";

            return new SuccessTestDetails(association, methodName, "service operation parameter(s)", "relevant service method", "Service", "_service");
        }
        
        public static SuccessTestDetails CreateDomainServiceDetails(IElement model)
        {
            var association = model.AssociatedElements?.FirstOrDefault();
            
            var entityName = association?.TypeReference?.Element?.Name ?? "Entity";
            var action = GetAssociationAction(association);
            var actionVerb = GetActionVerb(action);
            var querySuffix = GetQuerySuffix(action, model);
            var methodName = $"{model.Name}_{actionVerb}{entityName}{querySuffix}_Successfully";

            return new SuccessTestDetails(association, methodName, "domain service operation parameter(s)", "relevant service method", "Service", "_service");
        }
    }

    public static INugetPackageInfo GetMockFramework(MockFrameworkOptionsEnum mockFramework, IOutputTarget outputTarget) => mockFramework switch
    {
        MockFrameworkOptionsEnum.Nsubstitute => NugetPackages.NSubstitute(outputTarget),
        _ => NugetPackages.Moq(outputTarget),
    };

    private static string GetAssociationAction(IAssociationEnd association) => association?.SpecializationTypeId switch
    {
        "328f54e5-7bad-4b5f-90ca-03ce3105d016" => "Create",
        "4a04cfc2-5841-438c-9c16-fb58b784b365" => "Delete",
        "516069f6-09cc-4de8-8e31-3c71ca823452" => "Update",
        "93ef6675-cba4-4998-adff-cb22d5343ed4" => "Query",
        _ => "Perform_Action_On"
    };

    private static string GetActionVerb(string action) => action switch
    {
        "Create" => "Creates",
        "Delete" => "Deletes",
        "Update" => "Updates",
        "Query" => "Returns",
        _ => "ProcessesActionOn"
    };

    private static string GetQuerySuffix(string associationAction, IElement model)
    {
        // if not a query, then no suffix
        if(associationAction != "Query")
        {
            return string.Empty;
        }

        // if its an operation AND it has parameters
        if(model.SpecializationTypeId == SpecializationTypeIds.ServiceOperation)
        {
            var parameters = model.GetParameters();
            if(parameters.Any())
            {
                var topParams = parameters.Take(3).Select(p => p.Name.ToPascalCase());
                return $"_By{string.Join("", topParams)}";
            }
        }

        // if its a query AND it has properties
        if (model.SpecializationTypeId == SpecializationTypeIds.Query)
        {
            var properties = model.GetProperties();
            if(properties.Any())
            {
                var topParams = properties.Take(3).Select(p => p.Name.ToPascalCase());
                return $"_By{string.Join("", topParams)}";
            }
        }

        return string.Empty;
    }

    private static List<CSharpConstructorParameter> GetHandlerConstructorParameters(ICSharpFileBuilderTemplate csharpTemplate)
    {
        var @class = csharpTemplate.CSharpFile.Classes?.FirstOrDefault();

        if (@class is null)
        {
            return [];
        }

        // get the parameter with the most arguments
        var ctor = @class.Constructors.OrderByDescending(c => c.Parameters.Count()).FirstOrDefault();

        if (ctor is null)
        {
            return [];
        }

        return [.. ctor.Parameters];
    }

    private static string GetMockInstantiation(MockFrameworkOptionsEnum mockFramework, ICSharpTemplate template, string typeName) => mockFramework switch
    {
        MockFrameworkOptionsEnum.Nsubstitute => $"{template.UseType("NSubstitute.Substitute")}.For<{typeName}>();",
        _ => $"new {template.UseType("Moq.Mock")}<{template.UseType(typeName)}>();"
    };

    private static string GetMockParameterName(MockFrameworkOptionsEnum mockFramework, string parameterName) => mockFramework switch
    {
        MockFrameworkOptionsEnum.Nsubstitute => parameterName,
        _ => $"{parameterName}.Object"
    };

}
