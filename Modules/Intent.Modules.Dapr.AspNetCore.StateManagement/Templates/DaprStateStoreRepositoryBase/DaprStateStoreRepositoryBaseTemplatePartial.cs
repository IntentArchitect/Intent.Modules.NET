using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprStateStoreRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprStateStoreRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Collections.Concurrent")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Text.Json")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Dapr.Client")
                .AddClass("DaprStateStoreRepositoryBase", @class => @class
                    .Abstract()
                    .AddGenericParameter("TDomain", out var tDomain)
                    .AddField("ConcurrentBag<StateTransactionRequest>", "_transactionRequests", field => field
                        .PrivateReadOnly()
                        .WithAssignment("new()")
                    )
                    .AddField("object", "_lock", field => field
                        .PrivateReadOnly()
                        .WithAssignment("new()")
                    )
                    .AddConstructor(constructor => constructor
                        .Protected()
                        .AddParameter("DaprClient", "daprClient", parameter => parameter.IntroduceReadonlyField())
                        .AddParameter(this.GetDaprStateStoreUnitOfWorkName(), "unitOfWork", parameter => parameter.IntroduceReadonlyField())
                        .AddParameter("bool", "enableTransactions", parameter => parameter.IntroduceReadonlyField())
                        .AddParameter("string", "storeName", parameter => parameter.IntroduceReadonlyField())
                    )
                    .AddProperty(this.GetDaprStateStoreUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    )
                    .AddMethod($"Task<{tDomain}?>", "FindByKeyAsync", method => method
                        .Async()
                        .AddParameter("string", "key")
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                        .AddStatement(@"var entity = await _daprClient.GetStateAsync<TDomain?>(
                storeName: _storeName,
                key: key,
                cancellationToken: cancellationToken,
                metadata: new Dictionary<string, string>
                {
                    [""contentType""] = ""application/json""
                });"
                        )
                        .AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious())
                        .AddStatement("return entity;", s => s.SeparatedFromPrevious())
                    )
                    .AddMethod($"Task<List<{tDomain}>>", "FindByKeysAsync", method => method
                        .Protected()
                        .Async()
                        .AddParameter("string[]", "ids")
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                        .AddStatement(@"var result = await _daprClient.GetBulkStateAsync(
                storeName: _storeName,
                keys: ids,
                parallelism: 1,
                cancellationToken: cancellationToken,
                metadata: new Dictionary<string, string>
                {
                    [""contentType""] = ""application/json""
                });"
                        )
                        .AddStatement(@"var entities = result
                .Select(x => JsonSerializer.Deserialize<TDomain>(x.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!)
                .ToList();", s => s.SeparatedFromPrevious())
                        .AddForEachStatement("entity", "entities", forEach =>
                        {
                            forEach.SeparatedFromPrevious();

                            forEach
                                .AddStatement("_unitOfWork.Track(entity);");
                        })
                        .AddStatement("return entities;", s => s.SeparatedFromPrevious())
                    )
                    .AddMethod("void", "Upsert", method => method
                        .Protected()
                        .AddParameter("string", "key")
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);")
                        .AddIfStatement("_enableTransactions", ifStatement =>
                        {
                            ifStatement.SeparatedFromPrevious();

                            ifStatement
                                .AddStatement(@"EnqueueTransactionRequest(new StateTransactionRequest(
                    key: key,
                    value: JsonSerializer.SerializeToUtf8Bytes(entity),
                    operationType: StateOperationType.Upsert,
                    metadata: new Dictionary<string, string>
                    {
                        [""contentType""] = ""application/json"",
                    }));"
                                )
                                .AddStatement("return;");
                        })
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.SeparatedFromPrevious();

                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement(@"await _daprClient.SaveStateAsync(
                    storeName: _storeName,
                    key: key,
                    value: entity,
                    cancellationToken: cancellationToken,
                    metadata: new Dictionary<string, string>
                    {
                        [""contentType""] = ""application/json""
                    });"
                                    )
                                );
                        })
                    )
                    .AddMethod("void", "Remove", method => method
                        .Protected()
                        .AddParameter("string", "key")
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);")
                        .AddIfStatement("_enableTransactions", ifStatement =>
                        {
                            ifStatement.SeparatedFromPrevious();

                            ifStatement
                                .AddStatement(@"EnqueueTransactionRequest(new StateTransactionRequest(
                    key: key,
                    value: null,
                    operationType: StateOperationType.Delete));"
                                )
                                .AddStatement("return;");
                        })
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                .AddStatement(@"await _daprClient.DeleteStateAsync(
                    storeName: _storeName,
                    key: key,
                    cancellationToken: cancellationToken);"
                                    )
                                );
                        })
                    )
                    .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                        .AddStatement(@"var result = await _daprClient.QueryStateAsync<TDomain>(
                storeName: _storeName,
                jsonQuery: ""{ \""filter: \"": {}}"",
                cancellationToken: cancellationToken,
                metadata: new Dictionary<string, string>
                {
                    [""contentType""] = ""application/json"",
                    [""queryIndexName""] = ""key""
                });"
                        )
                        .AddStatement(@"var entities = result.Results
                .Select(x => x.Data)
                .ToList();",
                            s => s.SeparatedFromPrevious()
                        )
                        .AddForEachStatement("entity", "entities", forEach =>
                        {
                            forEach.SeparatedFromPrevious();

                            forEach
                                .AddStatement("_unitOfWork.Track(entity);");
                        })
                        .AddStatement("return entities;", s => s.SeparatedFromPrevious())
                    )
                    .AddMethod("void", "EnqueueTransactionRequest", method => method
                        .Private()
                        .AddParameter("StateTransactionRequest", "request")
                        .AddIfStatement("_transactionRequests.IsEmpty", @if => @if
                            .AddStatementBlock("lock (_lock)", @lock => @lock
                                .AddIfStatement("_transactionRequests.IsEmpty", innerIf => innerIf
                                    .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                                    {
                                        invocation.SeparatedFromPrevious();
                                        invocation
                                            .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                            .AddStatement(@"await _daprClient.ExecuteStateTransactionAsync(
                                storeName: _storeName,
                                operations: _transactionRequests.ToArray(),
                                cancellationToken: cancellationToken);"
                                                )
                                            );
                                    })
                                )
                            )
                        )
                        .AddStatement("_transactionRequests.Add(request);", s => s.SeparatedFromPrevious())
                    )
                );
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}