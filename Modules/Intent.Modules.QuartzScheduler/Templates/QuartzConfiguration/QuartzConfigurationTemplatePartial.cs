using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.QuartzScheduler.Templates.ScheduledJob;
using Intent.QuartzScheduler.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.QuartzScheduler.Templates.QuartzConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QuartzConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.QuartzScheduler.QuartzConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QuartzConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(ScheduledJobTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Quartz")
                .AddClass($"QuartzConfiguration", @class =>
                {
                    if (ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.AspNetCore"))
                    {
                        AddNugetDependency(NugetPackages.QuartzAspNetCore(OutputTarget));
                    }
                    else
                    {
                        AddNugetDependency(NugetPackages.QuartzExtensionsHosting(OutputTarget));
                    }
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureQuartz", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");

                        method.AddMethodChainStatement("services", statement =>
                        {
                            var scheduledJobs = ExecutionContext.MetadataManager.Services(ExecutionContext.GetApplicationConfig().Id).GetScheduledJobModels();

                            var quartzLambda = new CSharpLambdaBlock("q");

                            foreach (var job in scheduledJobs)
                            {
                                quartzLambda.AddIfStatement($"configuration.GetValue<bool?>($\"Quartz:Jobs:{job.Name.ToPascalCase()}:Enabled\") ?? {job.GetScheduling().Enabled().ToString().ToLower()}", stmt =>
                                {
                                    var jobConfig = new CSharpInvocationStatement($"q.ScheduleJob<{GetTypeName(ScheduledJobTemplate.TemplateId, job)}>");
                                    jobConfig.AddArgument(
                                        new CSharpLambdaBlock("trigger")
                                            .AddStatement($"trigger.WithCronSchedule(configuration.GetValue<string?>($\"Quartz:Jobs:{job.Name.ToPascalCase()}:CronSchedule\") ?? \"{job.GetScheduling().CronSchedule().ToString().ToLower()}\");")
                                            .AddStatement($"trigger.WithIdentity(\"{job.Name}\");")); ;
                                    stmt.AddStatement(jobConfig);

                                });
                            }

                            statement
                                .AddChainStatement(new CSharpInvocationStatement("AddQuartz")
                                    .WithoutSemicolon()
                                    .AddArgument(quartzLambda)
                                );
                        });

                        method.AddMethodChainStatement("services", statement =>
                        {
                            statement
                                .AddChainStatement(new CSharpInvocationStatement("AddQuartzHostedService")
                                    .AddArgument(new CSharpLambdaBlock("options")
                                    .AddStatement("options.WaitForJobsToComplete = true;")
                                ).WithoutSemicolon());
                        });


                        method.AddStatement("return services;", s => s.SeparatedFromPrevious());
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureQuartz", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));
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