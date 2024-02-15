using System;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Controllers.Templates;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers;

public static class Utils
{
    public static bool ShouldBeJsonResponseWrapped(this ICSharpTemplate template, IControllerOperationModel operationModel)
    {
        var isWrappedReturnType = operationModel.MediaType == HttpMediaType.ApplicationJson;
        var returnsCollection = operationModel.ReturnType?.IsCollection == true;
        var returnsString = operationModel.ReturnType.HasStringType();
        var returnsPrimitive = template.GetTypeInfo(operationModel.ReturnType).IsPrimitive &&
                               !returnsCollection;

        return isWrappedReturnType && (returnsPrimitive || returnsString);
    }

    public static CSharpStatement GetReturnStatement(this ControllerTemplate template, IControllerOperationModel operationModel)
    {
        if (FileTransferHelper.IsFileDownload( operationModel))
        {
            return @"if (result == null)
            {
                return NotFound();
            }
            return File(result.Stream, result.ContentType, result.Filename );";
        }
        var hasReturnType = operationModel.ReturnType != null;

        var resultExpression = default(string);
        if (hasReturnType)
        {
            resultExpression = template.ShouldBeJsonResponseWrapped(operationModel)
                ? $"new {template.GetJsonResponseName()}<{template.GetTypeName(operationModel.ReturnType)}>(result)"
                : "result";
        }

        string returnExpression;
        switch (operationModel.Verb)
        {
            case HttpVerb.Get:
            case HttpVerb.Patch:
            case HttpVerb.Put:
                returnExpression= hasReturnType ? $"Ok({resultExpression})" : "NoContent()";
                break;
            case HttpVerb.Delete:
                returnExpression = hasReturnType ? $"Ok({resultExpression})" : "Ok()";
                break;
            case HttpVerb.Post:
                switch (operationModel.Parameters.Count)
                {
                    case 1:
                        // Aggregate
                        {
                            var getByIdOperation = template.Model.Operations.FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                x.ReturnType is { IsCollection: false } &&
                                x.Parameters.Count == 1 &&
                                string.Equals(x.Parameters[0].Name, "id", StringComparison.OrdinalIgnoreCase));
                            if (getByIdOperation != null && operationModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                            {
                                returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ id = result }}, {resultExpression})";
                            }
                            else
                            {
                                returnExpression = null;
                            }
                        }
                        break;
                    case 2:
                        // Owned composite
                        {
                            var getByIdOperation = template.Model.Operations.FirstOrDefault(x => x.Verb == HttpVerb.Get &&
                                x.ReturnType is { IsCollection: false } &&
                                x.Parameters.Count == 2 &&
                                string.Equals(x.Parameters[0].Name, operationModel.Parameters[0].Name, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(x.Parameters[1].Name, "id", StringComparison.OrdinalIgnoreCase));
                            if (getByIdOperation != null && operationModel.ReturnType?.Element.Name is "guid" or "long" or "int" or "string")
                            {
                                var aggregateIdParameter = getByIdOperation.Parameters[0].Name.ToCamelCase();
                                returnExpression = $"CreatedAtAction(nameof({getByIdOperation.Name}), new {{ {aggregateIdParameter} = {aggregateIdParameter}, id = result }}, {resultExpression})";
                            }
                            else
                            {
                                returnExpression = null;
                            }
                        }
                        break;
                    default:
                        returnExpression = null;
                        break;
                }

                returnExpression ??= hasReturnType ? $"Created(string.Empty, {resultExpression})" : "Created(string.Empty, null)";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (operationModel.ReturnType != null &&
            operationModel.CanReturnNotFound() &&
            !template.GetTypeInfo(operationModel.ReturnType).IsPrimitive)
        {
            returnExpression = $"result == null ? NotFound() : {returnExpression}";
        }

        return $"return {returnExpression};";
    }

    public static bool CanReturnNotFound(this IControllerOperationModel operation)
    {
        if (operation.ReturnType?.IsNullable == true)
        {
            return false;
        }

        if (operation.Verb == HttpVerb.Get &&
            operation.ReturnType?.IsCollection != true &&
            !CSharpTypeIsCollection(operation.ReturnType) &&
            operation.Parameters.Any())
        {
            return true;
        }

        return operation.Parameters.Any(x => x.Name.EndsWith("id", StringComparison.OrdinalIgnoreCase) ||
                                             x.Name.StartsWith("id", StringComparison.OrdinalIgnoreCase));

        static bool CSharpTypeIsCollection(ITypeReference typeReference)
        {
            if (typeReference?.Element is not IElement element)
            {
                return false;
            }

            var stereotype = element.GetStereotype("C#");
            if (stereotype == null)
            {
                return false;
            }

            return stereotype.GetProperty<bool>("Is Collection");
        }
    }

    private static bool IsNonReferenceType(this ITypeReference typeReference)
    {
        if (typeReference?.Element is not IElement element)
        {
            return false;
        }

        var stereotype = element.GetStereotype("C#");
        if (stereotype == null)
        {
            return false;
        }

        return stereotype.GetProperty<bool>("Is Collection");
    }
}