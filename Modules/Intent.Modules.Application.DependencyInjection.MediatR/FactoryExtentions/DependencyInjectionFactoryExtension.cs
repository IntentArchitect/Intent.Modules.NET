using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.MediatR.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionFactoryExtension : FactoryExtensionBase
    {
        private readonly IList<ContainerRegistrationRequest> _containerRegistrationRequests = new List<ContainerRegistrationRequest>();

        public override string Id => "Intent.Application.DependencyInjection.MediatR.DependencyInjectionFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<ContainerRegistrationRequest>(HandleEvent);
        }

        private void HandleEvent(ContainerRegistrationRequest @event)
        {
            if (@event.Concern != "MediatR")
            {
                return;
            }

            @event.MarkAsHandled();
            _containerRegistrationRequests.Add(@event);
        }


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            RegisterServices(application);
        }

        string UseTypeOf(ICSharpFileBuilderTemplate template, string type)
        {
            var typeName = type.Substring("typeof(".Length, type.Length - "typeof()".Length);
            return $"typeof({template.UseType(typeName)})";
        }


        private void RegisterServices(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Application.DependencyInjection);
            if (template == null)
            {
                return;
            }
            template.AddNugetDependency(NugetPackages.MediatR(template.OutputTarget));
            application.EventDispatcher.Publish(new RemoveNugetPackageEvent("MediatR.Extensions.Microsoft.DependencyInjection", template.OutputTarget));

            template.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.Extensions.DependencyInjection");

                var method = file.Classes.First().FindMethod("AddApplication");

                method.AddInvocationStatement("services.AddMediatR", invocation =>
                {
                    invocation.AddMetadata("mediatr-config", true);
                    invocation.AddArgument(new CSharpLambdaBlock("cfg"), a =>
                    {
                        var options = (CSharpLambdaBlock)a;
                        options.AddStatement("cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());");

                        foreach (var registration in _containerRegistrationRequests.OrderBy(x => x.Priority))
                        {
                            foreach (var templateDependency in registration.TemplateDependencies)
                            {
                                var deptemplate = ((IntentTemplateBase)template).GetTemplate<IClassProvider>(templateDependency);
                                if (deptemplate != null)
                                {
                                    template.AddUsing(deptemplate.Namespace);
                                }

                                ((IntentTemplateBase)template).AddTemplateDependency(templateDependency);
                            }

                            foreach (var ns in registration.RequiredNamespaces)
                            {
                                template.AddUsing(ns);
                            }
                            options.AddStatement($"cfg.AddOpenBehavior({UseTypeOf(template, registration.ConcreteType)});");
                        }
                    });
                });
            });

        }
    }
}