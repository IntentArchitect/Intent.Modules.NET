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
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DynamoDBUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DaprStateStoreUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbMongoFrameworkUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.RedisOmUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _) ||
            template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DistributedCacheWithUnitOfWorkInterface, out _);
    }

    internal const string ChainEntityFramework = "ef";
    internal const string ChainCosmosDb = "cosmosdb";
    internal const string ChainDynamoDb = "dynamodb";
    internal const string ChainMongoDb = "mongodb";
    internal const string ChainDapr = "dapr";
    internal const string ChainRedisOm = "redisom";
    internal const string ChainTableStorage = "tablestorage";
    internal const string ChainDistributedCache = "distributedcache";

    public static void ApplyUnitOfWorkImplementations(
        this IHasCSharpStatements block,
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        CSharpStatement invocationStatement,
        Action<UnitOfWorkConfiguration> configure)
    {
        // This is for the ArgumentNullException
        template.AddUsing("System");

        var config = new UnitOfWorkConfiguration();
        configure(config);

        // Validation: If using service provider resolution, ensure service provider variable name is provided
        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider &&
            string.IsNullOrWhiteSpace(config.ServiceProviderVariableName))
        {
            throw new ArgumentException("ServiceProviderVariableName must be provided when using ServiceProvider resolution strategy.");
        }

        var chainOfResponsibilities = new Stack<ChainOfResponsibility>();

        // Auto-detect and add providers to chain
        AddCosmosDbToChain(template, constructor, config, chainOfResponsibilities);
        AddDynamoDbToChain(template, constructor, config, chainOfResponsibilities);
        AddDaprStateStoreToChain(template, constructor, config, chainOfResponsibilities);
        AddMongoDbToChain(template, constructor, config, chainOfResponsibilities);
        AddTableStorageToChain(template, constructor, config, chainOfResponsibilities);
        AddRedisOmToChain(template, constructor, config, chainOfResponsibilities);
        AddDistributedCacheToChain(template, constructor, config, chainOfResponsibilities);
        AddEntityFrameworkToChain(template, constructor, config, chainOfResponsibilities);

        ExecuteChainOfResponsibilities(
            block,
            template,
            invocationStatement,
            config,
            chainOfResponsibilities);
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
        block.ApplyUnitOfWorkImplementations(
            template,
            constructor,
            invocationStatement,
            config =>
            {
                if (returnType != null)
                {
                    config.WithReturnType(returnType);
                }

                config.WithResultVariableName(resultVariableName)
                      .UseCancellationToken(cancellationTokenExpression)
                      .WithFieldSuffix(fieldSuffix)
                      .WithTransactionScope(allowTransactionScope)
                      .WithComments(includeComments);

                // Apply variable overrides only if fieldSuffix differs from default
                if (fieldSuffix == "unitOfWork")
                {
                    return;
                }

                var efVariable = fieldSuffix;
                var cosmosVariable = $"cosmosDB{fieldSuffix.ToPascalCase()}";
                var daprVariable = $"daprStateStore{fieldSuffix.ToPascalCase()}";
                var mongoVariable = $"mongoDb{fieldSuffix.ToPascalCase()}";
                var tableStorageVariable = $"tableStorage{fieldSuffix.ToPascalCase()}";
                var redisVariable = $"redisOm{fieldSuffix.ToPascalCase()}";

                config.UseEntityFrameworkVariable(efVariable)
                    .UseCosmosDbVariable(cosmosVariable)
                    .UseDaprStateStoreVariable(daprVariable)
                    .UseMongoDbVariable(mongoVariable)
                    .UseTableStorageVariable(tableStorageVariable)
                    .UseRedisOmVariable(redisVariable);
            });
    }

    private static void AddCosmosDbToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.CosmosDBUnitOfWorkInterface, out _))
            return;

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainCosmosDb, "cosmosDBUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface)))
        {
            constructor.AddParameter(
                type: template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface),
                name: variableName,
                configure: param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.CosmosDBUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            next(blockStack);

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
        });
    }

    private static void AddDynamoDbToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DynamoDBUnitOfWorkInterface, out _))
        {
            return;
        }

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainDynamoDb, "dynamoDBUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.DynamoDBUnitOfWorkInterface)))
        {
            constructor.AddParameter(
                type: template.GetTypeName(TemplateIds.DynamoDBUnitOfWorkInterface),
                name: variableName,
                configure: param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.DynamoDBUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            next(blockStack);

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
        });
    }

    private static void AddDaprStateStoreToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DaprStateStoreUnitOfWorkInterface, out _))
            return;

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainDapr, "daprStateStoreUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface)))
        {
            constructor.AddParameter(
                template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface),
                variableName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.DaprStateStoreUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            next(blockStack);

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
        });
    }

    private static void AddMongoDbToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.MongoDbUnitOfWorkInterface, out _))
            return;

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainMongoDb, "mongoDbUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface)))
        {
            constructor.AddParameter(
                template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface),
                variableName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.MongoDbUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            next(blockStack);

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
        });
    }

    private static void AddTableStorageToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.TableStorageUnitOfWorkInterface, out _))
            return;

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainTableStorage, "tableStorageUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface)))
        {
            constructor.AddParameter(
                type: template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface),
                name: variableName,
                configure: param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.TableStorageUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            next(blockStack);

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
        });
    }

    private static void AddRedisOmToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.RedisOmUnitOfWorkInterface, out _))
            return;

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainRedisOm, "redisOmUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != template.GetTypeName(TemplateIds.RedisOmUnitOfWorkInterface)))
        {
            constructor.AddParameter(
                template.GetTypeName(TemplateIds.RedisOmUnitOfWorkInterface),
                variableName,
                c => c.IntroduceReadonlyField());
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.RedisOmUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            next(blockStack);

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
        });
    }

    private static void AddDistributedCacheToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (!template.TryGetTemplate<ICSharpTemplate>(TemplateIds.DistributedCacheWithUnitOfWorkInterface, out _))
            return;

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainDistributedCache, "distributedCacheWithUnitOfWork");

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(x => x.Name != variableName))
        {
            constructor.AddParameter(
                type: template.GetTypeName(TemplateIds.DistributedCacheWithUnitOfWorkInterface),
                name: variableName,
                configure: param => param.IntroduceReadonlyField((_, statement) => statement.ThrowArgumentNullException()));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                var typeName = template.GetTypeName(TemplateIds.DistributedCacheWithUnitOfWorkInterface);
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddUsingBlock($"{fieldOrVariable}.EnableUnitOfWork()", @using =>
            {
                blockStack.Push(@using);
                next(blockStack);
                @using.AddStatement<IHasCSharpStatements, CSharpStatement>($"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});", SeparatedFromPrevious);
            });
        });
    }

    private static void AddEntityFrameworkToChain(
        ICSharpFileBuilderTemplate template,
        CSharpConstructor constructor,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
        if (template.GetTemplate<ICSharpTemplate>(TemplateRoles.Infrastructure.Data.DbContext, TemplateDiscoveryOptions) == null)
            return;

        bool supportsAmbientTransactions = EFProviderSupportsAmbientTransactions(template);

        var shouldAddServiceResolution = false;
        var variableName = config.VariableOverrides.GetValueOrDefault(ChainEntityFramework, "unitOfWork");

        var typeName = ResolveEntityFrameworkTypeName(template);

        if (config.ResolutionStrategy == UnitOfWorkResolutionStrategy.ServiceProvider)
        {
            shouldAddServiceResolution = true;
        }
        else if (constructor.Parameters.All(p => p.Type != typeName))
        {
            constructor.AddParameter(typeName,
                variableName,
                param => param.IntroduceReadonlyField((_, statement) =>
                {
                    statement.ThrowArgumentNullException();
                }));
        }

        chainOfResponsibilities.Push((blockStack, next) =>
        {
            var currentBlock = blockStack.Peek();

            if (shouldAddServiceResolution)
            {
                currentBlock.AddStatement($"var {variableName} = {config.ServiceProviderVariableName}.GetRequiredService<{typeName}>();");
            }

            var doingTransaction = config.AllowTransactionScope && supportsAmbientTransactions;

            if (doingTransaction && config.IncludeComments)
            {
                currentBlock.AddStatement(
                    """
                    // The execution is wrapped in a transaction scope to ensure that if any other
                    // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
                    // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
                    // locks are released, while write-locks are maintained for the duration of the
                    // transaction). Learn more on this approach for EF Core:
                    // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
                    """,
                    stmt => stmt.SeparatedFromPrevious());
            }

            if (doingTransaction)
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
                        @using.BeforeSeparator = config.IncludeComments ? CSharpCodeSeparatorType.None : CSharpCodeSeparatorType.EmptyLines;
                        @using.AddMetadata("transaction", "using-block");
                    });
            }

            next(blockStack);

            if (doingTransaction && config.IncludeComments)
            {
                currentBlock.AddStatement(
                    """
                    // By calling SaveChanges at the last point in the transaction ensures that write-
                    // locks in the database are created and then released as quickly as possible. This
                    // helps optimize the application to handle a higher degree of concurrency.
                    """,
                    stmt => stmt.SeparatedFromPrevious());
            }

            var fieldOrVariable = shouldAddServiceResolution ? variableName : $"_{variableName}";
            currentBlock.AddStatement(
                $"await {fieldOrVariable}.SaveChangesAsync({config.CancellationTokenExpression});",
                s => s.AddMetadata("transaction", "save-changes"));

            if (doingTransaction && config.IncludeComments)
            {
                currentBlock.AddStatement("// Commit transaction if everything succeeds, transaction will auto-rollback when", stmt => stmt.SeparatedFromPrevious());
                currentBlock.AddStatement("// disposed if anything failed.");
            }

            if (doingTransaction)
            {
                currentBlock.AddStatement("transaction.Complete();", s => s.AddMetadata("transaction", "complete"));
            }
        });
    }

    private static bool EFProviderSupportsAmbientTransactions(ICSharpFileBuilderTemplate template)
    {
        var databaseSettingGroup = template.ExecutionContext.GetSettings().GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9"); // Database Settings

        var provider = databaseSettingGroup?.GetSetting("00bb780c-57bf-43c1-b952-303f11096be7")?.Value; // Database Provider
        if (provider is null)
        {
            return true;
        }

        return provider != "sql-lite";
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

    private static string ResolveEntityFrameworkTypeName(ICSharpFileBuilderTemplate template)
    {
        return template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out var unitOfWork)
            ? unitOfWork
            : template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out unitOfWork)
                ? unitOfWork
                : template.TryGetTypeName(TemplateRoles.Infrastructure.Data.DbContext, out unitOfWork)
                    ? unitOfWork
                    : throw new Exception("A Unit of Work interface could not be resolved. Please contact Intent Architect support.");
    }

    private static void ExecuteChainOfResponsibilities(
        IHasCSharpStatements block,
        ICSharpFileBuilderTemplate template,
        CSharpStatement invocationStatement,
        UnitOfWorkConfiguration config,
        Stack<ChainOfResponsibility> chainOfResponsibilities)
    {
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

        if (config.ReturnType != null)
        {
            // Sometimes we need to return the variable in a parent block if work also needs to be
            // done there in which case we need to split the declaration and assignment.

            var deepestBlock = stackAtDeepest.Peek();

            IHasCSharpStatements blockWithReturn;

            while (stackAtDeepest.TryPop(out blockWithReturn!))
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
                    replaceWith: $"var {config.ResultVariableName} = {invocationStatement}");
            }
            else
            {
                // We need to split the variable and assignment
                blockWithReturn.InsertStatement(0, new CSharpStatement($"{template.UseType(config.ReturnType)} {config.ResultVariableName};"));

                invocationStatement.FindAndReplace(
                    find: invocationStatement.ToString(),
                    replaceWith: $"{config.ResultVariableName} = {invocationStatement}");
            }

            blockWithReturn.AddStatement<IHasCSharpStatements, CSharpStatement>($"return {config.ResultVariableName};", s => s.SeparatedFromPrevious());
        }
    }
}