using System;
using System.Collections.Generic;
using System.Linq;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates;

internal class StoredProcedureHelpers
{
    public static void ApplyInterfaceMethods(
        ICSharpFileBuilderTemplate template,
        IReadOnlyCollection<StoredProcedureModel> storedProcedures)
    {
        template.AddTypeSource(TemplateFulfillingRoles.Domain.Enum);
        template.AddTypeSource(TemplateFulfillingRoles.Domain.DataContract);

        template.CSharpFile
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AfterBuild(file =>
            {
                var @interface = file.Interfaces.First();

                foreach (var storedProcedure in storedProcedures)
                {
                    var returnType = storedProcedure.TypeReference.Element != null
                        ? $"Task<{template.GetTypeName(storedProcedure.TypeReference, "System.Collections.Generic.IReadOnlyCollection<{0}>")}>"
                        : "Task";

                    @interface.AddMethod(returnType, storedProcedure.Name.ToPascalCase(), method =>
                    {
                        method.TryAddXmlDocComments(storedProcedure.InternalElement);

                        foreach (var parameter in storedProcedure.Parameters)
                        {
                            method.AddParameter(template.GetTypeName(parameter.TypeReference),
                                parameter.Name.ToLocalVariableName());
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", m => m.WithDefaultValue("default"));
                    });
                }
            });
    }

    public static void ApplyImplementationMethods(
        ICSharpFileBuilderTemplate template,
        IReadOnlyCollection<StoredProcedureModel> storedProcedures)
    {
        template.AddTypeSource(TemplateFulfillingRoles.Domain.Enum);
        template.AddTypeSource(TemplateFulfillingRoles.Domain.DataContract);

        template.CSharpFile
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var dbContextTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.Data.DbContext);
                var dbSetPropertiesByModelId = dbContextTemplate.CSharpFile.Classes.Single().Properties
                    .Where(x => x.HasMetadata("model"))
                    .ToDictionary(x => x.GetMetadata("model") switch
                    {
                        ClassModel model => model.Id,
                        DataContractModel model => model.Id,
                        _ => throw new Exception($"Unknown type: {x.GetMetadata("model").GetType()}")
                    });

                foreach (var storedProcedure in storedProcedures)
                {
                    var returnTypeElement = storedProcedure.TypeReference.Element;

                    var returnType = returnTypeElement != null
                        ? $"Task<{template.GetTypeName(storedProcedure.TypeReference, "System.Collections.Generic.IReadOnlyCollection<{0}>")}>"
                        : "Task";

                    @class.AddMethod(returnType, storedProcedure.Name.ToPascalCase(), method =>
                    {
                        method.Async();

                        foreach (var parameter in storedProcedure.Parameters)
                        {
                            method.AddParameter(template.GetTypeName(parameter.TypeReference),
                                parameter.Name.ToLocalVariableName());
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", m => m.WithDefaultValue("default"));

                        var nameInSchema = storedProcedure.GetStoredProcedureSettings()?.NameInSchema();
                        var spName = !string.IsNullOrWhiteSpace(nameInSchema)
                            ? nameInSchema
                            : storedProcedure.Name;

                        var parameters = storedProcedure.Parameters
                            .Select(x => $" {{{x.Name.ToLocalVariableName()}}}");
                        var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters)}\"";

                        if (returnTypeElement == null)
                        {
                            file.AddUsing("Microsoft.EntityFrameworkCore");
                            method.AddMethodChainStatement($"await _dbContext.Database.ExecuteSqlInterpolatedAsync({sql}, cancellationToken);");
                            return;
                        }

                        var source =
                            template is RepositoryTemplate repositoryTemplate &&
                            repositoryTemplate.Model.Id == returnTypeElement.Id
                                ? "GetSet()"
                                : $"_dbContext.{dbSetPropertiesByModelId[returnTypeElement.Id].Name}";

                        var isCollection = storedProcedure.TypeReference.IsCollection;
                        method.AddMethodChainStatement($"return {(!isCollection ? "(" : string.Empty)}await {source}", mcs =>
                        {
                            mcs
                                .AddChainStatement($"FromSqlInterpolated({sql})")
                                .AddChainStatement("IgnoreQueryFilters()")
                                .AddChainStatement($"ToArrayAsync(cancellationToken){(!isCollection ? ")" : string.Empty)}");

                            if (!isCollection)
                            {
                                mcs.AddChainStatement(storedProcedure.TypeReference.IsNullable
                                    ? "SingleOrDefault()"
                                    : "Single()");
                            }
                        });
                    });
                }
            });
    }

    public static IReadOnlyCollection<StoredProcedureModel> GetStoredProcedureModels(RepositoryModel repositoryModel)
    {
        return repositoryModel.InternalElement.ChildElements
            .Where(x => x.IsStoredProcedureModel())
            .Select(x => x.AsStoredProcedureModel())
            .ToArray();
    }
}