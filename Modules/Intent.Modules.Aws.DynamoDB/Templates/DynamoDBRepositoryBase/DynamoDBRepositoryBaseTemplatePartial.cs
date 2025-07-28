using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Aws.DynamoDB.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.DynamoDB.Templates.DynamoDBRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DynamoDBRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.DynamoDB.DynamoDBRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DynamoDBRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            var useOptimisticConcurrency = ExecutionContext.Settings.GetDynamoDBSettings().UseOptimisticConcurrency();
            AddNugetDependency(NugetPackages.AWSSDKDynamoDBv2(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")

                .AddClass("DynamoDBRepositoryBase", @class =>
                {
                    @class.Internal().Abstract();

                    @class.AddGenericParameter("TDomain", out var tDomain);

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @class.AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @class.AddGenericParameter("TDocument", out var tDocument);
                    @class.AddGenericParameter("TPartitionKey", out var tPartitionKey);
                    @class.AddGenericParameter("TSortKey", out var tSortKey);

                    @class
                        .ImplementsInterface($"{this.GetDynamoDBRepositoryInterfaceName()}<{tDomain}, {tPartitionKey}>")
                        .ImplementsInterface($"{this.GetDynamoDBRepositoryInterfaceName()}<{tDomain}, {tPartitionKey}, {tSortKey}>")
                        .AddGenericTypeConstraint(tDomain, c => c.AddType("class"));

                    if (createEntityInterfaces)
                    {
                        @class
                            .AddGenericTypeConstraint(tDomainState, c => c
                                .AddType("class")
                                .AddType(tDomain));
                    }

                    var tDomainStateConstraint = createEntityInterfaces
                        ? $", {tDomainState}"
                        : string.Empty;
                    @class
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType($"{this.GetDynamoDBDocumentOfTInterfaceName()}<{tDomain}{tDomainStateConstraint}, {tDocument}>")
                            .AddType("new()"))
                        ;

                    if (useOptimisticConcurrency)
                    {
                        @class.AddField("Dictionary<object, int?>", "_versions", f => f
                            .PrivateReadOnly()
                            .WithAssignment(new CSharpStatement("new Dictionary<object, int?>()")));
                    }
                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(UseType("Amazon.DynamoDBv2.DataModel.IDynamoDBContext"), "context", p => p.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetDynamoDBUnitOfWorkName(), "unitOfWork", p => p.IntroduceReadonlyField());
                    });

                    @class.AddProperty(this.GetDynamoDBUnitOfWorkInterfaceName(), "UnitOfWork", p => p
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    );

                    @class.AddMethod("void", "Add", method =>
                    {
                        method.AddParameter(tDomain, "entity");
                        method.AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious());

                        method.AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddArgument(new CSharpLambdaBlock("async cancellationToken"), lambda =>
                            {
                                var getEntityArgument = useOptimisticConcurrency
                                    ? ", _ => null"
                                    : string.Empty;

                                lambda.AddStatement(
                                    $"var document = new {tDocument}().PopulateFromEntity(entity{getEntityArgument});");
                                lambda.AddStatement("await _context.SaveAsync(document, cancellationToken);");
                            });
                        });
                    });

                    @class.AddMethod("void", "Update", method =>
                    {
                        method.AddParameter(tDomain, "entity");

                        method.AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.SeparatedFromPrevious();
                            invocation.AddArgument(new CSharpLambdaBlock("async cancellationToken"), lambda =>
                            {
                                lambda.AddStatement(useOptimisticConcurrency
                                    ? $"var document = new {tDocument}().PopulateFromEntity(entity, _versions.GetValueOrDefault);"
                                    : $"var document = new {tDocument}().PopulateFromEntity(entity);");

                                lambda.AddStatement("await _context.SaveAsync(document, cancellationToken);");
                            });
                        });
                    });

                    @class.AddMethod("void", "Remove", method =>
                    {
                        method.AddParameter(tDomain, "entity");

                        method.AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.SeparatedFromPrevious();
                            invocation.AddArgument(new CSharpLambdaBlock("async cancellationToken"), lambda =>
                            {
                                lambda.AddStatement(useOptimisticConcurrency
                                    ? $"var document = new {tDocument}().PopulateFromEntity(entity, _versions.GetValueOrDefault);"
                                    : $"var document = new {tDocument}().PopulateFromEntity(entity);");

                                lambda.AddStatement(
                                    "await _context.DeleteAsync<TDocument>(document.GetKey(), cancellationToken);");
                            });
                        });
                    });

                    @class.AddMethod($"Task<{tDomain}?>", "FindByKeyAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(tPartitionKey, "partitionKey");
                        method.AddOptionalCancellationTokenParameter();

                        method.AddStatement("var document = await _context.LoadAsync<TDocument>(partitionKey, cancellationToken);");
                        method.AddIfStatement("document == null", @if =>
                        {
                            @if.SeparatedFromPrevious(false);
                            @if.AddStatement("return null;");
                        });

                        method.AddStatement("var entity = LoadAndTrackDocument(document);", s => s.SeparatedFromPrevious());
                        method.AddStatement("return entity;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<{tDomain}?>", "FindByKeyAsync", method =>
                    {
                        method.Async();
                        method.AddParameter(tPartitionKey, "partitionKey");
                        method.AddParameter(tSortKey, "sortKey");
                        method.AddOptionalCancellationTokenParameter();

                        method.AddStatement("var document = await _context.LoadAsync<TDocument>(partitionKey, sortKey, cancellationToken);");
                        method.AddIfStatement("document == null", @if =>
                        {
                            @if.SeparatedFromPrevious(false);
                            @if.AddStatement("return null;");
                        });

                        method.AddStatement("var entity = LoadAndTrackDocument(document);", s => s.SeparatedFromPrevious());
                        method.AddStatement("return entity;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByKeysAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"IEnumerable<{tPartitionKey}>", "partitionKeys");
                        method.AddOptionalCancellationTokenParameter();

                        method.AddStatement("var batch = _context.CreateBatchGet<TDocument>();");
                        method.AddForEachStatement("partitionKey", "partitionKeys", @foreach =>
                        {
                            @foreach.AddStatement("batch.AddKey(partitionKey);");
                        });

                        method.AddStatement("await batch.ExecuteAsync(cancellationToken);", s => s.SeparatedFromPrevious());
                        method.AddStatement("return LoadAndTrackDocuments(batch.Results).ToList();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByKeysAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"IEnumerable<({tPartitionKey} Partition, {tSortKey} Sort)>", "keys");
                        method.AddOptionalCancellationTokenParameter();

                        method.AddStatement("var batch = _context.CreateBatchGet<TDocument>();");
                        method.AddForEachStatement("key", "keys", @foreach =>
                        {
                            @foreach.AddStatement("batch.AddKey(key.Partition, key.Sort);");
                        });

                        method.AddStatement("await batch.ExecuteAsync(cancellationToken);", s => s.SeparatedFromPrevious());

                        method.AddStatement("return LoadAndTrackDocuments(batch.Results).ToList();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Async();
                        method.AddOptionalCancellationTokenParameter();

                        method.AddStatement("var documents = await _context.ScanAsync<TDocument>(Enumerable.Empty<ScanCondition>()).GetRemainingAsync(cancellationToken);");
                        method.AddStatement("return LoadAndTrackDocuments(documents).ToList();", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod(tDomain, "LoadAndTrackDocument", method =>
                    {
                        method.AddParameter($"{tDocument}", "document");

                        method.AddStatement("var entity = document.ToEntity();");

                        method.AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious());
                        if (useOptimisticConcurrency)
                        {
                            method.AddStatement("_versions[document.GetKey()] = document.GetVersion();");
                        }

                        method.AddStatement("return entity;", s => s.SeparatedFromPrevious());

                    });

                    @class.AddMethod($"IEnumerable<{tDomain}>", "LoadAndTrackDocuments", method =>
                    {
                        method.AddParameter($"IEnumerable<{tDocument}>", "documents");
                        method.AddStatement("return documents.Select(LoadAndTrackDocument);");
                    });
                });
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