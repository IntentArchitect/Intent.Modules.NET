using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge)]
    public partial class RepositoryInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Entities.Repositories.Api.RepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RepositoryInterfaceTemplate(IOutputTarget outputTarget, object model = null)
            : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface("IRepository", @interface =>
                {
                    @interface.AddAttribute("[IntentManaged(Mode.Fully, Signature = Mode.Fully)]");
                    @interface
                        .AddGenericParameter("TDomain", out var tDomain, genericParameter => genericParameter.Contravariant())
                        .AddMethod("void", "Add", method => method
                            .AddParameter(tDomain, "entity")
                        )
                        .AddMethod("void", "Update", method => method
                            .AddParameter(tDomain, "entity")
                        )
                        .AddMethod("void", "Remove", method => method
                            .AddParameter(tDomain, "entity")
                        )
                        ;
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public CSharpFile CSharpFile { get; }
    }
}
