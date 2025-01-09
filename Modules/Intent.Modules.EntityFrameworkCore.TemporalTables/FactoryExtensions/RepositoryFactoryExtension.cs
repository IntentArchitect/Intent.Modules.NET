using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.TemporalTables.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.TemporalTables.Templates.TemporalHistory;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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
            UpdateEntityRepositoryInterface(application);
            UpdateEntityRepositoryImplementation(application);
        }

        private void UpdateEntityRepositoryInterface(IApplication application)
        {
            // get all class models which are temporal
            var temporalEntities = application.MetadataManager
                .GetDesigner(application.Id, ApiMetadataDesignerExtensions.DomainDesignerId)
                .GetClassModels()
                .Where(c => c.HasTemporalTable());

            var repoInterfaceTemplates = application
                .FindTemplateInstances<CSharpTemplateBase<ClassModel>>(TemplateDependency.OnTemplate(RepositoryInterfaceId))
                .Where(t => temporalEntities.Contains(t.Model));

            foreach (var template in repoInterfaceTemplates)
            {
                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var model = @interface.GetMetadata<ClassModel>("model");

                    AddFindHistoryInterfaceMethods(file, @interface, model);
                });
            }
        }

        private void UpdateEntityRepositoryImplementation(IApplication application)
        {
            // get all class models which are temporal
            var temporalEntities = application.MetadataManager
                .GetDesigner(application.Id, ApiMetadataDesignerExtensions.DomainDesignerId)
                .GetClassModels()
                .Where(c => c.HasTemporalTable());

            var repoInterfaceTemplates = application
                .FindTemplateInstances<CSharpTemplateBase<ClassModel>>(TemplateDependency.OnTemplate(RepositoryImplementationId))
                .Where(t => temporalEntities.Contains(t.Model));

            foreach (var template in repoInterfaceTemplates)
            {
                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    AddFindHistoryImplementationeMethods(file, @class, model);
                });
            }
        }

        private void AddFindHistoryInterfaceMethods(CSharpFile file, CSharpInterface @interface, ClassModel model)
        {
            var template = file.Template as CSharpTemplateBase<ClassModel>;

            template.AddUsing("System.Linq");

            @interface.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<{GetEntityStateName(template)}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });

            @interface.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<{GetEntityStateName(template)}, bool>>")}", "filterExpression");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });

            @interface.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"Expression<Func<{GetEntityStateName(template)}, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<{GetEntityStateName(template)}>, IQueryable<{GetEntityStateName(template)}>>", "queryOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));
            });
        }

        private void AddFindHistoryImplementationeMethods(CSharpFile file, CSharpClass @class, ClassModel model)
        {
            var template = file.Template as CSharpTemplateBase<ClassModel>;

            template.AddUsing("Microsoft.EntityFrameworkCore");
            template.AddUsing("System.Linq");

            @class.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<{GetEntityStateName(template)}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryAsync")
                    .AddArgument("historyOptions")
                    .AddArgument("null")
                    .AddArgument("null")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<{GetEntityStateName(template)}, bool>>")}", "filterExpression");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryAsync")
                    .AddArgument("historyOptions")
                    .AddArgument("filterExpression")
                    .AddArgument("null")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryOptions", "historyOptions");
                method.AddParameter($"Expression<Func<{GetEntityStateName(template)}, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<{GetEntityStateName(template)}>, IQueryable<{GetEntityStateName(template)}>>", "queryOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

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

                var periodStartName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodStartColumnName()) ? model.GetTemporalTable().PeriodStartColumnName() : "PeriodStart";
                var periodEndName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodEndColumnName()) ? model.GetTemporalTable().PeriodEndColumnName() : "PeriodEnd";

                var invoc = new CSharpInvocationStatement("await queryable.Select")
                    .AddArgument(new CSharpLambdaBlock("entity"), a =>
                    {
                        a.WithExpressionBody(new CSharpInvocationStatement($"new TemporalHistory<{GetEntityStateName(template)}>")
                            .WithoutSemicolon()
                            .AddArgument("entity")
                            .AddArgument($"EF.Property<DateTime>(entity, \"{periodStartName}\")")
                            .AddArgument($"EF.Property<DateTime>(entity, \"{periodEndName}\")"));

                    }).AddInvocation("ToListAsync", cfg => cfg.AddArgument("cancellationToken"));

                method.AddReturn(invoc.WithoutSemicolon());

                method.AddLocalMethod($"IQueryable<{GetEntityStateName(template)}>", "GetEntityQueryable", local =>
                {
                    local.AddParameter("TemporalHistoryQueryType", "queryType");

                    local.AddSwitchStatement("queryType", @switch =>
                    {
                        @switch.AddCase("TemporalHistoryQueryType.All", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalAll()");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.AsOf", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalAsOf(internalDateFrom)");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.Between", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalBetween(internalDateFrom, internalDateTo)");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.ContainedIn", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalContainedIn(internalDateFrom, internalDateTo)");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.FromTo", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalFromTo(internalDateFrom, internalDateTo)");
                        });
                        @switch.AddDefault(@case =>
                        {
                            @case.AddReturn("dbSet.TemporalBetween(internalDateFrom, internalDateTo)");
                        });
                    });
                });
            });
        }

        private void AddFindAllHistoryAsOfImplementationeMethods(CSharpFile file, CSharpClass @class, ClassModel model)
        {
            var template = file.Template as CSharpTemplateBase<ClassModel>;

            @class.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<{GetEntityStateName(template)}>>>", "FindHistoryAsOfAsync", method =>
            {
                method.AddParameter(template.UseType("System.DateTime"), "pointInTime");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryAsOfAsync")
                    .AddArgument("pointInTime")
                    .AddArgument("null")
                    .AddArgument("null")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryAsOfAsync", method =>
            {
                method.AddParameter(template.UseType("System.DateTime"), "pointInTime");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<{GetEntityStateName(template)}, bool>>")}", "filterExpression");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryAsOfAsync")
                    .AddArgument("pointInTime")
                    .AddArgument("filterExpression")
                    .AddArgument("null")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryAsOfAsync", method =>
            {
                method.AddParameter(template.UseType("System.DateTime"), "pointInTime");
                method.AddParameter($"Expression<Func<{GetEntityStateName(template)}, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<{GetEntityStateName(template)}>, IQueryable<{GetEntityStateName(template)}>>", "queryOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                method.AddObjectInitStatement("var dbSet", "GetSet();");
                method.AddObjectInitStatement("var queryable", "dbSet.TemporalAsOf(pointInTime);");

                method.AddIfStatement("filterExpression != null", @if =>
                {
                    @if.AddObjectInitStatement("queryable", "queryable.Where(filterExpression);");
                });

                method.AddIfStatement("queryOptions != null", @if =>
                {
                    @if.AddObjectInitStatement("queryable", "queryOptions(queryable);");
                });

                var periodStartName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodStartColumnName()) ? model.GetTemporalTable().PeriodStartColumnName() : "PeriodStart";
                var periodEndName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodEndColumnName()) ? model.GetTemporalTable().PeriodEndColumnName() : "PeriodEnd";

                var invoc = new CSharpInvocationStatement("await queryable.Select")
                    .AddArgument(new CSharpLambdaBlock("entity"), a =>
                    {
                        a.WithExpressionBody(new CSharpInvocationStatement($"new TemporalHistory<{GetEntityStateName(template)}>")
                            .WithoutSemicolon()
                            .AddArgument("entity")
                            .AddArgument($"EF.Property<DateTime>(entity, \"{periodStartName}\")")
                            .AddArgument($"EF.Property<DateTime>(entity, \"{periodEndName}\")"));

                    }).AddInvocation("ToListAsync", cfg => cfg.AddArgument("cancellationToken"));

                method.AddReturn(invoc.WithoutSemicolon());
            });
        }

        private void AddFindAllHistoryDateRangeImplementationeMethods(CSharpFile file, CSharpClass @class, ClassModel model)
        {
            var template = file.Template as CSharpTemplateBase<ClassModel>;

            @class.AddMethod($"Task<List<{GetTemporalHistoryName(template)}<{GetEntityStateName(template)}>>>", "FindHistoryByDateRangeAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryType", "queryType");
                method.AddParameter(template.UseType("System.DateTime"), "dateFrom");
                method.AddParameter(template.UseType("System.DateTime"), "dateTo");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryByDateRangeAsync")
                    .AddArgument("queryType")
                    .AddArgument("dateFrom")
                    .AddArgument("dateTo")
                    .AddArgument("null")
                    .AddArgument("null")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryByDateRangeAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryType", "queryType");
                method.AddParameter(template.UseType("System.DateTime"), "dateFrom");
                method.AddParameter(template.UseType("System.DateTime"), "dateTo");
                method.AddParameter($"{template.UseType($"System.Linq.Expressions.Expression<Func<{GetEntityStateName(template)}, bool>>")}", "filterExpression");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

                var invocStatement = new CSharpInvocationStatement("await FindHistoryByDateRangeAsync")
                    .AddArgument("queryType")
                    .AddArgument("dateFrom")
                    .AddArgument("dateTo")
                    .AddArgument("filterExpression")
                    .AddArgument("null")
                    .AddArgument("cancellationToken");

                method.AddReturn(invocStatement.WithoutSemicolon());
            });

            @class.AddMethod($"Task<List<TemporalHistory<{GetEntityStateName(template)}>>>", "FindHistoryByDateRangeAsync", method =>
            {
                method.AddParameter("TemporalHistoryQueryType", "queryType");
                method.AddParameter(template.UseType("System.DateTime"), "dateFrom");
                method.AddParameter(template.UseType("System.DateTime"), "dateTo");
                method.AddParameter($"Expression<Func<{GetEntityStateName(template)}, bool>>", "filterExpression");
                method.AddParameter($"Func<IQueryable<{GetEntityStateName(template)}>, IQueryable<{GetEntityStateName(template)}>>", "queryOptions");
                method.AddParameter("CancellationToken", "cancellationToken", cfg => cfg.WithDefaultValue("default"));

                method.Async();

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

                var periodStartName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodStartColumnName()) ? model.GetTemporalTable().PeriodStartColumnName() : "PeriodStart";
                var periodEndName = !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodEndColumnName()) ? model.GetTemporalTable().PeriodEndColumnName() : "PeriodEnd";

                var invoc = new CSharpInvocationStatement("await queryable.Select")
                    .AddArgument(new CSharpLambdaBlock("entity"), a =>
                    {
                        a.WithExpressionBody(new CSharpInvocationStatement($"new TemporalHistory<{GetEntityStateName(template)}>")
                            .WithoutSemicolon()
                            .AddArgument("entity")
                            .AddArgument($"EF.Property<DateTime>(entity, \"{periodStartName}\")")
                            .AddArgument($"EF.Property<DateTime>(entity, \"{periodEndName}\")"));

                    }).AddInvocation("ToListAsync", cfg => cfg.AddArgument("cancellationToken"));

                method.AddReturn(invoc.WithoutSemicolon());

                method.AddLocalMethod($"IQueryable<{GetEntityStateName(template)}>", "GetEntityQueryable", local =>
                {
                    local.AddParameter("TemporalHistoryQueryType", "queryType");

                    local.AddSwitchStatement("queryType", @switch =>
                    {
                        @switch.AddCase("TemporalHistoryQueryType.Between", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalBetween(dateFrom, dateTo)");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.ContainedIn", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalContainedIn(dateFrom, dateTo)");
                        });
                        @switch.AddCase("TemporalHistoryQueryType.FromTo", @case =>
                        {
                            @case.AddReturn("dbSet.TemporalFromTo(dateFrom, dateTo)");
                        });
                        @switch.AddDefault(@case =>
                        {
                            @case.AddReturn("dbSet.TemporalBetween(dateFrom, dateTo)");
                        });
                    });
                });
            });
        }

        private string GetEntityStateName(CSharpTemplateBase<ClassModel> template) => template.GetTypeName("Domain.Entity", template.Model);

        private string GetTemporalHistoryName(CSharpTemplateBase<ClassModel> template) => template.GetTypeName(TemporalHistoryTemplate.TemplateId);
    }
}