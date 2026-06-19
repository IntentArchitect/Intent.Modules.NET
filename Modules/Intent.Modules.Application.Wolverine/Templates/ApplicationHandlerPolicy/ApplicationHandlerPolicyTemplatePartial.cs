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

namespace Intent.Modules.Application.Wolverine.Templates.ApplicationHandlerPolicy
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApplicationHandlerPolicyTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.ApplicationHandlerPolicy";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApplicationHandlerPolicyTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.WolverineFx(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.Extensions.DependencyInjection")
                .AddUsing("Wolverine")
                .AddUsing("Wolverine.Runtime.Handlers")
                .AddClass("ApplicationHandlerPolicy", @class =>
                {
                    var commandType = GetTypeName("Intent.Application.Wolverine.CommandInterface");
                    var queryType = GetTypeName("Intent.Application.Wolverine.QueryInterface");
                    var authMiddleware = GetTypeName("Intent.Application.Wolverine.AuthorizationMiddleware");
                    var valMiddleware = GetTypeName("Intent.Application.Wolverine.ValidationMiddleware");
                    var logMiddleware = GetTypeName("Intent.Application.Wolverine.LoggingMiddleware");
                    var perfMiddleware = GetTypeName("Intent.Application.Wolverine.PerformanceMiddleware");
                    var errMiddleware = GetTypeName("Intent.Application.Wolverine.UnhandledExceptionMiddleware");
                    var uowMiddleware = GetTypeName("Intent.Application.Wolverine.UnitOfWorkMiddleware");

                    @class.Internal().Static();

                    @class.AddMethod("void", "Apply", method =>
                    {
                        method.Internal().Static();
                        method.AddParameter("WolverineOptions", "opts");

                        method.AddStatement($"opts.Policies.AddMiddleware<{authMiddleware}>(IsApplicationMessage);");
                        method.AddStatement($"opts.Policies.AddMiddleware<{valMiddleware}>(IsApplicationMessage);");
                        method.AddStatement($"opts.Policies.AddMiddleware<{logMiddleware}>(IsApplicationMessage);");
                        method.AddStatement($"opts.Policies.AddMiddleware<{perfMiddleware}>(IsApplicationMessage);");
                        method.AddStatement($"opts.Policies.AddMiddleware<{errMiddleware}>(IsApplicationMessage);");
                        method.AddStatement($"opts.Policies.AddMiddleware<{uowMiddleware}>(c => typeof({commandType}).IsAssignableFrom(c.MessageType));");
                        method.AddStatement("");
                        method.AddStatement($"opts.Services.AddTransient<{authMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{valMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{logMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{perfMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{errMiddleware}>();");
                        method.AddStatement($"opts.Services.AddTransient<{uowMiddleware}>();");
                    });

                    @class.AddMethod("bool", "IsApplicationMessage", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("HandlerChain", "chain");
                        method.AddStatement($"return typeof({commandType}).IsAssignableFrom(chain.MessageType) ||");
                        method.AddStatement($"    typeof({queryType}).IsAssignableFrom(chain.MessageType);");
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
