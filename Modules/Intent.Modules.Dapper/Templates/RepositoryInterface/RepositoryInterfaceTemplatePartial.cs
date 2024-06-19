using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapper.Templates.RepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RepositoryInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapper.RepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IDapperRepository", @interface =>
                {
                    string nullableChar = this.OutputTarget.GetProject().NullableEnabled ? "?" : "";

                    @interface
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddMethod($"Task", "AddAsync", method => method
                            .AddParameter(tDomain, "entity")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task", "UpdateAsync", method => method
                            .AddParameter(tDomain, "entity")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                        .AddMethod($"Task", "RemoveAsync", method => method
                            .AddParameter(tDomain, "entity")
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        )
                            .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter("CancellationToken", "cancellationToken", x => x.WithDefaultValue("default"))
                        );
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