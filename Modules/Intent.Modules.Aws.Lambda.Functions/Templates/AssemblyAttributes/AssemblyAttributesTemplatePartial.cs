using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.AssemblyAttributes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AssemblyAttributesTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Aws.Lambda.Functions.AssemblyAttributes";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AssemblyAttributesTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Diagnostics.CodeAnalysis")
                .OnBuild(file =>
                {
                    var lambdaFunctionTemplates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Aws.Lambda.Function").ToList();
                    if (lambdaFunctionTemplates?.Any() != true)
                    {
                        return;
                    }

                    var namespaces = lambdaFunctionTemplates.Select(x => x.Namespace).Distinct().ToList();
                    
                    foreach (var @namespace in namespaces.Order())
                    {
                        file.AddAssemblyAttribute("SuppressMessage", a =>
                        {
                            a.AddArgument("\"Formatting\"");
                            a.AddArgument("\"IDE0130:Namespace does not match folder structure.\"");
                            a.AddArgument($"Target = \"{@namespace}\"");
                            a.AddArgument("Scope = \"namespaceanddescendants\"");
                            a.AddArgument("Justification = \"Reducing namespace length should alleviate Lambda Annotation error messages about 127 character limits.\"");
                        });
                    }
                });
        }

        public override bool CanRunTemplate()
        {
            var lambdaFunctionTemplates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Aws.Lambda.Function").ToList();
            return lambdaFunctionTemplates?.Any() == true;
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "AssemblyAttributes",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}