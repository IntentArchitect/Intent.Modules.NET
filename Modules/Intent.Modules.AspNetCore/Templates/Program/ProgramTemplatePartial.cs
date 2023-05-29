using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.AspNetCore.Templates.Program
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class ProgramTemplate : CSharpTemplateBase<object, ProgramDecoratorBase>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Program";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Hosting")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddClass("Program", @class =>
                {
                    @class.AddMethod("void", "Main", method =>
                    {
                        method.Static();
                        method.AddParameter("string[]", "args");
                        method.AddStatement("CreateHostBuilder(args).Build().Run();",
                            stmt => stmt.AddMetadata("host-run", true));
                    });
                    @class.AddMethod("IHostBuilder", "CreateHostBuilder", method =>
                    {
                        method.Static();
                        method.AddParameter("string[]", "args");
                        method.WithExpressionBody(new CSharpMethodChainStatement("Host.CreateDefaultBuilder(args)")
                            .AddChainStatement(new CSharpInvocationStatement("ConfigureWebHostDefaults")
                                .AddArgument(new CSharpLambdaBlock("webBuilder")
                                    .AddStatement("webBuilder.UseStartup<Startup>();"))
                                .WithoutSemicolon()
                            ));
                    });
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
