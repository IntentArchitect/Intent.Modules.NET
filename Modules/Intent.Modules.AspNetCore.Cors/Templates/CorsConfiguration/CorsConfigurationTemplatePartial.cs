using System;
using System.Collections.Generic;
using Intent.Engine;
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

namespace Intent.Modules.AspNetCore.Cors.Templates.CorsConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CorsConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Cors.CorsConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CorsConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("Microsoft.AspNetCore.Cors.Infrastructure")
                .AddUsing("Microsoft.Extensions.Configuration")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddClass($"CorsConfiguration", @class =>
                {
                    @class.Static();
                    @class.AddMethod("IServiceCollection", "ConfigureCors", method =>
                    {
                        method.Static();
                        method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                        method.AddParameter("IConfiguration", "configuration");
                        method.AddStatement("var corsPolicies = configuration.GetSection(\"CorsPolicies\").Get<CorsPolicies>();");
                        method.AddIfStatement("corsPolicies == null", @if =>
                        {
                            @if.SeparatedFromPrevious(false);
                            @if.AddStatement("return services;");
                        });

                        method.AddMethodChainStatement("services", statement =>
                        {
                            statement
                                .AddChainStatement(new CSharpInvocationStatement("AddCors")
                                    .WithoutSemicolon()
                                    .AddArgument(new CSharpLambdaBlock("options")
                                        .AddIfStatement("corsPolicies.Default != null", @if =>
                                        {
                                            @if.AddStatement("options.AddDefaultPolicy(policy => ApplyOptions(policy, corsPolicies.Default));");
                                        })
                                        .AddForEachStatement(
                                            "(name, policyOptions)",
                                            "corsPolicies.Named ?? new Dictionary<string, PolicyOptions>()",
                                            @for =>
                                            {
                                                @for.AddStatement("options.AddPolicy(name, policy => ApplyOptions(policy, policyOptions));");
                                            })
                                    )
                                );
                        });

                        method.AddStatement("return services;", s => s.SeparatedFromPrevious());
                    });

                    @class.AddMethod("void", "ApplyOptions", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("CorsPolicyBuilder", "policy");
                        method.AddParameter("PolicyOptions", "options");
                        method.AddMethodChainStatement("policy", methodChain =>
                        {
                            methodChain
                                .AddChainStatement(new CSharpInvocationStatement("WithOrigins")
                                    .AddArgument("options.Origins ?? Array.Empty<string>()")
                                    .WithoutSemicolon()
                                );
                            methodChain
                                .AddChainStatement(new CSharpInvocationStatement("WithMethods")
                                    .AddArgument("options.Methods ?? Array.Empty<string>()")
                                    .WithoutSemicolon()
                                );
                            methodChain
                                .AddChainStatement(new CSharpInvocationStatement("WithHeaders")
                                    .AddArgument("options.Headers ?? Array.Empty<string>()")
                                    .WithoutSemicolon()
                                );
                            methodChain
                                .AddChainStatement(new CSharpInvocationStatement("WithExposedHeaders")
                                    .AddArgument("options.ExposedHeaders ?? Array.Empty<string>()")
                                    .WithoutSemicolon()
                                );
                        });

                        method.AddIfStatement(
                            "options.AllowCredentials",
                            @if => @if.AddStatement("policy.AllowCredentials();"));

                        method.AddIfStatement(
                            "options.PreflightMaxAge != null",
                            @if => @if.AddStatement("policy.SetPreflightMaxAge(options.PreflightMaxAge.Value);"));
                    });

                    @class.AddNestedClass("CorsPolicies", nestedClass =>
                    {
                        nestedClass.Private();
                        nestedClass.AddProperty("PolicyOptions?", "Default");
                        nestedClass.AddProperty("Dictionary<string, PolicyOptions>?", "Named");
                    });

                    @class.AddNestedClass("PolicyOptions", nestedClass =>
                    {
                        nestedClass.Private();
                        nestedClass.AddProperty("string[]?", "Origins");
                        nestedClass.AddProperty("string[]?", "Methods");
                        nestedClass.AddProperty("string[]?", "Headers");
                        nestedClass.AddProperty("string[]?", "ExposedHeaders");
                        nestedClass.AddProperty("bool", "AllowCredentials");
                        nestedClass.AddProperty("TimeSpan?", "PreflightMaxAge");
                    });
                });
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("ConfigureCors", ServiceConfigurationRequest.ParameterType.Configuration)
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(ApplicationBuilderRegistrationRequest
                .ToRegister("UseCors")
                .WithPriority(-2));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("CorsPolicies", new
            {
                Default = new
                {
                    Origins = new[] { "*" },
                    Methods = new[] { "*" },
                    Headers = new[] { "*" }
                }
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