using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.ValidationBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValidationBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.ValidationBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidationBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddUsing("System.Linq");
            AddUsing("System.Threading");
            AddUsing("System.Threading.Tasks");
            AddNugetDependency(NugetPackages.FluentValidation(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"ValidationBehaviour", @class =>
                {
                    @class
                        .AddGenericParameter("TRequest")
                        .AddGenericParameter("TResponse")
                        .AddGenericTypeConstraint("TRequest", constraint =>
                        {
                            constraint.AddType("notnull");
                        });

                    @class.ImplementsInterface("IPipelineBehavior<TRequest, TResponse>");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IEnumerable<IValidator<TRequest>>", "validators", paramConfig =>
                        {
                            paramConfig.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod("Task<TResponse>", "Handle", method =>
                    {
                        method.Async();

                        method.AddParameter("TRequest", "request")
                            .AddParameter("RequestHandlerDelegate<TResponse>", "next")
                            .AddParameter("CancellationToken", "cancellationToken");

                        method.AddIfStatement("_validators.Any()", ifconfig =>
                        {
                            ifconfig.AddObjectInitStatement("var context", "new ValidationContext<TRequest>(request);")
                                .SeparatedFromNext();

                            ifconfig.AddObjectInitStatement("var validationResults", "await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));");
                            ifconfig.AddObjectInitStatement("var failures", "validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();").SeparatedFromNext();

                            ifconfig.AddIfStatement("failures.Count != 0", validationif =>
                            {
                                validationif.AddStatement("throw new ValidationException(failures);");
                            });
                        });

                        method.AddReturn("await next()");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ValidationBehaviour",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(4)
                .ForConcern("MediatR")
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(this));
        }
    }
}