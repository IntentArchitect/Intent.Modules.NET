using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ContinuousIntegration.AzurePipelines.Templates.AzurePipelines
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AzurePipelinesTemplate : IntentTemplateBase<object>, IDataFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ContinuousIntegration.AzurePipelines.azure-pipelines";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzurePipelinesTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            DataFile = new DataFile($"azure-pipelines", "../", overwriteBehaviour: OverwriteBehaviour.OnceOff)
                .WithYamlWriter(alwaysQuoteStrings: true)
                .WithRootObject(this, dictionary =>
                {
                    dictionary.WithBlankLinesBetweenItems();

                    dictionary.WithObject("trigger", trigger =>
                    {
                        trigger.WithObject("branches", branches =>
                        {
                            branches.WithArray("include", array =>
                            {
                                array.WithValue("*");
                            });
                        });
                    });

                    dictionary.WithObject("pool", pool =>
                    {
                        pool.WithValue("vmImage", "ubuntu-latest");
                    });

                    dictionary.WithArray("variables", variables =>
                    {
                        variables.WithObject(variable =>
                        {
                            variable.WithValue("name", "buildConfiguration");
                            variable.WithValue("value", "debug");
                        });

                        variables.WithObject(variable =>
                        {
                            variable.WithValue("name", "intentSolutionPath");
                            variable.WithValue("value", "intent");
                        });

                        variables.WithObject(variable =>
                        {
                            variable.CommentedOut();
                            variable.WithValue("group", "Intent Architect Credentials");
                        });
                    });

                    dictionary.WithArray("steps", steps =>
                    {
                        steps.WithBlankLinesBetweenItems();

                        steps.WithObject(step =>
                        {
                            step.WithValue("task", "DotNetCoreCLI@2");
                            step.WithValue("displayName", "dotnet build $(buildConfiguration)");
                            step.WithObject("inputs", inputs =>
                            {
                                inputs.WithValue("command", "build");
                                inputs.WithValue("projects", "**/*.csproj");
                                inputs.WithValue("arguments", "--configuration $(buildConfiguration)");
                            });
                        });

                        steps.WithObject(step =>
                        {
                            step.WithValue("task", "DotNetCoreCLI@2");
                            step.WithValue("displayName", "dotnet test");
                            step.WithObject("inputs", inputs =>
                            {
                                inputs.WithValue("command", "test");
                                inputs.WithValue("projects", "**/*Tests/*.csproj");
                                inputs.WithValue("arguments", "--configuration $(buildConfiguration)");
                            });
                        });

                        steps.WithObject(step =>
                        {
                            step.WithValue("task", "PowerShell@2");
                            step.WithValue("displayName", "install intent cli");
                            step.WithObject("inputs", inputs =>
                            {
                                inputs.WithValue("targetType", "inline");
                                inputs.WithValue("pwsh", true);
                                inputs.WithValue("script", "dotnet tool install Intent.SoftwareFactory.CLI --global");
                            });
                        });

                        steps.WithObject(step =>
                        {
                            step.WithValue("task", "PowerShell@2");
                            step.WithValue("displayName", "run intent cli");
                            step.WithObject("env", env =>
                            {
                                env.WithValue("INTENT_USER", "$(intent-architect-user)");
                                env.WithValue("INTENT_PASS", "$(intent-architect-password)");
                                env.WithValue("INTENT_SOLUTION_PATH", "$(intentSolutionPath)");
                            });
                            step.WithObject("inputs", inputs =>
                            {
                                inputs.WithValue("targetType", "inline");
                                inputs.WithValue("pwsh", true);
                                inputs.WithValue("script", """
                                                           if (($Env:INTENT_USER -Eq $null) -or ($Env:INTENT_USER -Like "`$(*")) {
                                                             Write-Host "##vso[task.logissue type=warning;]Intent Architect Credentials not configured, see https://github.com/IntentArchitect/Intent.Modules.NET/blob/development/Modules/Intent.Modules.ContinuousIntegration.AzurePipelines/README.md#configuring-intent-architect-credentials for more information."
                                                             Return
                                                           }
                                                           
                                                           intent-cli ensure-no-outstanding-changes "$Env:INTENT_USER" "$Env:INTENT_PASS" "$Env:INTENT_SOLUTION_PATH"
                                                           """);
                            });
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public IDataFile DataFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => DataFile.GetConfig();

        [IntentManaged(Mode.Fully)]
        public override string TransformText() => DataFile.ToString();
    }
}