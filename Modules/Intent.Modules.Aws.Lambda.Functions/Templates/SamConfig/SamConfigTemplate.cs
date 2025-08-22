using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.SamConfig
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SamConfigTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $"""
                    version = 0.1
                    
                    [default.deploy.parameters]
                    stack_name = "{ExecutionContext.GetApplicationConfig().Name}"
                    resolve_s3 = true
                    s3_prefix = "{ExecutionContext.GetApplicationConfig().Name}"
                    region = "us-east-1"
                    confirm_changeset = true
                    capabilities = "CAPABILITY_IAM"
                    image_repositories = []
                    """;
        }
    }
}