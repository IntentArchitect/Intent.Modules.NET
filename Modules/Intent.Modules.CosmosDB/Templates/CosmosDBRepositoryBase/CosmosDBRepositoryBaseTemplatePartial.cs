using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
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

namespace Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CosmosDBRepositoryBaseTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.CosmosDB.CosmosDBRepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CosmosDBRepositoryBaseTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetDependencies.IEvangelistAzureCosmosRepository);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Microsoft.Azure.Cosmos")
                .AddClass($"CosmosDBRepositoryBase", @class =>
                {
                    @class
                        .Internal()
                        .Abstract()
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TPersistence", out var tPersistence)
                        .AddGenericParameter("TDocument", out var tDocument)
                        .ImplementsInterface($"{this.GetCosmosDBRepositoryInterfaceName()}<{tDomain}, {tPersistence}>")
                        .AddGenericTypeConstraint(tPersistence, c => c
                            .AddType(tDomain))
                        .AddGenericTypeConstraint(tDocument, c => c
                            .AddType(tPersistence)
                            .AddType($"{this.GetCosmosDBDocumentInterfaceName()}<{tDocument}, {tDomain}>")
                            .AddType("new()"))
                        ;

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter(this.GetCosmosDBUnitOfWorkName(), "unitOfWork",
                            p => p.IntroduceReadonlyField());
                        ctor.AddParameter(UseType($"Microsoft.Azure.CosmosRepository.IRepository<{tDocument}>"),
                            "cosmosRepository", p => p.IntroduceReadonlyField());
                        ctor.AddParameter("string", "idFieldName", p => p.IntroduceReadonlyField());
                    });

                    @class.AddProperty(this.GetCosmosDBUnitOfWorkInterfaceName(), "UnitOfWork", p => p
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation("_unitOfWork")
                    );

                    @class.AddMethod("void", "Add", m => m
                        .AddParameter(tDomain, "entity")
                        .AddStatement("_unitOfWork.Track(entity);", s => s.SeparatedFromPrevious())
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);")
                                    .AddStatement(
                                        "await _cosmosRepository.CreateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Update", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);")
                                    .AddStatement(
                                        "await _cosmosRepository.UpdateAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod("void", "Remove", m => m
                        .AddParameter(tDomain, "entity")
                        .AddInvocationStatement("_unitOfWork.Enqueue", invocation =>
                        {
                            invocation.SeparatedFromPrevious();
                            invocation
                                .AddArgument(new CSharpLambdaBlock("async cancellationToken")
                                    .AddStatement($"var document = new {tDocument}().PopulateFromEntity(entity);")
                                    .AddStatement(
                                        "await _cosmosRepository.DeleteAsync(document, cancellationToken: cancellationToken);")
                                );
                        })
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", m => m
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement("var documents = await _cosmosRepository.GetAsync(_ => true, cancellationToken);")
                        .AddStatement("var results = documents.Cast<TDomain>().ToList();")
                        .AddForEachStatement("result", "results", fe =>
                        {
                            fe.SeparatedFromPrevious();
                            fe.AddStatement("_unitOfWork.Track(result);");
                        })
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<{tDomain}?>", "FindByIdAsync", m => m
                        .Async()
                        .AddParameter("string", "id")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            "var document = await _cosmosRepository.GetAsync(id, cancellationToken: cancellationToken);")
                        .AddStatement("return document;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method =>
                    {
                        method.Virtual();
                        method.Async();
                        method.AddParameter($"Expression<Func<{tPersistence}, bool>>", "filterExpression")
                            .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                                                                     
                        method.AddStatement($"var documents = await _cosmosRepository.GetAsync(AdaptPredicate(filterExpression), cancellationToken);");
                        method.AddStatement($"return documents.Cast<TDomain>().ToList();");
                    });

                    @class.AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", m => m
                        .AddParameter("IEnumerable<string>", "ids")
                        .Async()
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                        .AddStatement(
                            @"var queryDefinition = new QueryDefinition($""SELECT * from c WHERE ARRAY_CONTAINS(@ids, c.{_idFieldName})"")
                .WithParameter(""@ids"", ids);")
                        .AddStatement(
                            "var documents = await _cosmosRepository.GetByQueryAsync(queryDefinition, cancellationToken);")
                        .AddStatement("var results = documents.Cast<TDomain>().ToList();")
                        .AddForEachStatement("result", "results", fe =>
                        {
                            fe.SeparatedFromPrevious();
                            fe.AddStatement("_unitOfWork.Track(result);");
                        })
                        .AddStatement("return results;", s => s.SeparatedFromPrevious())
                    );

                    @class.AddMethod("Expression<Func<TDocument, bool>>", "AdaptPredicate", method =>
                    {
                        method
                            .Static()
                            .AddParameter("Expression<Func<TPersistence, bool>>", "expression");

                        method.AddStatement("if (!typeof(TPersistence).IsAssignableFrom(typeof(TDocument))) throw new Exception(string.Format(\"{0} is not assignable from {1}.\", typeof(TPersistence), typeof(TDocument)));");
                        method.AddStatement("var beforeParameter = expression.Parameters.Single();");
                        method.AddStatement("var afterParameter = Expression.Parameter(typeof(TDocument), beforeParameter.Name);");
                        method.AddStatement("var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);");
                        method.AddStatement("return Expression.Lambda<Func<TDocument, bool>>(visitor.Visit(expression.Body), afterParameter);");
                      
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

                        nestClass.AddMethod("Expression", "Visit", method => 
                        {
                            method
                                .Override()
                                .AddParameter("Expression", "node");

                            method.AddStatement("return node == _before ? _after : base.Visit(node);");
                        });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            this.ApplyAppSetting("RepositoryOptions", new
            {
                CosmosConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                DatabaseId = ExecutionContext.GetApplicationConfig().Name,
                ContainerId = "Container"
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