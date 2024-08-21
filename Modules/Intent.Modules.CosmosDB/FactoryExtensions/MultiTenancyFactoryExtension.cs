using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.CosmosDB.Templates;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepository;
using Intent.Modules.CosmosDB.Templates.CosmosDBRepositoryBase;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
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
            if (!application.TemplateExists(TemplateRoles.Distribution.WebApi.MultiTenancyConfiguration))
            {
                return;
            }


            if (DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(application.Settings))
            {
                ConfigureSeperateDatabaseMultiTenancy(application);
                return;
            }

            var repositoryBaseTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(CosmosDBRepositoryBaseTemplate.TemplateId);
            UpdateRepositoryBase(repositoryBaseTemplate);

            var repositoryTemplates = application.FindTemplateInstances(CosmosDBRepositoryTemplate.TemplateId, _ => true);
            foreach (var template in repositoryTemplates)
            {
                UpdateRepository((CosmosDBRepositoryTemplate)template);
            }
        }

        private void ConfigureSeperateDatabaseMultiTenancy(IApplication application)
        {
            ConfigureStartUpForSeperateDatabaseMultiTenancy(application);
            ConfigureDependencyInjectionForSeperateDatabaseMultiTenancy(application);
        }

        private void ConfigureDependencyInjectionForSeperateDatabaseMultiTenancy(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                //Add the Using
                template.GetCosmosDBMultiTenancyConfigurationName();
                var method = file.Classes.First().FindMethod("AddInfrastructure");
                method.AddInvocationStatement("services.ConfigureCosmosSeperateDBMultiTenancy", invocation =>
                {
                    invocation.AddArgument("configuration");
                });
            });

        }

        private void ConfigureStartUpForSeperateDatabaseMultiTenancy(IApplication application)
        {
            var template = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            template?.CSharpFile.AfterBuild(_ =>
            {
                template.StartupFile.ConfigureApp((statements, ctx) =>
                {
                    var useUseMultiTenancyStatement = statements.FindStatement(x => x.ToString()!.Contains(".UseMultiTenancy()"));
                    if (useUseMultiTenancyStatement == null)
                    {
                        throw new("app.UseMultiTenancy() was not configured");
                    }
                    template.GetCosmosDBMultiTenantMiddlewareName();
                    useUseMultiTenancyStatement.InsertBelow($"{ctx.App}.UseCosmosMultiTenantMiddleware();");
                });
            }, 15);
        }

        private static void UpdateRepository(CosmosDBRepositoryTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                file
                    .AddUsing("System")
                    .AddUsing("System.Linq.Expressions");

                var @class = file.Classes.First();
                var constructor = @class.Constructors.First();

                var pk = template.Model.GetPrimaryKeyAttribute();
                template.Model.TryGetPartitionAttribute(out var partitionAttribute);

                constructor.ConstructorCall.AddArgument($"{(partitionAttribute != null ? $"\"{partitionAttribute.Name.ToCamelCase()}\"" : "default")}");
                if (RequiresMultiTenancy(template.Model))
                {
                    constructor.AddParameter(template.UseType("Finbuckle.MultiTenant.IMultiTenantContextAccessor<TenantInfo>"), "multiTenantContextAccessor");
                    constructor.ConstructorCall.AddArgument("multiTenantContextAccessor");
                }
                else
                {
                    constructor.ConstructorCall.AddArgument($"default({template.UseType("Finbuckle.MultiTenant.IMultiTenantContextAccessor<TenantInfo>")})");
                }

                if (RequiresMultiTenancy(template.Model))
                {
                    if (partitionAttribute == null)
                    {
                        Logging.Log.Warning($"No Partition Key configured for {template.Model.Name} which is marked for Multi-tenancy");
                    }
                    @class.AddMethod($"Expression<Func<{template.GetCosmosDBDocumentName()}, bool>>",
                        "GetHasPartitionKeyExpression",
                        method =>
                        {
                            method.Protected().Override();
                            method.AddParameter($"string{nullableChar}", "partitionKey");
                            method.AddStatement($"return document => document.{partitionAttribute.Name.ToPascalCase()} == partitionKey;");
                        });
                }
            });
        }

        private static void UpdateRepositoryBase(ICSharpFileBuilderTemplate template)
        {
            template.AddNugetDependency(NugetPackages.FinbuckleMultiTenant(template.OutputTarget));
            template.CSharpFile.OnBuild(file =>
            {
                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                file.AddUsing("System");

                var @class = file.Classes.First();
                @class.AddField($"string{nullableChar}", "_tenantId", f => f.PrivateReadOnly());

                var constructor = @class.Constructors.First();
                constructor.AddParameter($"string{nullableChar}", "partitionKeyFieldName", p => p.IntroduceReadonlyField());
                constructor.AddParameter(template.UseType($"Finbuckle.MultiTenant.IMultiTenantContextAccessor<TenantInfo>{nullableChar}"), "multiTenantContextAccessor");

                constructor.AddIfStatement("multiTenantContextAccessor != null", stmt =>
                {
                    stmt.AddStatement("_tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? throw new InvalidOperationException(\"Could not resolve tenantId\");");
                });

                // Add method
                {
                    var method = @class.FindMethod("Add");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    var documentDeclarationStatement = invocationArgument.FindStatement(x => x.HasMetadata(MetadataNames.DocumentDeclarationStatement));

                    ConfigurableStatement(
                        invocationArgument.Statements[invocationArgument.Statements.IndexOf(documentDeclarationStatement) + 1],
                        c => c.SeparatedFromPrevious());

                    documentDeclarationStatement.InsertBelow(
                        ConfigurableStatement("CheckTenancy(document);", c => c.SeparatedFromPrevious()));
                }

                // Update method
                {
                    var method = @class.FindMethod("Update");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    var documentDeclarationStatement = invocationArgument.FindStatement(x => x.HasMetadata(MetadataNames.DocumentDeclarationStatement));

                    ConfigurableStatement(
                        invocationArgument.Statements[invocationArgument.Statements.IndexOf(documentDeclarationStatement) + 1],
                        c => c.SeparatedFromPrevious());

                    documentDeclarationStatement.InsertBelow(
                        ConfigurableStatement("CheckTenancy(document);", c => c.SeparatedFromPrevious()));
                }

                // Remove method
                {
                    var method = @class.FindMethod("Remove");
                    var enqueueStatement = method.Statements.OfType<CSharpInvocationStatement>().First(x => x.HasMetadata(MetadataNames.EnqueueStatement));
                    var invocationArgument = (CSharpLambdaBlock)enqueueStatement.Statements[0];

                    var documentDeclarationStatement = invocationArgument.FindStatement(x => x.HasMetadata(MetadataNames.DocumentDeclarationStatement));

                    ConfigurableStatement(
                        invocationArgument.Statements[invocationArgument.Statements.IndexOf(documentDeclarationStatement) + 1],
                        c => c.SeparatedFromPrevious());

                    documentDeclarationStatement.InsertBelow(
                        ConfigurableStatement("CheckTenancy(document);", c => c.SeparatedFromPrevious()));
                }

                // FindAllAsync(CancellationToken cancellationToken = default)
                {
                    var method = @class.FindMethod(x => x.Name == "FindAllAsync" && x.Parameters.Count == 1);
                    var documentsDeclarationStatement = method.FindStatement(x => x.HasMetadata(MetadataNames.DocumentsDeclarationStatement));
                    documentsDeclarationStatement.FindAndReplace("_ => true", "GetHasPartitionKeyExpression(_tenantId)");
                }

                // FindByIdAsync
                {
                    var method = @class.FindMethod(m => m.Name == "FindByIdAsync");
                    var documentDeclarationStatement = method.FindStatement(x => x.HasMetadata(MetadataNames.DocumentDeclarationStatement));
                    documentDeclarationStatement.FindAndReplace("id, partitionKey, ", "id, partitionKey ?? _tenantId, ");
                }

                // FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default)
                {
                    var method = @class.FindMethod(x => x.Name == "FindAllAsync" && x.Parameters.Count == 2);

                    var documentsDeclarationStatement = method.FindStatement(x => x.HasMetadata(MetadataNames.DocumentsDeclarationStatement));
                    documentsDeclarationStatement.FindAndReplace("AdaptFilterPredicate(filterExpression)", "predicate");
                    documentsDeclarationStatement.SeparatedFromPrevious();
                    documentsDeclarationStatement.InsertAbove(
                        "var predicate = AdaptFilterPredicate(filterExpression);",
                        "predicate = GetFilteredByTenantIdPredicate(predicate);");
                }

                // FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default)
                {
                    var method = @class.FindMethod(x => x.Name == "FindAllAsync" && x.Parameters.Count == 4);

                    var pagedDocumentsDeclarationStatement = method.FindStatement(x => x.HasMetadata(MetadataNames.PagedDocumentsDeclarationStatement));
                    pagedDocumentsDeclarationStatement.FindAndReplace("AdaptFilterPredicate(filterExpression)", "predicate");
                    pagedDocumentsDeclarationStatement.SeparatedFromPrevious();
                    pagedDocumentsDeclarationStatement.InsertAbove(
                        "var predicate = AdaptFilterPredicate(filterExpression);",
                        "predicate = GetFilteredByTenantIdPredicate(predicate);");
                }

                // FindByIdsAsync
                {
                    var method = @class.FindMethod("FindByIdsAsync");
                    var queryDefinitionDeclarationStatement = method.FindStatement(x => x.HasMetadata(MetadataNames.QueryDefinitionDeclarationStatement));
                    queryDefinitionDeclarationStatement.Replace(@"var queryDefinition = new QueryDefinition($""SELECT * from c WHERE {(!string.IsNullOrEmpty(_partitionKeyFieldName) ? ""(@tenantId = null OR c.{_partitionKeyFieldName} = @tenantId)  AND "" : """")}ARRAY_CONTAINS(@ids, c.{_idFieldName})"")
                    .WithParameter(""@tenantId"", _tenantId)
                    .WithParameter(""@ids"", ids);");
                }

                @class.InsertMethod(
                    index: @class.Methods.IndexOf(@class.FindMethod("AdaptFilterPredicate")),
                    returnType: "Expression<Func<TDocument, bool>>",
                    name: "GetHasPartitionKeyExpression",
                    configure: method =>
                    {
                        method.Protected().Virtual();
                        method.AddParameter("string?", "partitionKey");
                        method.AddStatement("return _ => true;");
                    });
                @class.InsertMethod(
                    index: @class.Methods.IndexOf(@class.FindMethod("GetHasPartitionKeyExpression")),
                    returnType: "void",
                    name: "CheckTenancy",
                    configure: method =>
                    {
                        method.AddParameter("TDocument", "document");
                        method.AddIfStatement("_tenantId == null", c => c.AddStatement("return;"));
                        method.AddStatement("document.PartitionKey ??= _tenantId;", c => c.SeparatedFromPrevious());
                        method.AddStatement(new CSharpIfStatement("document.PartitionKey != _tenantId"), @if =>
                        {
                            @if.SeparatedFromPrevious(false);
                            @if.AddStatement("throw new InvalidOperationException(\"TenantId mismatch\");");
                        });
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
                            "if (_tenantId == null) return predicate;",
                            "var restrictToTenantPredicate = GetHasPartitionKeyExpression(_tenantId);",
                            "return Expression.Lambda<Func<TDocument, bool>>(Expression.AndAlso(predicate.Body, restrictToTenantPredicate.Body), restrictToTenantPredicate.Parameters[0]);"
                        });
                    });
            });
        }

        private static bool RequiresMultiTenancy(ClassModel model)
        {
            return model.HasStereotype("Multi Tenant");
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