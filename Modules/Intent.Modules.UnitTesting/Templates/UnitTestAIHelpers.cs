using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AI;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.UnitTesting.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

namespace Intent.Modules.UnitTesting.Templates
{
    internal class UnitTestAITaskProvider(Func<IChange[], IOutputFile[], IAITask?> createTask) : IAITaskProvider
    {
        public IAITask[] GetTasks(IChange[] changes, IOutputFile[] outputFiles)
        {
            var task = createTask(changes, outputFiles);
            return task != null ? [task] : [];
        }
    }

    internal class UnitTestAITask : IAITask
    {
        private readonly IIntentTemplate _template;

        public UnitTestAITask(IIntentTemplate template, ITemplate implementationTemplate)
        {
            Id = ((IntentTemplateBase)template).GetCorrelationId()
                ?? throw new ArgumentException("CorrelationId could not be found for template", nameof(template));
            _template = template;

            var ownDeps = _template.GetAllTemplateDependencies()
                .Select(x => _template.ExecutionContext.FindTemplateInstance(x))
                .Where(x => x is not null).Cast<ITemplate>();

            var implDeps = implementationTemplate is IIntentTemplate intentImplTemplate
                ? intentImplTemplate.GetAllTemplateDependencies()
                    .Select(x => _template.ExecutionContext.FindTemplateInstance(x))
                    .Where(x => x is not null).Cast<ITemplate>()
                : [];

            RelatedTemplates = ownDeps
                .Union(implDeps)
                .Append(implementationTemplate)
                .Distinct()
                .ToList();
        }

        public string Id { get; }
        public ITemplate Template => _template;
        public string Type { get; init; }
        public string Title { get; init; }
        public string Instructions { get; init; }
        public string Context { get; init; }
        public IList<ITemplate> RelatedTemplates { get; }
        public IList<string> FilesToInclude { get; }
    }

    internal static class UnitTestMockHelpers
    {
        public static string GetMockFrameworkName(UnitTestSettings.MockFrameworkOptionsEnum mockFramework)
        {
            return mockFramework switch
            {
                UnitTestSettings.MockFrameworkOptionsEnum.Nsubstitute => "NSubstitute",
                UnitTestSettings.MockFrameworkOptionsEnum.Moq => "Moq",
                _ => "Unknown Mock Framework"
            };
        }

        public static string GetMockFrameworkContext(UnitTestSettings.MockFrameworkOptionsEnum mockFramework)
        {
            return mockFramework switch
            {
                UnitTestSettings.MockFrameworkOptionsEnum.Nsubstitute =>
                    """
                    ## NSubstitute Patterns
                    - Create mocks: `var _repositoryMock = Substitute.For<IRepository>();`
                    - Setup return: `_repositoryMock.FindByIdAsync(id, Arg.Any<CancellationToken>()).Returns(entity);`
                    - Setup async return: `_repositoryMock.FindByIdAsync(id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(entity));`
                    - Verify call: `await _repositoryMock.Received(1).FindByIdAsync(id, Arg.Any<CancellationToken>());`
                    - Verify not called: `_repositoryMock.DidNotReceive().Remove(Arg.Any<Entity>());`
                    - Capture callback: `_repositoryMock.When(x => x.Add(Arg.Any<Entity>())).Do(x => captured = x.Arg<Entity>());`
                    - Constructor: Pass mocks directly without `.Object` suffix
                    """,
                UnitTestSettings.MockFrameworkOptionsEnum.Moq =>
                    """
                    ## Moq Patterns
                    - Create mocks: `var _repositoryMock = new Mock<IRepository>();`
                    - Setup return: `_repositoryMock.Setup(x => x.FindByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);`
                    - Verify call: `_repositoryMock.Verify(x => x.FindByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);`
                    - Verify not called: `_repositoryMock.Verify(x => x.Remove(It.IsAny<Entity>()), Times.Never);`
                    - Capture callback: `_repositoryMock.Setup(x => x.Add(It.IsAny<Entity>())).Callback<Entity>(e => captured = e);`
                    - Constructor: Pass mocks using `.Object` property (e.g., `_repositoryMock.Object`)
                    """,
                _ => "Could not determine configured Mock framework."
            };
        }
    }
}
