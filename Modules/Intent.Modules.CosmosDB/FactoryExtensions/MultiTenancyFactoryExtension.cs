using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.CosmosDB.Templates;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepository;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using CSharpStatement = Intent.Modules.Common.CSharp.Builder.CSharpStatement;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDB.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MultiTenancyFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.CosmosDB.MultiTenancyFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.TemplateExists("Configuration.MultiTenancy"))
            {
                return;
            }

            UpdateRepositoryBase(application);
            
            var repositoryTemplates = application.FindTemplateInstances(CosmosDBRepositoryTemplate.TemplateId, _ => true);
            foreach (var template in repositoryTemplates)
            {
                UpdateRepository(application, (CosmosDBRepositoryTemplate)template);
            }
        }

        private static void UpdateRepository(IApplication application, CosmosDBRepositoryTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var constructor = @class.Constructors.First();

                if (!template.Model.TryGetContainerSettings(out _, out var partitionKey))
                {
                    partitionKey = template.Model.GetPrimaryKeyAttribute().Name;
                }

                constructor.AddParameter(template.UseType("Finbuckle.MultiTenant.IMultiTenantContextAccessor"), "multiTenantContextAccessor");
                constructor.ConstructorCall.AddArgument($"\"{partitionKey.ToCamelCase()}\"");
                constructor.ConstructorCall.AddArgument("multiTenantContextAccessor");

                @class.AddMethod($"Expression<Func<{template.GetCosmosDBDocumentName()}, bool>>",
                    "GetHasPartitionKeyExpression",
                    method =>
                    {
                        method.Protected().Override();
                        method.AddParameter("string?", "partitionKey");
                        method.AddStatement($"return document => document.{partitionKey.ToPascalCase()} == partitionKey;");
                    });
            });
        }

        private static void UpdateRepositoryBase(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(CosmosDBRepositoryBaseTemplate.TemplateId);
            template.AddNugetDependency(NugetDependencies.FinbuckleMultiTenant);

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System");

                var @class = file.Classes.First();
                @class.AddField("string?", "_tenantId", f => f.PrivateReadOnly());

                var constructor = @class.Constructors.First();
                constructor.AddParameter("string", "partitionKeyFieldName", p => p.IntroduceReadonlyField());
                constructor.AddParameter(template.UseType("Finbuckle.MultiTenant.IMultiTenantContextAccessor"), "multiTenantContextAccessor");
                constructor.AddStatement("_tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? throw new InvalidOperationException(\"Could not resolve tenantId\");");

                // Add method
                {
                    var method = @class.FindMethod("Add");
                    var invocationStatement = method.Statements.OfType<CSharpInvocationStatement>().First();
                    var invocationArgument = (CSharpLambdaBlock)invocationStatement.Statements[0];
                    invocationArgument.InsertStatements(1, new Collection<CSharpStatement>()
                    {
                        ConfigurableStatement("document.PartitionKey ??= _tenantId;", c => c.SeparatedFromPrevious()),
                        ConfigurableStatement(new CSharpIfStatement("document.PartitionKey != _tenantId"), @if =>
                        {
                            @if.AddStatement("throw new InvalidOperationException(\"TenantId mismatch\");");
                        }),
                    });
                }

                // Update method
                {
                    var method = @class.FindMethod("Update");
                    var invocationStatement = method.Statements.OfType<CSharpInvocationStatement>().First();
                    var invocationArgument = (CSharpLambdaBlock)invocationStatement.Statements[0];
                    invocationArgument.InsertStatements(1, new Collection<CSharpStatement>()
                    {
                        ConfigurableStatement("document.PartitionKey ??= _tenantId;", c => c.SeparatedFromPrevious()),
                        ConfigurableStatement(new CSharpIfStatement("document.PartitionKey != _tenantId"), @if =>
                        {
                            @if.AddStatement("throw new InvalidOperationException(\"TenantId mismatch\");");
                        }),
                    });
                }

                // Remove method
                {
                    var method = @class.FindMethod("Remove");
                    var invocationStatement = method.Statements.OfType<CSharpInvocationStatement>().First();
                    var invocationArgument = (CSharpLambdaBlock)invocationStatement.Statements[0];
                    invocationArgument.InsertStatements(1, new Collection<CSharpStatement>()
                    {
                        ConfigurableStatement("document.PartitionKey ??= _tenantId;", c => c.SeparatedFromPrevious()),
                        ConfigurableStatement(new CSharpIfStatement("document.PartitionKey != _tenantId"), @if =>
                        {
                            @if.AddStatement("throw new InvalidOperationException(\"TenantId mismatch\");");
                        }),
                    });
                }

                // FindAllAsync(CancellationToken cancellationToken = default)
                {
                    var method = @class.FindMethod(x => x.Name == "FindAllAsync" && x.Parameters.Count == 1);
                    method.Statements[0].FindAndReplace("_ => true", "GetHasPartitionKeyExpression(_tenantId)");
                }

                // FindByIdAsync
                {
                    var method = @class.FindMethod("FindByIdAsync");
                    method.Statements[0].FindAndReplace("id, ", "id, _tenantId, ");
                }

                // FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default)
                {
                    var method = @class.FindMethod(x => x.Name == "FindAllAsync" && x.Parameters.Count == 2);
                    ConfigurableStatement(method.Statements[0], s =>
                    {
                        s.FindAndReplace("AdaptFilterPredicate(filterExpression)", "predicate");
                        s.SeparatedFromPrevious();
                    });

                    method.InsertStatements(0, new Collection<CSharpStatement>
                    {
                        "var predicate = AdaptFilterPredicate(filterExpression);",
                        "predicate = GetFilteredByTenantIdPredicate(predicate);"
                    });
                }

                // FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default)
                {
                    var method = @class.FindMethod(x => x.Name == "FindAllAsync" && x.Parameters.Count == 4);
                    ConfigurableStatement(method.Statements[0], s =>
                    {
                        s.FindAndReplace("AdaptFilterPredicate(filterExpression)", "predicate");
                        s.SeparatedFromPrevious();
                    });

                    method.InsertStatements(0, new Collection<CSharpStatement>
                    {
                        "var predicate = AdaptFilterPredicate(filterExpression);",
                        "predicate = GetFilteredByTenantIdPredicate(predicate);"
                    });
                }

                // FindByIdsAsync
                {
                    var method = @class.FindMethod("FindByIdsAsync");
                    var queryStatement = method.Statements[0];
                    queryStatement.Replace(@"var queryDefinition = new QueryDefinition($""SELECT * from c WHERE c.{_partitionKeyFieldName} = @tenantId AND ARRAY_CONTAINS(@ids, c.{_idFieldName})"")
                    .WithParameter(""@tenantId"", _tenantId)
                    .WithParameter(""@ids"", ids);");
                }

                @class.InsertMethod(
                    index: @class.Methods.IndexOf(@class.FindMethod("AdaptFilterPredicate")),
                    returnType: "Expression<Func<TDocument, bool>>",
                    name: "GetHasPartitionKeyExpression",
                    configure: method =>
                    {
                        method.Protected().Abstract();
                        method.AddParameter("string?", "partitionKey");


                    });

                @class.InsertMethod(
                    index: @class.Methods.IndexOf(@class.FindMethod("AdaptFilterPredicate")) - 1,
                    returnType: "Expression<Func<TDocument, bool>>",
                    name: "GetFilteredByTenantIdPredicate",
                    configure: method =>
                    {
                        method.WithComments(new[]
                        {
                            "/// <summary>",
                            "/// Returns a predicate which filters by tenantId in addition to the provided <paramref name=\"predicate\"/>.",
                            "/// </summary>",
                            "/// <param name=\"predicate\">The existing predicate to also filter by.</param>",
                        });
                        method.Private();
                        method.AddParameter("Expression<Func<TDocument, bool>>", "predicate");
                        method.AddStatements(new[]
                        {
                            "var restrictToTenantPredicate = GetHasPartitionKeyExpression(_tenantId);",
                            "return Expression.Lambda<Func<TDocument, bool>>(Expression.AndAlso(predicate.Body, restrictToTenantPredicate.Body), restrictToTenantPredicate.Parameters[0]);"
                        });
                    });
            });
        }

        private static TStatement ConfigurableStatement<TStatement>(TStatement statement, Action<TStatement> configure)
            where TStatement : CSharpStatement
        {
            configure(statement);
            return statement;
        }

        private static CSharpStatement ConfigurableStatement(string statement, Action<CSharpStatement> configure)
        {
            return ConfigurableStatement(new CSharpStatement(statement), configure);
        }
    }
}