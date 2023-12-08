using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Internal.EnvironmentVariableTest.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EnvironmentVarsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Modules.Internal.EnvironmentVariableTest.EnvironmentVarsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        record EnvironmentVariable(string EnvName, string EnvValue, string ProfileName, string RoleName);

        private EnvironmentVariable _environmentVariable = new EnvironmentVariable(string.Empty, string.Empty, string.Empty, string.Empty);

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            _environmentVariable = new EnvironmentVariable(settings["Environment Var Name"], settings["Environment Var Value"], settings["Target Profile"], settings["Target Role"]);
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.BeforeTemplateExecution"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            application.EventDispatcher.Publish(new EnvironmentVariableRegistrationRequest(
                _environmentVariable.EnvName,
                _environmentVariable.EnvValue,
                new string[] { _environmentVariable.ProfileName },
                _environmentVariable.RoleName)
            );
        }
    }
}