using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Dapper.Templates.EntityRepositoryInterface;
using Intent.Modules.Dapper.Templates.RepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Domain;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates.Repository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapper.Repository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Linq")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Dapper")
                .AddClass($"{Model.Name}Repository", @class =>
                {
                    @class.AddMetadata("model", model);
                    @class.AddAttribute("[IntentManaged(Mode.Merge, Signature = Mode.Fully)]");
                    @class.WithBaseType($"RepositoryBase<{EntityName}>");
                    @class.ImplementsInterface(this.GetEntityRepositoryInterfaceName());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration");
                        ctor.CallsBase(b => b.AddArgument("configuration"));
                    });

                    var pks = model.GetPks();

                    @class.AddMethod($"Task", "AddAsync", method =>
                    {
                        method
                            .Async()
                            .AddParameter(EntityName, "entity")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));

                        method.AddUsingBlock("var connection = GetConnection()", stmt =>
                        {
                            var sqlStatement = CreateInsertStatement(model);
                            stmt.AddStatement($"var sql = @\"{sqlStatement}\";", s => s.SeparatedFromNext());
                            if (pks.Count == 1)
                            {
                                var pk = pks[0];
                                stmt.AddStatement($"var newId = await connection.QuerySingleAsync<{GetTypeName(pk)}>(sql, entity);");
                                stmt.AddStatement("entity.Id = newId;");
                            }
                            else
                            {
                                stmt.AddStatement($"await connection.ExecuteAsync(sql, entity);");
                            }

                        });

                    });
                    @class.AddMethod($"Task", "UpdateAsync", method =>
                    {
                        method
                            .Async()
                            .AddParameter(EntityName, "entity")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));

                        var sqlStatement = CreateUpdateStatement(model);

                        method.AddUsingBlock("var connection = GetConnection()", stmt =>
                        {
                            stmt.AddStatement($"var sql = @\"{sqlStatement}\";", s => s.SeparatedFromNext());
                            stmt.AddStatement($"await connection.ExecuteAsync(sql, entity);");
                        });

                    });
                    @class.AddMethod($"Task", "RemoveAsync", method =>
                    {
                        method
                            .Async()
                            .AddParameter(EntityName, "entity")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        method.AddUsingBlock("var connection = GetConnection()", stmt =>
                        {
                            var clauses = new Clauses(pks, PascalCase, "entity");
                            stmt.AddStatement($"var sql = \"DELETE FROM {model.SqlTableName()} WHERE {clauses.MatchClause}\";", s => s.SeparatedFromNext());
                            stmt.AddStatement($"await connection.ExecuteAsync(sql, new {{{clauses.InitClause}}});");
                        });
                    });
                    @class.AddMethod($"Task<{EntityName}?>", "FindByIdAsync", method =>
                    {

                        Clauses clauses;
                        if (pks.Count == 1)
                        {
                            clauses = new Clauses(pks, CamelCase);
                            var pk = pks.First();
                            method.AddParameter(GetTypeName(pk.TypeReference), pk.Name.ToCamelCase());
                        }
                        else
                        {
                            clauses = new Clauses(pks, PascalCase, "id");
                            method.AddParameter($"({string.Join(", ", pks.Select(pk => $"{GetTypeName(pk)} {pk.Name.ToPascalCase()}"))})", "id");
                        }
                        method
                            .Async()
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        method.AddUsingBlock("var connection = GetConnection()", stmt =>
                        {

                            stmt.AddStatement($"var sql = \"SELECT * FROM {model.SqlTableName()} WHERE {clauses.MatchClause}\";", s => s.SeparatedFromNext());
                            stmt.AddStatement($"return await connection.QuerySingleOrDefaultAsync<{EntityName}>(sql, new {{{clauses.InitClause}}});");
                        });
                    });
                    @class.AddMethod($"Task<List<{EntityName}>>", "FindAllAsync", method =>
                    {
                        method
                            .Async()
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"));
                        method.AddUsingBlock("var connection = GetConnection()", stmt =>
                        {
                            stmt.AddStatement($"var sql = \"SELECT * FROM {model.SqlTableName()}\";", s => s.SeparatedFromNext());
                            stmt.AddStatement($"var result = await connection.QueryAsync<{EntityName}>(sql);");
                            stmt.AddStatement("return result.ToList();");

                        });
                    });
                })
                .AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var method in @class.Methods)
                    {
                        if (!method.Statements.Any())
                        {
                            method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                            method.AddStatement($"""throw new {UseType("System.NotImplementedException")}("Your implementation here...");""");
                        }
                    }
                }, 1000);
        }

        private string CreateUpdateStatement(ClassModel model)
        {
            var pks = model.GetPks();
            var sqlStatement = new StringBuilder();
            sqlStatement.AppendLine();
            sqlStatement.AppendLine($"UPDATE {model.SqlTableName()} SET");
            var columns = model.Attributes.Where(a => !a.HasStereotype("Primary Key")).ToList();
            for (int i = 0; i < columns.Count; i++)
            {
                var attribute = columns[i];
                var lastColumn = i == columns.Count - 1;
                sqlStatement.AppendLine($"    {attribute.ColumnName()} = @{attribute.Name}{(lastColumn ? "" : ",")}");

            }
            var clauses = new Clauses(pks, PascalCase, "entity");
            sqlStatement.AppendLine($"WHERE {clauses.MatchClause}");
            return sqlStatement.ToString();
        }

        private string CreateInsertStatement(ClassModel model)
        {
            var pks = model.GetPks();

            var sqlStatement = new StringBuilder();
            sqlStatement.AppendLine();
            sqlStatement.AppendLine($"INSERT INTO {model.SqlTableName()}");
            sqlStatement.AppendLine($"({string.Join(", ", model.Attributes.Where(a => !a.HasStereotype("Primary Key")).Select(x => x.Name))})");
            if (pks.Count == 1)
            {
                sqlStatement.AppendLine($"OUTPUT Inserted.{pks[0].ColumnName()}");

            }
            sqlStatement.AppendLine($"VALUES");
            sqlStatement.AppendLine($"({string.Join(", ", model.Attributes.Where(a => !a.HasStereotype("Primary Key")).Select(x => $"@{x.ColumnName()}"))})");
            return sqlStatement.ToString();
        }

        private string CamelCase(string s)
        {
            return s.ToCamelCase();
        }
        private string PascalCase(string s)
        {
            return s.ToPascalCase();
        }

        private record Clauses
        {
            public Clauses(IList<AttributeModel> pks, Func<string, string> casing = null, string initPrefix = null)
            {
                MatchClause = string.Join("AND ", pks.Select(pk => $"{pk.ColumnName()} = @{pk.Name}"));
                InitClause = string.Join(", ", pks.Select(pk => $"{pk.Name} = {(initPrefix == null ? "" : $"{initPrefix}.")}{(casing != null ? casing(pk.Name) : pk.Name)}"));
            }

            internal string MatchClause { get; }
            internal string InitClause { get; }
        }

        public override void BeforeTemplateExecution()
        {
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate));
        }

        public string EntityName => GetTypeName("Domain.Entity", Model);

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

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