using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.TemporalTables.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.TemporalTables.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EfConfigurationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.TemporalTables.EfConfigurationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityConfig = application
               .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                   TemplateDependency.OnTemplate("Infrastructure.Data.EntityTypeConfiguration"))
               .Where(p => p.Model.HasTemporalTable() || (p.Model.ParentClass is not null && p.Model.ParentClass.HasTemporalTable()))
               .Cast<ICSharpFileBuilderTemplate>()
               .ToArray();

            foreach (var entity in entityConfig)
            {
                entity.CSharpFile.AfterBuild(file =>
                {
                    var entityClass = file.Classes.First();
                    var configMethod = entityClass.FindMethod("Configure");

                    var existingToTableStatement = configMethod?.FindStatement(GetToTableStatement);

                    if (file.Template is IIntentTemplate<ClassModel> template)
                    {

                        // use case for when there is no existing ToTable statement in the configuration
                        if (existingToTableStatement == null)
                        {
                            CSharpInvocationStatement toTableInvocation = GetNewToTableStatement(template.Model);
                            configMethod.InsertStatement(0, toTableInvocation);

                            return;
                        }

                        if (existingToTableStatement is CSharpInvocationStatement invokeStatement)
                        {
                            // if there is a ToTable statement and it does NOT already contain a builder action
                            if (!StatementContainsTableBuilderAction(invokeStatement))
                            {
                                var lambda = GetTableBuilderAction(template.Model);
                                invokeStatement.AddArgument(lambda);

                                return;
                            }

                            // finally the use case when there is already a lambda in the ToTable invocation
                            var existingTableBuilderAction = invokeStatement.Statements[^1];
                            var updatedTableBuilderAction = GetUpdatedTableBuildAction(existingTableBuilderAction, template.Model);

                            invokeStatement.Statements.Remove(existingTableBuilderAction);
                            invokeStatement.AddArgument(updatedTableBuilderAction);
                        }
                    }
                });
            }
        }

        private static CSharpInvocationStatement GetNewToTableStatement(ClassModel model)
        {
            var toTableInvocation = new CSharpInvocationStatement("builder.ToTable");
            var lambda = GetTableBuilderAction(model);
            toTableInvocation.AddArgument(lambda);
            return toTableInvocation;
        }

        private static bool StatementContainsTableBuilderAction(CSharpInvocationStatement invokeStatement)
        {
            if (!invokeStatement.Statements.Any())
            {
                return false;
            }

            return invokeStatement.Statements[^1].ToString().Contains("=>");
        }

        private static bool GetToTableStatement(ICSharpStatement statement)
        {
            return statement.Text?.StartsWith("builder.ToTable") ?? false;
        }

        private static CSharpLambdaBlock GetTableBuilderAction(ClassModel model)
        {
            var lambda = new CSharpLambdaBlock("tb");
            lambda.AddStatement(GetTemporalInvocationStatement(model));

            return lambda;
        }

        private static CSharpInvocationStatement GetTemporalInvocationStatement(ClassModel model)
        {
            var invocation = new CSharpInvocationStatement("tb.IsTemporal");

            // if all are default values
            if (model.HasTemporalTable() && string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodStartColumnName())
                && string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodEndColumnName())
                && string.IsNullOrWhiteSpace(model.GetTemporalTable().HistoryTableName()))
            {
                return invocation;
            }

            var lambdaBlock = new CSharpLambdaBlock("t");

            if (model.HasTemporalTable() && !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodStartColumnName()))
            {
                lambdaBlock.AddStatement($"t.HasPeriodStart(\"{model.GetTemporalTable().PeriodStartColumnName()}\")", cfg => cfg.WithSemicolon());
            }

            if (model.HasTemporalTable() && !string.IsNullOrWhiteSpace(model.GetTemporalTable().PeriodEndColumnName()))
            {
                lambdaBlock.AddStatement($"t.HasPeriodEnd(\"{model.GetTemporalTable().PeriodEndColumnName()}\")", cfg => cfg.WithSemicolon());
            }

            if (model.HasTemporalTable() && !string.IsNullOrWhiteSpace(model.GetTemporalTable().HistoryTableName()))
            {
                lambdaBlock.AddStatement($"t.UseHistoryTable(\"{model.GetTemporalTable().HistoryTableName()}\")", cfg => cfg.WithSemicolon());
            }

            invocation.AddArgument(lambdaBlock);

            return invocation;
        }

        private static CSharpStatement GetUpdatedTableBuildAction(CSharpStatement existingTableBuilderAction, ClassModel model)
        {
            // get the parameter and body of the lambda
            var lambdaParts = existingTableBuilderAction.ToString().Split("=>");

            // should always be, but for some reason if the existing statement is not a lambda
            if (lambdaParts.Length == 2)
            {
                var parameter = lambdaParts[0].Trim();
                var bodyExpressions = lambdaParts[1].Split(";");

                var newLambda = new CSharpLambdaBlock(parameter);

                // add the existing statements back
                foreach (var expression in bodyExpressions)
                {
                    newLambda.AddStatement(expression, conf => conf.WithSemicolon());
                }

                newLambda.AddStatement(GetTemporalInvocationStatement(model));

                return newLambda;
            }

            return existingTableBuilderAction;
        }

    }
}