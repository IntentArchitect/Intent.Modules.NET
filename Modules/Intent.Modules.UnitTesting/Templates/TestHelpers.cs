using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.UnitTesting.Settings;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.Constants.TemplateRoles.Application;
using static Intent.Modules.UnitTesting.Settings.UnitTestSettings;

namespace Intent.Modules.UnitTesting.Templates;
internal static class TestHelpers
{
    public static string GetCommandQueryNormalizedPath<TModel>(this CSharpTemplateBase<TModel> template)
    {
        var model = template.Model as IElementWrapper;

        var additionalFolders = model?.InternalElement?.ParentElement?.Name == null ?
            model.InternalElement.Name.RemoveSuffix("Command").RemoveSuffix("Query").RemoveSuffix("Service") :
            string.Empty;

        return template.GetFolderPath(additionalFolders);
    }

    public static string GetOperationNormalizedPath<TModel>(this CSharpTemplateBase<TModel> template)
    {
        var operationModel = template.Model as Modelers.Services.Api.OperationModel;
        var parentService = operationModel?.ParentService;

        if(parentService != null)
        {
            return string.Join("/", parentService.GetParentFolderNames());
        }

        return template.GetFolderPath();
    }

    public static void PopulateTestConstructor(ICSharpTemplate template, CSharpConstructor ctor, ITemplate handlerTemplate, ICSharpFileBuilderTemplate csharpTemplate,
        bool isCQRS = true)
    {
        var handlerCtorParams = GetHandlerConstructorParameters(csharpTemplate);

        if (handlerCtorParams.Count != 0)
        {
            ctor.AddStatement($"// Mock the parameters to the {(isCQRS ? "Handler" : "Service")} constructor");
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
        ctor.AddStatement($"// {(isCQRS ? "_handler" : "_service")} = new {template.GetTypeName(handlerTemplate)}({ctorParams});");
    }

    public static void AddDefaultSuccessTest(ICSharpTemplate template, IElementWrapper model, CSharpClass @class, bool isCQRS = true)
    {
        var association = model.InternalElement.AssociatedElements?.FirstOrDefault();

        var entityName = association?.TypeReference?.Element?.Name == null ? "Entity" : association.TypeReference.Element.Name;
        var action = GetAssociationAction(association);
        var methodName = $"{(isCQRS ? "Handle" : "Operation")}_Should_{action}_{entityName}_Successfully";

        if (association != null)
        {
            @class.AddMethod(template.UseType("System.Threading.Tasks.Task"), methodName, method =>
            {
                method.AddAttribute(template.UseType("Xunit.Fact"));
                method.AddAttribute(template.UseType("Intent.RoslynWeaver.Attributes.IntentInitialGen"));
                method.Async();

                method.AddStatement("// Arrange");
                method.AddStatement($"// Create an instance of the {(isCQRS ? "command/query" : "service operation parameter(s)")} here with relevant data for the test");
                method.AddStatement("");

                method.AddStatement("// Act");
                method.AddStatement($"// Invoke the {(isCQRS ? "Handle method" : "relevant service method")}");
                method.AddStatement("");

                method.AddStatement("// Assert");
                method.AddStatement("// Check the outcomes of the test");
                method.AddStatement($"Assert.Fail($\"Implement unit test logic for test '{{nameof({methodName})}}'\");");
                method.AddStatement("");

                method.AddReturn("");
            });
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
