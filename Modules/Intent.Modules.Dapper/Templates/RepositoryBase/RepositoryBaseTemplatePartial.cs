using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Dapper.Templates.RepositoryInterface;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates.RepositoryBase
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RepositoryBaseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapper.RepositoryBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryBaseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.Dapper(OutputTarget));
            AddNugetDependency(NugetPackages.SystemDataSqlClient(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Data")
                .AddUsing("System.Data.SqlClient")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddClass($"RepositoryBase")
                .OnBuild(file =>
                {
                    string nullableChar = this.OutputTarget.GetProject().NullableEnabled ? "?" : "";

                    var @class = file.Classes.First();
                    @class
                        .Abstract()
                        .AddGenericParameter("TDomain", out var tDomain);
                    @class.AddGenericTypeConstraint(tDomain, constr => constr.AddType("class"));

                    @class.AddField("string", "_connectionString", f => f.PrivateReadOnly());

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IConfiguration", "configuration");
                        ctor.AddStatement("_connectionString = configuration.GetConnectionString(\"DefaultConnection\") ?? throw new Exception(\"No `DefaultConnection` connection string configured\");");
                    });

                    @class.AddMethod("IDbConnection", "GetConnection", method =>
                    {
                        method.Protected();
                        method.AddStatement("var result = new SqlConnection(_connectionString);");
                        method.AddStatement("result.Open();");
                        method.AddStatement("return result;");
                    });

                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("ConnectionStrings:DefaultConnection",
                $"Server=.;Initial Catalog={OutputTarget.ApplicationName()};Integrated Security=true;MultipleActiveResultSets=True{GetSqlServerExtendedConnectionString(OutputTarget.GetProject())}"
                ));

        }
        private static string GetSqlServerExtendedConnectionString(ICSharpProject project)
        {
            return project.TryGetMaxNetAppVersion(out var version) && version.Major >= 7
                ? ";Encrypt=False"
                : string.Empty;
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