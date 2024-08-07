using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            AddNugetDependency(NugetPackages.AzureDataTables(OutputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Azure.Data.Tables")
                .AddUsing("Azure")
                .AddClass("TableStorageRepositoryBase", @class =>
                {
                    @class
                        .Internal()
                        .Abstract()
                        .AddGenericParameter("TDomain", out var tDomain);

                    var tDomainState = tDomain;
                    if (createEntityInterfaces)
                    {
                        @class.AddGenericParameter("TDomainState", out tDomainState);
                    }

                    @class
                        .AddGenericParameter("TTable", out var tTable)
                        .AddGenericParameter("TTableInterfae", out var tTableInterface)
                        .ImplementsInterface($"{this.GetTableStorageRepositoryInterfaceName()}<{tDomain}, {tTableInterface}>")
                        .AddGenericTypeConstraint(tDomainState, constraint =>
                        {
                            constraint.AddType("class");
                            if (createEntityInterfaces)
                            {
                                constraint.AddType(tDomain);
                            }
                        })
                        .AddGenericTypeConstraint(tTable, c => c
                            .AddType("class")
                            .AddType($"{this.GetTableStorageTableAdapterInterfaceName()}<{tDomain}, {tTable}>")
                            .AddType("new()"))
                    ;

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(this.GetTableStorageUnitOfWorkName(), "unitOfWork",
                            p => p.IntroduceReadonlyField());
                        ctor.AddParameter(UseType("TableServiceClient"), "tableServiceClient");
                        ctor.AddParameter("string", "tableName");
                        @class.AddField("TableClient", "_tableClient", f => f.PrivateReadOnly());

                        ctor.AddStatement("_tableClient = tableServiceClient.GetTableClient(tableName);");
                        ctor.AddStatement("_tableClient.CreateIfNotExists();");
                    });

                    @class.AddProperty(this.GetTableStorageUnitOfWorkInterfaceName(), "UnitOfWork", p => p
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    );

                    @class.AddMethod("void", "Add", m => m
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious())
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tTable}().PopulateFromEntity(entity);")
                                    .AddStatement(
                                        "await _tableClient.AddEntityAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Update", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tTable}().PopulateFromEntity(entity);")
                                    .AddStatement(
                                        "await _tableClient.UpdateEntityAsync(document, ETag.All, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Remove", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.AddMetadata(MetadataNames.EnqueueStatement, true);
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tTable}().PopulateFromEntity(entity);")
                                    .AddStatement(
                                        "await _tableClient.DeleteEntityAsync(document.PartitionKey, document.RowKey);")
                                );
                        })
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var results = new List<TDomain>();")
                        .AddStatement($"var response = _tableClient.QueryAsync<{tTable}>(cancellationToken: cancellationToken);")
                        .AddForEachStatement("document", "response", loop =>
                        {
                            loop.Await();
                            loop.AddStatement("results.Add(document.ToEntity());");
                        })
                        .AddStatement("Track(results);")
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<{tDomain}?>", "FindByIdAsync", m => m
                        .Async()
                        .AddParameter("(string partitionKey, string rowKey)", "id")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            $"var response = await _tableClient.GetEntityAsync<{tTable}>(id.partitionKey, id.rowKey, cancellationToken: cancellationToken);")
                        .AddStatement("var entity = response.Value.ToEntity();")
                        .AddStatement("Track(entity);")
                        .AddStatement("return entity;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tTableInterface}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));

                        method
                            .AddStatement($"var results = new List<{tDomain}>();")
                            .AddStatement("var response = _tableClient.QueryAsync(AdaptFilterPredicate(filterExpression), cancellationToken: cancellationToken);")
                        .AddForEachStatement("document", "response", loop =>
                        {
                            loop.Await();
                            loop.AddStatement("results.Add(document.ToEntity());");
                        })

                            .AddStatement("Track(results);")
                            .AddStatement("return results;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod($"Expression<Func<{tTable}, bool>>", "AdaptFilterPredicate", method =>
                    {
                        method
                            .Private()
                            .Static()
                            .AddParameter($"Expression<Func<{tTableInterface}, bool>>", "expression")
                            .WithComments(new[]
                            {
                                "/// <summary>",
                                $"/// Adapts a <typeparamref name=\"{tTableInterface}\"/> predicate to a <typeparamref name=\"{tTable}\"/> predicate.",
                                "/// </summary>"
                            });

                        method.AddStatement($"if (!typeof({tTableInterface}).IsAssignableFrom(typeof(TTable))) throw new Exception($\"typeof({tTableInterface}) is not assignable from typeof({tTable}).\");");
                        method.AddStatement("var beforeParameter = expression.Parameters.Single();");
                        method.AddStatement($"var afterParameter = Expression.Parameter(typeof({tTable}), beforeParameter.Name);");
                        method.AddStatement("var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);");
                        method.AddStatement($"return Expression.Lambda<Func<{tTable}, bool>>(visitor.Visit(expression.Body)!, afterParameter);");

                    });

                    @class.AddMethod("void", "Track", method =>
                    {
                        method.AddParameter($"IEnumerable<{tDomain}>", "items");

                        method.AddForEachStatement("item", "items", stmt => stmt.AddStatement("_unitOfWork.Track(item);"));

                    });

                    @class.AddMethod("void", "Track", method =>
                    {
                        method.AddParameter($"{tDomain}", "item");
                        method.AddStatement("_unitOfWork.Track(item);");
                    });


                    @class.AddNestedClass("SubstitutionExpressionVisitor", nestClass =>
                    {
                        nestClass
                            .Private()
                            .WithBaseType("ExpressionVisitor");
                        nestClass.AddConstructor(ctor =>
                        {
                            ctor
                                .AddParameter("Expression", "before", p => p.IntroduceReadonlyField())
                                .AddParameter("Expression", "after", p => p.IntroduceReadonlyField());
                        });

                        nestClass.AddMethod("Expression?", "Visit", method =>
                        {
                            method
                                .Override()
                                .AddParameter("Expression?", "node");

                            method.AddStatement("return node == _before ? _after : base.Visit(node);");
                        });
                    });
                });
        }


        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            this.ApplyAppSetting("TableStorageConnectionString", "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;");
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