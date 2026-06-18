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

namespace Intent.Modules.Application.Wolverine.Templates.WolverineConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class WolverineConfigurationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.WolverineConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public WolverineConfigurationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.WolverineFx(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Wolverine")
                .AddClass("WolverineConfiguration", @class =>
                {
                    var commandType = GetTypeName("Intent.Application.Wolverine.CommandInterface");
                    var handlerPolicyType = GetTypeName("Intent.Application.Wolverine.ApplicationHandlerPolicy");
                    var authMiddleware = GetTypeName("Intent.Application.Wolverine.AuthorizationMiddleware");
                    var valMiddleware = GetTypeName("Intent.Application.Wolverine.ValidationMiddleware");
                    var logMiddleware = GetTypeName("Intent.Application.Wolverine.LoggingMiddleware");
                    var perfMiddleware = GetTypeName("Intent.Application.Wolverine.PerformanceMiddleware");
                    var errMiddleware = GetTypeName("Intent.Application.Wolverine.UnhandledExceptionMiddleware");
                    var uowMiddleware = GetTypeName("Intent.Application.Wolverine.UnitOfWorkMiddleware");

                    @class.Static();

                    @class.AddMethod("void", "Configure", method =>
                    {
                        method.Static();
                        method.AddParameter("WolverineOptions", "opts");
                        method.AddStatement($"opts.Discovery.IncludeAssembly(typeof({commandType}).Assembly);");
                        method.AddStatement($"{handlerPolicyType}.Apply(opts);");
                        method.AddStatement($"opts.Services.AddTransient<{authMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{valMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{logMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{perfMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{errMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{uowMiddleware}>();");
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