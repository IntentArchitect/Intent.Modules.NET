using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Blazor.Templates.Templates.Client.Program;

internal class CSharpMethodWrapper<TCSharpMethod>(ICSharpMethod<TCSharpMethod> wrapped) : IBlazorProgramFileMethod where TCSharpMethod : ICSharpMethod<TCSharpMethod>
{
    IEnumerable<ICSharpParameter> IHasICSharpParameters.Parameters => wrapped.Parameters;

    IList<ICSharpStatement> IHasCSharpStatementsActual.Statements => wrapped.Statements;

    string IHasCSharpName.Name => wrapped.Name;

    void ICSharpCodeContext.RepresentsModel(IMetadataModel model) => wrapped.RepresentsModel(model);

    ICSharpCodeContext ICSharpCodeContext.AddMetadata(string key, object value) => wrapped.AddMetadata(key, value);

    bool ICSharpCodeContext.HasMetadata(string key) => wrapped.HasMetadata(key);

    T ICSharpCodeContext.GetMetadata<T>(string key) => wrapped.GetMetadata<T>(key);

    object ICSharpCodeContext.GetMetadata(string key) => wrapped.GetMetadata(key);

    bool ICSharpCodeContext.TryGetMetadata<T>(string key, out T value) => wrapped.TryGetMetadata(key, out value);

    bool ICSharpCodeContext.TryGetMetadata(string key, out object value) => wrapped.TryGetMetadata(key, out value);

    IHasCSharpName ICSharpCodeContext.GetReferenceForModel(string modelId) => wrapped.GetReferenceForModel(modelId);

    IHasCSharpName ICSharpCodeContext.GetReferenceForModel(IMetadataModel model) => wrapped.GetReferenceForModel(model);

    bool ICSharpCodeContext.TryGetReferenceForModel(string modelId, out IHasCSharpName reference) => wrapped.TryGetReferenceForModel(modelId, out reference);

    bool ICSharpCodeContext.TryGetReferenceForModel(IMetadataModel model, out IHasCSharpName reference) => wrapped.TryGetReferenceForModel(model, out reference);

    void ICSharpCodeContext.RegisterReferenceable(string modelId, ICSharpReferenceable cSharpReferenceable) => wrapped.RegisterReferenceable(modelId, cSharpReferenceable);

    ICSharpFile ICSharpCodeContext.File => wrapped.File;

    ICSharpCodeContext ICSharpCodeContext.Parent => wrapped.Parent;

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddGenericParameter(string typeName, out ICSharpGenericParameter param)
    {
        wrapped.AddGenericParameter(typeName, out param);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddGenericParameter(string typeName)
    {
        wrapped.AddGenericParameter(typeName);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddGenericTypeConstraint(string genericParameterName, Action<ICSharpGenericTypeConstraint>? configure)
    {
        wrapped.AddGenericTypeConstraint(genericParameterName, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddOptionalCancellationTokenParameter(string? parameterName)
    {
        wrapped.AddOptionalCancellationTokenParameter(parameterName);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddParameter(string type, string name, Action<ICSharpMethodParameter>? configure)
    {
        wrapped.AddParameter(type, name, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddParameter<TModel>(string type, TModel model, Action<ICSharpMethodParameter>? configure)
    {
        wrapped.AddParameter(type, model, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddParameter<TModel>(TModel model, Action<ICSharpMethodParameter>? configure)
    {
        wrapped.AddParameter(model, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddParameters<TModel>(IEnumerable<TModel> models, Action<ICSharpMethodParameter>? configure)
    {
        wrapped.AddParameters(models, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddStatement(string statement, Action<ICSharpStatement>? configure)
    {
        wrapped.AddStatement(statement, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddStatement<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure)
    {
        wrapped.AddStatement(statement, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddStatements(IEnumerable<string> statements, Action<IEnumerable<ICSharpStatement>>? configure)
    {
        wrapped.AddStatements(statements, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddStatements(string statements, Action<IEnumerable<ICSharpStatement>>? configure)
    {
        wrapped.AddStatements(statements, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.AddStatements<TCSharpStatement>(IEnumerable<TCSharpStatement> statements, Action<IEnumerable<TCSharpStatement>>? configure)
    {
        wrapped.AddStatements(statements, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.Async(bool asValueTask)
    {
        wrapped.Async(asValueTask);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.FindAndReplaceStatement(Func<ICSharpStatement, bool> matchFunc, ICSharpStatement replaceWith)
    {
        wrapped.FindAndReplaceStatement(matchFunc, replaceWith);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.InsertParameter(int index, string type, string name, Action<ICSharpMethodParameter>? configure)
    {
        wrapped.InsertParameter(index, type, name, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.InsertStatement(int index, ICSharpStatement statement, Action<ICSharpStatement>? configure)
    {
        wrapped.InsertStatement(index, statement, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.InsertStatements(int index, IReadOnlyCollection<ICSharpStatement> statements, Action<IEnumerable<ICSharpStatement>>? configure)
    {
        wrapped.InsertStatements(index, statements, configure);
        return this;
    }

    void ICSharpMethod<IBlazorProgramFileMethod>.RemoveStatement(ICSharpStatement statement)
    {
        wrapped.RemoveStatement(statement);
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.Static()
    {
        wrapped.Static();
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.Sync()
    {
        wrapped.Sync();
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.WithExpressionBody(string statement, Action<ICSharpStatement>? configure)
    {
        wrapped.WithExpressionBody(statement, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.WithExpressionBody<TCSharpStatement>(TCSharpStatement statement, Action<TCSharpStatement>? configure)
    {
        wrapped.WithExpressionBody(statement, configure);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.WithReturnType(ICSharpType returnType)
    {
        wrapped.WithReturnType(returnType);
        return this;
    }

    IBlazorProgramFileMethod ICSharpMethod<IBlazorProgramFileMethod>.WithReturnType(string returnType)
    {
        wrapped.WithReturnType(returnType);
        return this;
    }

    IList<ICSharpGenericParameter> ICSharpMethod<IBlazorProgramFileMethod>.GenericParameters => wrapped.GenericParameters;

    IList<ICSharpGenericTypeConstraint> ICSharpMethod<IBlazorProgramFileMethod>.GenericTypeConstraints => wrapped.GenericTypeConstraints;

    bool ICSharpMethod<IBlazorProgramFileMethod>.HasExpressionBody => wrapped.HasExpressionBody;

    bool ICSharpMethod<IBlazorProgramFileMethod>.IsAsync => wrapped.IsAsync;

    bool ICSharpMethod<IBlazorProgramFileMethod>.IsStatic => wrapped.IsStatic;

    ICSharpExpression ICSharpMethod<IBlazorProgramFileMethod>.ReturnType => wrapped.ReturnType;
}