using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.TemporalTables.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.TemporalTables.Templates.TemporalHistory;
using Intent.Modules.EntityFrameworkCore.TemporalTables.Templates.TemporalInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.TemporalTables.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RepositoryFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.TemporalTables.RepositoryFactoryExtension";

        public const string RepositoryInterfaceId = "Intent.Entities.Repositories.Api.EntityRepositoryInterface";

        public const string RepositoryImplementationId = "Intent.EntityFrameworkCore.Repositories.Repository";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            UpdateInterface(application);
            UpdateImplementation(application);
        }

        private void UpdateInterface(IApplication application)
        {
            // get all class models which are temporal
            var temporalEntities = application.MetadataManager
                .GetDesigner(application.Id, ApiMetadataDesignerExtensions.DomainDesignerId)
                .GetClassModels()
                .Where(c => c.HasTemporalTable());

            if (!temporalEntities.Any())
            {
                return;
            }

            var repoInterfaceTemplates = application
               .FindTemplateInstances<CSharpTemplateBase<ClassModel>>(TemplateDependency.OnTemplate(RepositoryInterfaceId))
               .Where(t => temporalEntities.Contains(t.Model));

            foreach (var templateInstance in repoInterfaceTemplates)
            {
                ((ICSharpFileBuilderTemplate)templateInstance).CSharpFile.AfterBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var model = @interface.GetMetadata<ClassModel>("model");

                    AddRepositoryFindHistoryInterfaceMethods(file);
                });
            }
        }

        private void UpdateImplementation(IApplication application)
        {
            // get all class models which are temporal
            var temporalEntities = application.MetadataManager
                .GetDesigner(application.Id, ApiMetadataDesignerExtensions.DomainDesignerId)
                .GetClassModels()
                .Where(c => c.HasTemporalTable());

            if (!temporalEntities.Any())
            {
                return;
            }

            var baseEFRepositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.RepositoryBase");

            if (baseEFRepositoryTemplate is null)
            {
                return;
            }

            baseEFRepositoryTemplate.CSharpFile.AfterBuild(file =>
            {
                AddBaseFindHistoryImplementationeMethods(file);
            });

            var repoTemplates = application
                .FindTemplateInstances<CSharpTemplateBase<ClassModel>>(TemplateDependency.OnTemplate(RepositoryImplementationId))
                .Where(t => temporalEntities.Contains(t.Model));

            foreach (var templateInstance in repoTemplates)
            {
                ((ICSharpFileBuilderTemplate)templateInstance).CSharpFile.AfterBuild(file =>
                {
                    AddRepositoryFindHistoryImplementationMethods(file);
                });
            }
        }

        private void AddBaseFindHistoryInterfaceMethods(CSharpFile file)
        {
            var @interface = file.Interfaces.First();

            var template = file.Template as CSharpTemplateBase<object>;

            template.AddUsing("System.Linq");

            @interface.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<TDomain>>>", "FindHistoryAsync", method =>
            {
                method.AddGenericParameter("TTemporalPersistence")
                    .AddGenericTypeConstraint("TTemporalPersistence", con =>
                    {
                        con.AddType("class")
                            .AddType("TDomain")
                            .AddType("TPersistence")
                            .AddType(template.GetTypeName(TemporalInterfaceTemplate.TemplateId));
                    });

                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter("string", "validFromColumnName", param => param.WithDefaultValue("\"PeriodStart\""));
                method.AddParameter("string", "validToColumnName", param => param.WithDefaultValue("\"PeriodEnd\""));
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });

            @interface.AddMethod($"Task<List<TemporalHistory<TDomain>>>", "FindHistoryAsync", method =>
            {
                method.AddGenericParameter("TTemporalPersistence")
                    .AddGenericTypeConstraint("TTemporalPersistence", con =>
                    {
                        con.AddType("class")
                            .AddType("TDomain")
                            .AddType("TPersistence")
                            .AddType(template.GetTypeName(TemporalInterfaceTemplate.TemplateId));
                    });

                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<TTemporalPersistence, bool>>")}", "filterExpression");
                method.AddParameter("string", "validFromColumnName", param => param.WithDefaultValue("\"PeriodStart\""));
                method.AddParameter("string", "validToColumnName", param => param.WithDefaultValue("\"PeriodEnd\""));
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });

            @interface.AddMethod($"Task<List<TemporalHistory<TDomain>>>", "FindHistoryAsync", method =>
            {
                method.AddGenericParameter("TTemporalPersistence")
                   .AddGenericTypeConstraint("TTemporalPersistence", con =>
                   {
                       con.AddType("class")
                           .AddType("TDomain")
                           .AddType("TPersistence")
                           .AddType(template.GetTypeName(TemporalInterfaceTemplate.TemplateId));
                   });

                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"Expression<Func<TTemporalPersistence, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<TTemporalPersistence>, IQueryable<TTemporalPersistence>>", "queryOptions");
                method.AddParameter("string", "validFromColumnName", param => param.WithDefaultValue("\"PeriodStart\""));
                method.AddParameter("string", "validToColumnName", param => param.WithDefaultValue("\"PeriodEnd\""));
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });
        }

        private void AddRepositoryFindHistoryInterfaceMethods(CSharpFile file)
        {
            var @interface = file.Interfaces.First();
            var model = @interface.GetMetadata<ClassModel>("model");

            var template = file.Template as CSharpTemplateBase<ClassModel>;
            var tDomainType = template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model);
            var tPersistanceType = template.GetTypeName(TemplateRoles.Domain.Entity.Primary, model);

            template.AddUsing("System.Linq");

            @interface.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<{tDomainType}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });

            @interface.AddMethod($"Task<List<TemporalHistory<{tDomainType}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<{tPersistanceType}, bool>>")}", "filterExpression");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });

            @interface.AddMethod($"Task<List<TemporalHistory<{tDomainType}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"Expression<Func<{tPersistanceType}, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<{tDomainType}>, IQueryable<{tPersistanceType}>>", "queryOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });
        }

        private void AddBaseFindHistoryImplementationeMethods(CSharpFile file)
        {
            var @class = file.Classes.First();
            var template = file.Template as CSharpTemplateBase<object>;

            template.AddUsing("Microsoft.EntityFrameworkCore");
            template.AddUsing("System.Linq");

            @class.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<TDomain>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter("string", "validFromColumnName", param => param.WithDefaultValue("\"PeriodStart\""));
                method.AddParameter("string", "validToColumnName", param => param.WithDefaultValue("\"PeriodEnd\""));
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.AddGenericParameter("TTemporalPersistence");
                method.AddGenericTypeConstraint("TTemporalPersistence", con =>
                {
                    con.AddType("class")
                    .AddType("TDomain")
                    .AddType("TPersistence")
                    .AddType(template.GetTypeName(TemporalInterfaceTemplate.TemplateId));
                });

                method.Async();
                method.Protected();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryAsync<TTemporalPersistence>")
                    .AddArgument("historyOptions")
                    .AddArgument("null")
                    .AddArgument("null")
                    .AddArgument("validFromColumnName")
                    .AddArgument("validToColumnName")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<TDomain>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<TTemporalPersistence, bool>>")}", "filterExpression");
                method.AddParameter("string", "validFromColumnName", param => param.WithDefaultValue("\"PeriodStart\""));
                method.AddParameter("string", "validToColumnName", param => param.WithDefaultValue("\"PeriodEnd\""));
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.AddGenericParameter("TTemporalPersistence");
                method.AddGenericTypeConstraint("TTemporalPersistence", con =>
                {
                    con.AddType("class")
                    .AddType("TDomain")
                    .AddType("TPersistence")
                    .AddType(template.GetTypeName(TemporalInterfaceTemplate.TemplateId));
                });

                method.Async();
                method.Protected();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryAsync<TTemporalPersistence>")
                    .AddArgument("historyOptions")
                    .AddArgument("filterExpression")
                    .AddArgument("null")
                    .AddArgument("validFromColumnName")
                    .AddArgument("validToColumnName")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<TDomain>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"Expression<Func<TTemporalPersistence, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<TTemporalPersistence>, IQueryable<TTemporalPersistence>>", "queryOptions");
                method.AddParameter("string", "validFromColumnName", param => param.WithDefaultValue("\"PeriodStart\""));
                method.AddParameter("string", "validToColumnName", param => param.WithDefaultValue("\"PeriodEnd\""));
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.AddGenericParameter("TTemporalPersistence");
                method.AddGenericTypeConstraint("TTemporalPersistence", con =>
                {
                    con.AddType("class")
                    .AddType("TDomain")
                    .AddType("TPersistence")
                    .AddType(template.GetTypeName(TemporalInterfaceTemplate.TemplateId));
                });

                method.Async();
                method.Protected();

                method.AddObjectInitStatement("var internalDateFrom", "(historyOptions is null || historyOptions.DateFrom == null || historyOptions.DateFrom == DateTime.MinValue) ? DateTime.MinValue : historyOptions!.DateFrom.Value;");
                method.AddObjectInitStatement("var internalDateTo", "(historyOptions is null || historyOptions.DateTo == null || historyOptions.DateTo == DateTime.MinValue) ? DateTime.MinValue : historyOptions!.DateTo.Value;");
                method.AddObjectInitStatement("var queryType", "(historyOptions is null || historyOptions?.QueryType == null) ? TemporalHistoryQueryType.All : historyOptions.QueryType.Value;");

                method.AddObjectInitStatement("var dbSet", "GetSet();");
                method.AddObjectInitStatement("var queryable", "GetEntityQueryable(queryType);");

                method.AddIfStatement("filterExpression != null", @if =>
                {
                    @if.AddObjectInitStatement("queryable", "queryable.Where(filterExpression);");
                });

                method.AddIfStatement("queryOptions != null", @if =>
                {
                    @if.AddObjectInitStatement("queryable", "queryOptions(queryable);");
                });

                var invoc = new CSharpInvocationStatement("await queryable.Select")
                    .AddArgument(new CSharpLambdaBlock("entity"), a =>
                    {
                        a.WithExpressionBody(new CSharpInvocationStatement($"new TemporalHistory<TDomain>")
                            .WithoutSemicolon()
                            .AddArgument("entity")
                            .AddArgument($"EF.Property<DateTime>(entity, validFromColumnName)")
                            .AddArgument($"EF.Property<DateTime>(entity, validToColumnName)"));

                    }).AddInvocation("ToListAsync", cfg => cfg.AddArgument("cancellationToken"));

                method.AddReturn(invoc.WithoutSemicolon());

                method.AddLocalMethod($"IQueryable<TTemporalPersistence>", "GetEntityQueryable", local =>
                {
                    local.AddParameter("TemporalHistoryQueryType", "queryType");

                    local.AddSwitchStatement("queryType", @switch =>
                    {
                        @switch.AddCase("TemporalHistoryQueryType.All", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalAll().Cast<TTemporalPersistence>()");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.AsOf", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalAsOf(internalDateFrom).Cast<TTemporalPersistence>()");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.Between", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalBetween(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>()");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.ContainedIn", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalContainedIn(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>()");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.FromTo", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalFromTo(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>()");
                        });
                        @switch.AddDefault(@case =>
                        {
                            @case.AddReturn("dbSet.TemporalBetween(internalDateFrom, internalDateTo).Cast<TTemporalPersistence>()");
                        });
                    });
                });
            });
        }

        private void AddRepositoryFindHistoryImplementationMethods(CSharpFile file)
        {
            var @class = file.Classes.First();
            var model = @class.GetMetadata<ClassModel>("model");

            var template = file.Template as CSharpTemplateBase<ClassModel>;
            var tDomainType = template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model);
            var tPersistanceType = template.GetTypeName(TemplateRoles.Domain.Entity.Primary, model);

            template.AddUsing("System.Linq");

            var periodStartName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodStartColumnName()) ? model.GetTemporalTable().PeriodStartColumnName() : "PeriodStart";
            var periodEndName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodEndColumnName()) ? model.GetTemporalTable().PeriodEndColumnName() : "PeriodEnd";

            @class.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<{tDomainType}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                var invocStatement = new CSharpInvocationStatement($"await FindHistoryAsync<{tDomainType}>")
                    .AddArgument("historyOptions")
                    .AddArgument("null")
                    .AddArgument("null")
                    .AddArgument($"\"{periodStartName}\"")
                    .AddArgument($"\"{periodEndName}\"")
                    .AddArgument("cancellationToken");

                method.Async();

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{tDomainType}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<{tPersistanceType}, bool>>")}", "filterExpression");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                var invocStatement = new CSharpInvocationStatement($"await FindHistoryAsync<{tDomainType}>")
                    .AddArgument("historyOptions")
                    .AddArgument("filterExpression")
                    .AddArgument("null")
                    .AddArgument($"\"{periodStartName}\"")
                    .AddArgument($"\"{periodEndName}\"")
                    .AddArgument("cancellationToken");

                method.Async();

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{tDomainType}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"Expression<Func<{tPersistanceType}, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<{tDomainType}>, IQueryable<{tPersistanceType}>>", "queryOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                var invocStatement = new CSharpInvocationStatement($"await FindHistoryAsync<{tDomainType}>")
                    .AddArgument("historyOptions")
                    .AddArgument("filterExpression")
                    .AddArgument("queryOptions")
                    .AddArgument($"\"{periodStartName}\"")
                    .AddArgument($"\"{periodEndName}\"")
                    .AddArgument("cancellationToken");

                method.Async();

                method.AddReturn(invocStatement.WithoutSemicolon());
            });
        }

        private string GetTemporalHistoryName(IntentTemplateBase template) => template.GetTypeName(TemporalHistoryTemplate.TemplateId);
    }
}