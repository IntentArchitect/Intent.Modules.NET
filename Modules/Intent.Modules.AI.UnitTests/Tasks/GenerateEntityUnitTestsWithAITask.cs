using System;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.ModuleTask", Version = "1.0")]

namespace Intent.Modules.AI.UnitTests.Tasks
{
    [IntentManaged(Mode.Merge)]
    public class GenerateEntityUnitTestsWithAITask : IModuleTask
    {
        [IntentManaged(Mode.Merge)]
        public GenerateEntityUnitTestsWithAITask()
        {
        }

        public string TaskTypeId => "Intent.AI.UnitTests.GenerateEntityUnitTestsWithAITask";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string TaskTypeName => "Generate entity unit tests with AI";
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public int Order => 0;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public string Execute(params string[] args)
        {
            // IntentInitialGen
            // TODO: Implement GenerateEntityUnitTestsWithAITask.Execute(...) functionality
            throw new NotImplementedException("Implement your handler logic here...");
        }
    }
}