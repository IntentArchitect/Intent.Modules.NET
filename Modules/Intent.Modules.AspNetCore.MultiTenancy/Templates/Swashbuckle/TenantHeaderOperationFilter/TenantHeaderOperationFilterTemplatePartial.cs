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

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.Swashbuckle.TenantHeaderOperationFilter
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class TenantHeaderOperationFilterTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Modules.AspNetCore.MultiTenancy.Swashbuckle.TenantHeaderOperationFilter";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TenantHeaderOperationFilterTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"TenantHeaderOperationFilter",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            var configTemplate = ExecutionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.SwashbuckleConfiguration"));
            if (configTemplate == null)
            {
                return;
            }

            var priClass = configTemplate.CSharpFile.Classes.First();
            var method = priClass.FindMethod("ConfigureSwagger");
            var invocation = (CSharpInvocationStatement)method.FindStatement(stmt => stmt.GetText("").Contains("AddSwaggerGen"));
            var lambda = (CSharpLambdaBlock)invocation.Statements.First();
            lambda.AddStatement($"options.OperationFilter<{GetTypeName(this)}>();");
        }
    }
}