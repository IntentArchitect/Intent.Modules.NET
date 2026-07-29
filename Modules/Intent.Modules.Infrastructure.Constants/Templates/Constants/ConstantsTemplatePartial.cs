using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using InfrastructureConstants = Intent.Modules.Constants.Infrastructure;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Infrastructure.Constants.Templates.Constants
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ConstantsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Infrastructure.Constants.ConstantsTemplate";
        private readonly Dictionary<string, string> _connectionStringNames = new(StringComparer.Ordinal);

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ConstantsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{OutputTarget.ApplicationName().ToPascalCase().Replace(".", "")}Constants", @class =>
                {
                    @class.Static();
                });

            ExecutionContext.EventDispatcher.Subscribe<InfrastructureRegisteredEvent>(HandleInfrastructureRegistered);

            CSharpFile.OnBuild(file =>
            {
                var constantsClass = file.Classes.First();

                foreach (var (connectionStringName, fieldName) in _connectionStringNames.OrderBy(x => x.Value))
                {
                    constantsClass.AddField("string", fieldName,
                        field => field.Constant($@"""{connectionStringName}"""));
                }

                UpdateInfrastructureDependencyInjection();
            }, 100);
        }

        private void UpdateInfrastructureDependencyInjection()
        {
            var dependencyInjectionTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                            TemplateRoles.Infrastructure.DependencyInjection);

            dependencyInjectionTemplate?.CSharpFile.AfterBuild(file =>
            {
                if (_connectionStringNames.Count == 0)
                {
                    return;
                }

                var method = file.Classes.First().FindMethod("AddInfrastructure");
                if (method == null)
                {
                    return;
                }

                var constantsTypeName = dependencyInjectionTemplate.GetTypeName(TemplateId);
                foreach (var statement in method.Statements.ToList())
                {
                    var replacementStatement = TryReplaceConnectionStringLiteral(statement, constantsTypeName);
                    if (replacementStatement is null)
                    {
                        continue;
                    }

                    method.InsertStatements(method.Statements.IndexOf(statement), replacementStatement.ConvertToStatements().ToList());
                    statement.Remove();
                }
            }, 1000);
        }

        private void HandleInfrastructureRegistered(InfrastructureRegisteredEvent @event)
        {
            var propertyName = GetConnectionStringPropertyName(@event.InfrastructureComponent);
            if (propertyName is null)
            {
                return;
            }

            if (@event.Properties.TryGetValue(propertyName, out var connectionStringName) &&
                !string.IsNullOrWhiteSpace(connectionStringName))
            {
                _connectionStringNames[connectionStringName] = connectionStringName.ToPascalCase();
            }
        }

        private static string? GetConnectionStringPropertyName(string infrastructureComponent)
        {
            return infrastructureComponent switch
            {
                var x when x == InfrastructureConstants.SqlServer.Name => InfrastructureConstants.SqlServer.Property.ConnectionStringName,
                var x when x == InfrastructureConstants.PostgreSql.Name => InfrastructureConstants.PostgreSql.Property.ConnectionStringName,
                var x when x == InfrastructureConstants.MySql.Name => InfrastructureConstants.MySql.Property.ConnectionStringName,
                var x when x == InfrastructureConstants.Oracle.Name => InfrastructureConstants.Oracle.Property.ConnectionStringName,
                var x when x == InfrastructureConstants.CosmosDb.Name => InfrastructureConstants.CosmosDb.Property.ConnectionStringName,
                _ => null
            };
        }

        private string? TryReplaceConnectionStringLiteral(CSharpStatement statement, string constantsTypeName)
        {
            var connectionStringStatement = (statement as IHasCSharpStatements)?.FindStatement(x => x.HasMetadata("is-connection-string"));
            if (connectionStringStatement == null)
            {
                return null;
            }

            var connectionStringText = connectionStringStatement.GetText(string.Empty);
            var match = _connectionStringNames
                .FirstOrDefault(kvp => connectionStringText.Contains($@"""{kvp.Key}""", StringComparison.Ordinal));
            if (match.Key is null)
            {
                return null;
            }

            var replacementExpression = connectionStringText.Replace(
                $@"""{match.Key}""",
                $"{constantsTypeName}.{match.Value}",
                StringComparison.Ordinal);

            return statement.GetText(string.Empty).Replace(
                connectionStringText,
                replacementExpression,
                StringComparison.Ordinal);
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
