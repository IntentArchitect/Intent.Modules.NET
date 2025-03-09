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

namespace Intent.Modules.AspNetCore.Grpc.Templates.GrpcConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class GrpcConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.GrpcConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GrpcConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.GrpcAspNetCore(outputTarget));
            AddNugetDependency(NugetPackages.GrpcAspNetCoreServerReflection(outputTarget));
            AddNugetDependency(NugetPackages.GrpcStatusProto(outputTarget));

            var serviceTemplates = new List<ICSharpTemplate>();

            ExecutionContext.EventDispatcher.Subscribe<RegisterGrpcService>(@event => serviceTemplates.Add(@event.Template));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Builder")
                .AddUsing("Microsoft.AspNetCore.Routing")
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Microsoft.Extensions.Hosting")
                .AddClass($"GrpcConfiguration", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IServiceCollection", "ConfigureGrpc", method =>
                    {
                        method.Static();
                        method.AddInvocationStatement("services.AddGrpc", invocation =>
                        {
                            method.AddParameter("IServiceCollection", "services", p => p.WithThisModifier());
                            invocation.AddLambdaBlock("options", block =>
                            {
                                block.AddStatement($"options.Interceptors.Add<{this.GetGrpcExceptionInterceptorName()}>();");
                            });
                        });

                        method.AddStatement("services.AddGrpcReflection();");

                        method.AddReturn("services");
                    });

                    @class.AddMethod("IEndpointRouteBuilder", "MapGrpcServices", method =>
                    {
                        method.Static();
                        method.AddParameter("WebApplication", "app", p => p.WithThisModifier());

                        foreach (var template in serviceTemplates)
                        {
                            method.AddStatement($"app.MapGrpcService<{GetTypeName(template)}>();");
                        }

                        method.AddIfStatement("app.Environment.IsDevelopment()", @if =>
                        {
                            @if.AddStatement("app.MapGrpcReflectionService();");
                        });

                        method.AddReturn("app", s => s.SeparatedFromPrevious());
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