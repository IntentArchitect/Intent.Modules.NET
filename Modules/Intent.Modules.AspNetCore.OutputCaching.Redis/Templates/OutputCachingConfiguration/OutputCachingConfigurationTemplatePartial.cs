using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.OutputCaching.Redis.Api;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.Templates.OutputCachingConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OutputCachingConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.OutputCaching.Redis.OutputCachingConfiguration";

        [IntentManaged(Mode.Ignore)]
        public OutputCachingConfigurationTemplate(IOutputTarget outputTarget, IList<CachingPolicyModel> models = null) : base(TemplateId, outputTarget, models)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"OutputCachingConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureOutputCaching", method =>
                    {
                        method
                            .Static()
                            .AddParameter("IServiceCollection", "services", p => p.WithThisModifier())
                            .AddParameter("IConfiguration", "configuration")
                            ;
                        method.AddStatement("var redisConfig = configuration.GetSection(\"OuputCaching\").Get<OutputCachingConfig>();");
                        method.AddIfStatement("redisConfig == null", stmt => { stmt.AddStatement("throw new Exception(\"Missing 'OuputCaching' configuration in appsettings.json \");"); });
                        method.AddInvocationStatement("services.AddStackExchangeRedisOutputCache", stmt => stmt
                            .AddArgument(new CSharpLambdaBlock("options")
                                .AddStatement($@"options.Configuration = redisConfig.Configuration;")
                                .AddStatement($@"options.InstanceName = redisConfig.InstanceName;")));
                        method.AddInvocationStatement("services.AddOutputCache", stmt =>
                        {
                            stmt.AddArgument(new CSharpLambdaBlock("options"), x =>
                            {
                                var lambda = (CSharpLambdaBlock)x;
                                lambda.AddStatement($@"options.AddBasePolicy(b => b.NoCache());");
                                foreach (var cachingPolicy in models)
                                {
                                    var builderFluent = new CSharpMethodChainStatement("builder").WithoutSemicolon();
                                    var config = cachingPolicy.GetCachingConfig();
                                    if (config == null)
                                    {
                                        throw new ElementException(cachingPolicy.InternalElement, "Missing 'Caching Config' stereotype.");
                                    }
                                    if (config.NoCaching())
                                    {
                                        builderFluent.AddChainStatement($"NoCache()");
                                    }
                                    else
                                    {
                                        if (config.Duration() is not null)
                                        {
                                            builderFluent.AddChainStatement($"Expire(TimeSpan.FromSeconds({config.Duration()}))");
                                        }
                                        if (!string.IsNullOrEmpty(config.Tags()))
                                        {
                                            string tags = string.Join(", ", config.Tags().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"Tag({tags})");
                                        }
                                        if (!string.IsNullOrEmpty(config.VaryByRouteValueNames()))
                                        {
                                            string routeValueNames = string.Join(", ", config.VaryByRouteValueNames().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"SetVaryByRouteValue({routeValueNames})");
                                        }
                                        if (!string.IsNullOrEmpty(config.VaryByQueryKeys()))
                                        {
                                            string queryKeys = string.Join(", ", config.VaryByQueryKeys().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"SetVaryByQuery({queryKeys})");
                                        }
                                        if (!string.IsNullOrEmpty(config.VaryByHeaderNames()))
                                        {
                                            string headerNames = string.Join(", ", config.VaryByHeaderNames().Split(",").Select(x => $"\"{x.Trim()}\""));
                                            builderFluent.AddChainStatement($"SetVaryByHeader({headerNames})");
                                        }
                                    }
                                    lambda.AddInvocationStatement("options.AddPolicy", policy =>
                                    {
                                        policy.AddArgument($"\"{cachingPolicy.Name}\"");
                                        policy.AddArgument(new CSharpLambdaBlock("builder").WithExpressionBody(builderFluent));
                                    });
                                }

                            });
                        });

                        method.AddStatement("return services;");
                    });
                    @class.AddNestedClass($"OutputCachingConfig", child =>
                     {
                         child.Private();
                         child.AddProperty("string?", "Configuration");
                         child.AddProperty("string?", "InstanceName");
                     });
                });
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate()) { return; }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureOutputCaching", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));

            //This need to be called after UseCors(-2) and UseRouting()
            //https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-8.0#add-the-middleware-to-the-app
            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseOutputCache")
                .WithPriority(-1));


            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("OuputCaching", new
            {
                Configuration = "localhost:6379",
                InstanceName = "SampleInstance",
            }));

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