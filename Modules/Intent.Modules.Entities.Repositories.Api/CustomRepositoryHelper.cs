using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Entities.Repositories.Api;
public static class CustomRepositoryHelper
{
    public static void ApplyInterfaceMethods<TTemplate, TModel>(
    TTemplate template,
    RepositoryModel repositoryModel)
    where TTemplate : CSharpTemplateBase<TModel>, ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);
        template.AddTypeSource(TemplateRoles.Domain.ValueObject);

        template.CSharpFile
            .OnBuild(file =>
            {
                var @interface = file.Interfaces.First();

                foreach (var childElement in repositoryModel.InternalElement.ChildElements)
                {
                    var operationModel = OperationModelExtensions.AsOperationModel(childElement);
                    if (operationModel != null)
                    {
                        var returnType = operationModel.ReturnType is null ? "void" : template.GetTypeName(operationModel.ReturnType, "System.Collections.Generic.List<{0}>");
                        @interface.AddMethod(returnType, operationModel.Name.ToPascalCase(), method =>
                        {
                            method.TryAddXmlDocComments(childElement);
                            method.AddMetadata("model", operationModel);
                            method.RepresentsModel(operationModel);

                            foreach (var parameterModel in operationModel.Parameters)
                            {
                                method.AddParameter(template.GetTypeName(parameterModel.TypeReference), parameterModel.Name);
                            }

                            var isAsync = operationModel.Name.EndsWith("Async") || operationModel.HasStereotype("Asynchronous");

                            if (isAsync)
                            {
                                template.CSharpFile.AddUsing("System.Threading");
                                template.CSharpFile.AddUsing("System.Threading.Tasks");
                                method.Async();
                                method.AddOptionalCancellationTokenParameter();
                            }
                        });

                        continue;
                    }
                }
            }, 1);
    }

    /// <summary>
    /// Apply any custom repository methods to the first class of the provided <paramref name="template"/>.
    /// </summary>
    public static void ApplyImplementationMethods<TTemplate, TModel>(
        TTemplate template,
        RepositoryModel repositoryModel)
        where TTemplate : ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);
        template.AddTypeSource(TemplateRoles.Domain.ValueObject);

        var @class = template.CSharpFile.Classes.First();

        // at the end, on all empty methods
        template.CSharpFile.AfterBuild(file =>
        {
            var @class = template.CSharpFile.Classes.First();

            foreach (var method in @class.Methods)
            {
                if (!method.Statements.Any())
                {
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());
                    method.AddStatement($"// TODO: Implement {method.Name} ({template.CSharpFile.Classes.First().Name}) functionality");
                    method.AddStatement($"""throw new {template.UseType("System.NotImplementedException")}("Your implementation here...");""");
                }
            }
        }, 1000);

        foreach (var childElement in repositoryModel.InternalElement.ChildElements)
        {
            var operationModel = OperationModelExtensions.AsOperationModel(childElement);
            if (operationModel != null)
            {
                var returnType = operationModel.ReturnType is null ? "void" : template.GetFullyQualifiedTypeName(operationModel.ReturnType, "System.Collections.Generic.List<{0}>");
                @class.AddMethod(template.UseType(returnType), operationModel.Name.ToPascalCase(), method =>
                {
                    method.AddMetadata("model", operationModel);
                    method.RepresentsModel(operationModel);

                    foreach (var parameterModel in operationModel.Parameters)
                    {
                        method.AddParameter(template.GetTypeName(parameterModel.TypeReference), parameterModel.Name);
                    }

                    var isAsync = operationModel.Name.EndsWith("Async") || operationModel.HasStereotype("Asynchronous");

                    if (isAsync)
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter();
                    }
                });

                continue;
            }
        }
    }
}
