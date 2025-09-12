#nullable enable
using System.Collections.Generic;

namespace Intent.Modules.Persistence.UnitOfWork.Shared;

internal enum UnitOfWorkResolutionStrategy
{
    ConstructorInjection,
    ServiceProvider
}

public class UnitOfWorkConfiguration
{
    internal string? ReturnType { get; private set; }
    internal string ResultVariableName { get; private set; } = "result";
    internal string CancellationTokenExpression { get; private set; } = "cancellationToken";
    internal string FieldSuffix { get; private set; } = "unitOfWork";
    internal bool AllowTransactionScope { get; private set; } = true;
    internal bool IncludeComments { get; private set; } = true;
    internal Dictionary<string, string> VariableOverrides { get; } = new();
    internal UnitOfWorkResolutionStrategy ResolutionStrategy { get; private set; } = UnitOfWorkResolutionStrategy.ConstructorInjection;
    internal string? ServiceProviderVariableName { get; private set; }

    public UnitOfWorkConfiguration WithReturnType(string returnType)
    {
        ReturnType = returnType;
        return this;
    }

    public UnitOfWorkConfiguration WithResultVariableName(string variableName)
    {
        ResultVariableName = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseCancellationToken(string expression)
    {
        CancellationTokenExpression = expression;
        return this;
    }

    public UnitOfWorkConfiguration WithFieldSuffix(string suffix)
    {
        FieldSuffix = suffix;
        return this;
    }

    public UnitOfWorkConfiguration WithTransactionScope(bool allow = true)
    {
        AllowTransactionScope = allow;
        return this;
    }

    public UnitOfWorkConfiguration WithComments(bool include = true)
    {
        IncludeComments = include;
        return this;
    }

    public UnitOfWorkConfiguration UseEntityFrameworkVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainEntityFramework] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseCosmosDbVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainCosmosDb] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseMongoDbVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainMongoDb] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseDaprStateStoreVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainDapr] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseRedisOmVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainRedisOm] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseTableStorageVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainTableStorage] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseDistributedCacheVariable(string variableName)
    {
        VariableOverrides[PersistenceUnitOfWork.ChainDistributedCache] = variableName;
        return this;
    }

    public UnitOfWorkConfiguration UseConstructorInjection()
    {
        ResolutionStrategy = UnitOfWorkResolutionStrategy.ConstructorInjection;
        ServiceProviderVariableName = null;
        return this;
    }

    public UnitOfWorkConfiguration UseServiceProvider(string serviceProviderVariableName)
    {
        ResolutionStrategy = UnitOfWorkResolutionStrategy.ServiceProvider;
        ServiceProviderVariableName = serviceProviderVariableName;
        return this;
    }
}