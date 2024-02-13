using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.UnitOfWork.Persistence.Shared;

internal static class PersistenceUnitOfWork
{
    private static readonly TemplateDiscoveryOptions TemplateDiscoveryOptions = new() { TrackDependency = false, ThrowIfNotFound = false };

    public static bool SystemUsesPersistenceUnitOfWork(this ICSharpFileBuilderTemplate template)
    {
        return
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.CosmosDBUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DaprStateStoreUnitOfWorkInterface, out _) ||
            template.GetTemplate<ICSharpTemplate>(TemplateRoles.Infrastructure.Data.DbContext, TemplateDiscoveryOptions) != null ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.RedisOmUnitOfWorkInterface, out _);
    }

    public static void ApplyUnitOfWorkImplementations(
        this CSharpClassMethod method,
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        CSharpStatement invocationStatement,
        string returnType = null,
        string resultVariableName = "result",
        string cancellationTokenExpression = "cancellationToken",
        string fieldSuffix = "unitOfWork",
        bool allowTransactionScope = true,
        bool includeComments = true)
    {
        var cosmosDbParameterName = $"cosmosDB{fieldSuffix.ToPascalCase()}";
        var daprParameterName = $"daprStateStore{fieldSuffix.ToPascalCase()}";
        var efParameterName = fieldSuffix;
        var mongoDbParameterName = $"mongoDb{fieldSuffix.ToPascalCase()}";
        var tableStorageParameterName = $"tableStorage{fieldSuffix.ToPascalCase()}";
        var redisOmParameterName = $"redisOm{fieldSuffix.ToPascalCase()}";

        var useTransactionScope = false;
        var useOutsideTransactionScope = false;

        //This is for the ArgumentNullException
        template.AddUsing("System");
        var requiresCosmosDb = template.TryGetTemplate<ICSharpTemplate>(TemplateIds.CosmosDBUnitOfWorkInterface, out _);
        if (requiresCosmosDb)
        {
            if (!constructor.Parameters.Any(p => p.Type == template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface),
                cosmosDbParameterName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
            }

            useOutsideTransactionScope = true;
        }

        var requiresDapr = template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DaprStateStoreUnitOfWorkInterface, out _);
        if (requiresDapr)
        {
            if (!constructor.Parameters.Any(p => p.Type == template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface),
                daprParameterName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
            }

            useOutsideTransactionScope = true;
        }

        var requiresEf = template.GetTemplate<ICSharpTemplate>(TemplateRoles.Infrastructure.Data.DbContext, TemplateDiscoveryOptions) != null;
        if (requiresEf)
        {
            string typeName = template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out var unitOfWork) ? unitOfWork
                    : template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out unitOfWork) ? unitOfWork
                    : template.TryGetTypeName(TemplateRoles.Infrastructure.Data.DbContext, out unitOfWork) ? unitOfWork
                    : throw new Exception("A Unit of Work interface could not be resolved. Please contact Intent Architect support.");

            if (!constructor.Parameters.Any(p => p.Type == typeName))
            {
                constructor.AddParameter(typeName,
                efParameterName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));

            }

            useTransactionScope = allowTransactionScope;
        }

        var requiresMongoDb = template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out _);
        if (requiresMongoDb)
        {
            if (!constructor.Parameters.Any(p => p.Type == template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface),
                mongoDbParameterName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
            }
            useOutsideTransactionScope = true;
        }

        var requiresTableStorage = template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _);
        if (requiresTableStorage)
        {
            if (!constructor.Parameters.Any(p => p.Type == template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface),
                tableStorageParameterName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
            }

            useOutsideTransactionScope = true;
        }

        var requiresRedisOm = template.TryGetTemplate<ICSharpTemplate>(TemplateIds.RedisOmUnitOfWorkInterface, out _);
        if (requiresRedisOm)
        {
            constructor.AddParameter(
                template.GetTypeName(TemplateIds.RedisOmUnitOfWorkInterface),
                redisOmParameterName,
                c => c.IntroduceReadonlyField());

            useOutsideTransactionScope = true;
        }

        var hasSeparateResultDeclaration = useTransactionScope && useOutsideTransactionScope;
        if (hasSeparateResultDeclaration && returnType != null)
        {
            method.AddStatement($"{template.UseType(returnType)} {resultVariableName};");
        }

        var hasCSharpStatements = (IHasCSharpStatements)method;
        if (useTransactionScope)
        {
            var transactionScope = template.UseType("System.Transactions.TransactionScope");
            var transactionScopeOption = template.UseType("System.Transactions.TransactionScopeOption");
            var transactionOptions = template.UseType("System.Transactions.TransactionOptions");
            var isolationLevel = template.UseType("System.Transactions.IsolationLevel");
            var transactionScopeAsyncFlowOption =
                template.UseType("System.Transactions.TransactionScopeAsyncFlowOption");

            if (includeComments)
            { 
                method.AddStatements(new[]
                {
                    new CSharpStatement("// The execution is wrapped in a transaction scope to ensure that if any other").SeparatedFromPrevious(),
                    "// SaveChanges calls to the data source (e.g. EF Core) are called, that they are",
                    "// transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-",
                    "// locks are released, while write-locks are maintained for the duration of the",
                    "// transaction). Learn more on this approach for EF Core:",
                    "// https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions"
                });
            }

            method.AddUsingBlock(@$"var transaction = new {transactionScope}({transactionScopeOption}.Required,
                new {transactionOptions} {{ IsolationLevel = {isolationLevel}.ReadCommitted }}, {transactionScopeAsyncFlowOption}.Enabled)",
                usingBlock =>
                {
                    hasCSharpStatements = usingBlock;
                    usingBlock.BeforeSeparator = CSharpCodeSeparatorType.None;
                    usingBlock.AddMetadata("transaction", "using-block");
                });
        }

        if (returnType != null)
        {
            var varKeyword = hasSeparateResultDeclaration ? string.Empty : "var ";

            invocationStatement.FindAndReplace(
                find: invocationStatement.ToString(),
                replaceWith: $"{varKeyword}{resultVariableName} = {invocationStatement}");
        }

        hasCSharpStatements.AddStatement(invocationStatement);

        if (useTransactionScope && includeComments)
        {
            hasCSharpStatements.AddStatements(new[]
            {
                new CSharpStatement("// By calling SaveChanges at the last point in the transaction ensures that write-").SeparatedFromPrevious(),
                "// locks in the database are created and then released as quickly as possible. This",
                "// helps optimize the application to handle a higher degree of concurrency."
            });
        }

        if (requiresEf)
        {
            hasCSharpStatements.AddStatement(
                $"await _{efParameterName}.SaveChangesAsync({cancellationTokenExpression});",
                s => s.AddMetadata("transaction", "save-changes"));
        }

        if (useTransactionScope)
        {
            if (includeComments)
            {
                hasCSharpStatements.AddStatements(new[]
                {
                    new CSharpStatement("// Commit transaction if everything succeeds, transaction will auto-rollback when").SeparatedFromPrevious(),
                    "// disposed if anything failed."
                });
            }
            hasCSharpStatements.AddStatement(
                "transaction.Complete();",
                s => s.AddMetadata("transaction", "complete"));
        }

        var separatedFromPrevious = default(Action<CSharpStatement>);
        if (useOutsideTransactionScope)
        {
            hasCSharpStatements = method;
            separatedFromPrevious = c => c.SeparatedFromPrevious();
        }

        if (requiresCosmosDb)
        {
            hasCSharpStatements.AddStatement($"await _{cosmosDbParameterName}.SaveChangesAsync({cancellationTokenExpression});", separatedFromPrevious);
            separatedFromPrevious = null;
        }

        if (requiresDapr)
        {
            hasCSharpStatements.AddStatement($"await _{daprParameterName}.SaveChangesAsync( {cancellationTokenExpression} );", separatedFromPrevious);
            separatedFromPrevious = null;
        }

        if (requiresMongoDb)
        {
            hasCSharpStatements.AddStatement($"await _{mongoDbParameterName}.SaveChangesAsync({cancellationTokenExpression});", separatedFromPrevious);
            separatedFromPrevious = null;
        }

        if (requiresTableStorage)
        {
            hasCSharpStatements.AddStatement($"await _{tableStorageParameterName}.SaveChangesAsync({cancellationTokenExpression});", separatedFromPrevious);
            separatedFromPrevious = null;
        }
        
        if (requiresRedisOm)
        {
            hasCSharpStatements.AddStatement($"await _{redisOmParameterName}.SaveChangesAsync({cancellationTokenExpression});", separatedFromPrevious);
            separatedFromPrevious = null;
        }

        if (returnType != null)
        {
            hasCSharpStatements.AddStatement($"return {resultVariableName};", c => c.SeparatedFromPrevious());
        }
    }
}