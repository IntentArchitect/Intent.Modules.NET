using System.Collections.Generic;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class SecurityConfigurationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Swashbuckle.Interop.JWT.SecurityConfigurationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private readonly List<SwaggerOAuth2SchemeEvent> _swaggerSchemes;
        private string _stsPort = SchemeEventConstants.STS_Port_Tag;
        
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<SwaggerOAuth2SchemeEvent>(Handle);
            application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle);
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(SwashbuckleConfigurationTemplate.TemplateId));
            if (template != null)
            {
                
            }
        }
        
        private void Handle(SwaggerOAuth2SchemeEvent @event)
        {
            _swaggerSchemes.Add(@event);
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }
    }
}