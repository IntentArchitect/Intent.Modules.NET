using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.AwsLambdaToolsDefaults
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AwsLambdaToolsDefaultsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     {
                        "Information": [
                            "This file provides default values for the deployment wizard inside Visual Studio and the AWS Lambda commands added to the .NET Core CLI.",
                            "To learn more about the Lambda commands with the .NET Core CLI execute the following command at the command line in the project root directory.",
                            "dotnet lambda help",
                            "All the command line options for the Lambda command can be specified in this file."
                         ],
                         "profile": "",
                         "region": "",
                         "configuration": "Release",
                         "s3-prefix": "{{ExecutionContext.GetApplicationConfig().Name}}/",
                         "template": "serverless.template",
                         "template-parameters": "",
                         "docker-host-build-output-dir": "./bin/Release/lambda-publish"
                     }
                     """;
        }
    }
}