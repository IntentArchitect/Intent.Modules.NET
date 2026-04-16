using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.UpdateProjectTags
{
    public class UpdateProjectTagsCommand : IRequest, ICommand
    {
        public UpdateProjectTagsCommand(List<string> tags)
        {
            Tags = tags;
        }

        public List<string> Tags { get; set; }
    }
}