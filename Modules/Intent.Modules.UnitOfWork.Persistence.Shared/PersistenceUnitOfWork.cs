#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.UnitOfWork.Persistence.Shared;

internal static class PersistenceUnitOfWork
{
    private static readonly TemplateDiscoveryOptions TemplateDiscoveryOptions = new() { TrackDependency = false, ThrowIfNotFound = false };
    private delegate void Next(Stack<IHasCSharpStatements> blockStack);
    private delegate void ChainOfResponsibility(Stack<IHasCSharpStatements> blockStack, Next next);

    public static bool SystemUsesPersistenceUnitOfWork(this ICSharpFileBuilderTemplate template)
    {
        return
            template.GetTemplate<ICSharpTemplate>(TemplateRoles.Infrastructure.Data.DbContext, TemplateDiscoveryOptions) != null ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.CosmosDBUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DaprStateStoreUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.RedisOmUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DistributedCacheWithUnitOfWorkInterface, out _);
    }

    private static void SeparatedFromPrevious(CSharpStatement statement)
    {
        var index = statement.Parent.Statements.IndexOf(statement);
        if (index == 0 || statement.Parent.Statements[index - 1] is not IHasCSharpStatements)
        {
            return;
        }

        statement.SeparatedFromPrevious();
    }

    public static void ApplyUnitOfWorkImplementations(
        this IHasCSharpStatements block,
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        CSharpStatement invocationStatement,
        string? returnType = null,
        string resultVariableName = "result",
        string cancellationTokenExpression = "cancellationToken",
        string fieldSuffix = "unitOfWork",
        bool allowTransactionScope = true,
        bool includeComments = true)
    {
        //This is for the ArgumentNullException
        template.AddUsing("System");

        var chainOfResponsibilities = new Stack<ChainOfResponsibility>();

        if (template.TryGetTemplate<ICSharpTemplate>(TemplateIds.CosmosDBUnitOfWorkInterface, out _))
        {
            var cosmosDbParameterName = $"cosmosDB{fieldSuffix.ToPascalCase()}";

            if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                    type: template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface),
                    name: cosmosDbParameterName,
                    configure: param => param.IntroduceReadonlyField((_, statement) =>
                    {
                        statement.ThrowArgumentNullException();
                    }));
            }

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();

                next(blockStack);
                currentBlock.AddStatement($"await _{cosmosDbParameterName}.SaveChangesAsync({cancellationTokenExpression});", SeparatedFromPrevious);
            });
        }

        if (template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DaprStateStoreUnitOfWorkInterface, out _))
        {
            var daprParameterName = $"daprStateStore{fieldSuffix.ToPascalCase()}";

            if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                    template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface),
                    daprParameterName,
                    param => param.IntroduceReadonlyField((_, statement) =>
                    {
                        statement.ThrowArgumentNullException();
                    }));
            }

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();

                next(blockStack);
                currentBlock.AddStatement($"await _{daprParameterName}.SaveChangesAsync( {cancellationTokenExpression} );", SeparatedFromPrevious);
            });
        }

        if (template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out _))
        {
            var mongoDbParameterName = $"mongoDb{fieldSuffix.ToPascalCase()}";

            if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                    template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface),
                    mongoDbParameterName,
                    param => param.IntroduceReadonlyField((_, statement) =>
                    {
                        statement.ThrowArgumentNullException();
                    }));
            }

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();

                next(blockStack);
                currentBlock.AddStatement($"await _{mongoDbParameterName}.SaveChangesAsync({cancellationTokenExpression});", SeparatedFromPrevious);
            });
        }

        if (template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _))
        {
            var tableStorageParameterName = $"tableStorage{fieldSuffix.ToPascalCase()}";

            if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface)))
            {
                constructor.AddParameter(
                    type: template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface),
                    name: tableStorageParameterName,
                    configure: param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
            }

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();

                next(blockStack);
                currentBlock.AddStatement($"await _{tableStorageParameterName}.SaveChangesAsync({cancellationTokenExpression});", SeparatedFromPrevious);
            });
        }

        if (template.TryGetTemplate<ICSharpTemplate>(TemplateIds.RedisOmUnitOfWorkInterface, out _))
        {
            var redisOmParameterName = $"redisOm{fieldSuffix.ToPascalCase()}";

            constructor.AddParameter(
                template.GetTypeName(TemplateIds.RedisOmUnitOfWorkInterface),
                redisOmParameterName,
                c => c.IntroduceReadonlyField());

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();

                next(blockStack);
                currentBlock.AddStatement($"await _{redisOmParameterName}.SaveChangesAsync({cancellationTokenExpression});", SeparatedFromPrevious);
            });
        }

        if (template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DistributedCacheWithUnitOfWorkInterface, out _))
        {
            const string distributedCacheParameterName = "distributedCacheWithUnitOfWork";

            if (constructor.Parameters.All(x => x.Name != distributedCacheParameterName))
            {
                constructor.AddParameter(
                    type: template.GetTypeName(TemplateIds.DistributedCacheWithUnitOfWorkInterface),
                    name: distributedCacheParameterName,
                    configure: param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
            }

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();

                currentBlock.AddUsingBlock("_distributedCacheWithUnitOfWork.EnableUnitOfWork()", @using =>
                {
                    blockStack.Push(@using);
                    next(blockStack);
                    @using.AddStatement($"await _{distributedCacheParameterName}.SaveChangesAsync({cancellationTokenExpression});", SeparatedFromPrevious);
                });
            });
        }

        if (template.GetTemplate<ICSharpTemplate>(TemplateRoles.Infrastructure.Data.DbContext, TemplateDiscoveryOptions) != null)
        {
            var efParameterName = fieldSuffix;

            var typeName = template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out var unitOfWork)
                ? unitOfWork
                : template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out unitOfWork)
                    ? unitOfWork
                    : template.TryGetTypeName(TemplateRoles.Infrastructure.Data.DbContext, out unitOfWork)
                        ? unitOfWork
                        : throw new Exception("A Unit of Work interface could not be resolved. Please contact Intent Architect support.");

            if (constructor.Parameters.All(p => p.Type != typeName))
            {
                constructor.AddParameter(typeName,
                    efParameterName,
                    param => param.IntroduceReadonlyField((_, statement) =>
                    {
                        statement.ThrowArgumentNullException();
                    }));
            }

            chainOfResponsibilities.Push((blockStack, next) =>
            {
                var currentBlock = blockStack.Peek();
                if (allowTransactionScope && includeComments)
                {
                    currentBlock.AddStatements(new[]
                    {
                        new CSharpStatement("// The execution is wrapped in a transaction scope to ensure that if any other").SeparatedFromPrevious(),
                        "// SaveChanges calls to the data source (e.g. EF Core) are called, that they are",
                        "// transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-",
                        "// locks are released, while write-locks are maintained for the duration of the",
                        "// transaction). Learn more on this approach for EF Core:",
                        "// https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions"
                    });
                }

                if (allowTransactionScope)
                {
                    var transactionScope = template.UseType("System.Transactions.TransactionScope");
                    var transactionScopeOption = template.UseType("System.Transactions.TransactionScopeOption");
                    var transactionOptions = template.UseType("System.Transactions.TransactionOptions");
                    var isolationLevel = template.UseType("System.Transactions.IsolationLevel");
                    var transactionScopeAsyncFlowOption = template.UseType("System.Transactions.TransactionScopeAsyncFlowOption");

                    currentBlock.AddUsingBlock(@$"var transaction = new {transactionScope}({transactionScopeOption}.Required,
                new {transactionOptions} {{ IsolationLevel = {isolationLevel}.ReadCommitted }}, {transactionScopeAsyncFlowOption}.Enabled)",
                        @using =>
                        {
                            currentBlock = @using;
                            blockStack.Push(@using);
                            @using.BeforeSeparator = includeComments ? CSharpCodeSeparatorType.None : CSharpCodeSeparatorType.EmptyLines;
                            @using.AddMetadata("transaction", "using-block");
                        });
                }

                next(blockStack);

                if (allowTransactionScope && includeComments) currentBlock.AddStatements(new[]
                {
                    new CSharpStatement("// By calling SaveChanges at the last point in the transaction ensures that write-").SeparatedFromPrevious(),
                    "// locks in the database are created and then released as quickly as possible. This",
                    "// helps optimize the application to handle a higher degree of concurrency."
                });
                currentBlock.AddStatement(
                    $"await _{efParameterName}.SaveChangesAsync({cancellationTokenExpression});",
                    s => s.AddMetadata("transaction", "save-changes"));

                if (allowTransactionScope && includeComments) currentBlock.AddStatements(new[]
                {
                    new CSharpStatement("// Commit transaction if everything succeeds, transaction will auto-rollback when").SeparatedFromPrevious(),
                    "// disposed if anything failed."
                });
                if (allowTransactionScope)
                {
                    currentBlock.AddStatement("transaction.Complete();", s => s.AddMetadata("transaction", "complete"));
                }
            });
        }

        // Ultimate invocation statement:
        var stackAtDeepest = default(Stack<IHasCSharpStatements>)!;
        chainOfResponsibilities.Push((blockStack, _) =>
        {
            blockStack.Peek().AddStatement<IHasCSharpStatements, CSharpStatement>(invocationStatement);
            stackAtDeepest = new Stack<IHasCSharpStatements>(blockStack.Reverse());
        });

        var previousAction = default(Next);
        while (chainOfResponsibilities.TryPop(out var current))
        {
            var capturedCurrent = current;
            var capturedAction = previousAction!;

            previousAction = blockStack => capturedCurrent(blockStack, capturedAction);
        }

        previousAction?.Invoke(new Stack<IHasCSharpStatements>([block]));

        if (returnType != null)
        {
            // Sometimes we need to return the variable in a parent block if work also needs to be
            // done there in which case we need to split the declaration and assignment.

            var deepestBlock = stackAtDeepest.Peek();

            IHasCSharpStatements blockWithReturn;

            while (stackAtDeepest.TryPop(out blockWithReturn))
            {
                if (stackAtDeepest.All(x => x.Statements.Last() is IHasCSharpStatements))
                {
                    break;
                }
            }

            if (blockWithReturn == deepestBlock)
            {
                // Simple case where we don't need to split the variable assignment
                invocationStatement.FindAndReplace(
                    find: invocationStatement.ToString(),
                    replaceWith: $"var {resultVariableName} = {invocationStatement}");
            }
            else
            {
                // We need to split the variable and assignment
                blockWithReturn.InsertStatement(0, new CSharpStatement($"{template.UseType(returnType)} {resultVariableName};"));

                invocationStatement.FindAndReplace(
                    find: invocationStatement.ToString(),
                    replaceWith: $"{resultVariableName} = {invocationStatement}");
            }

            blockWithReturn!.AddStatement<IHasCSharpStatements, CSharpStatement>($"return {resultVariableName};", s => s.SeparatedFromPrevious());
        }
    }
}