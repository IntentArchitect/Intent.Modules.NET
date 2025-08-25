using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ProjectReferenceModelAssociationExtensions
    {
        [IntentManaged(Mode.Fully)]
        public static IList<ProjectReferenceSourceEndModel> ProjectReferenceSources(this CSharpProjectNETModel model)
        {
            return model.InternalElement.AssociatedElements
                .Where(x => x.Association.SpecializationType == ProjectReferenceModel.SpecializationType && x.IsSourceEnd())
                .Select(x => ProjectReferenceModel.CreateFromEnd(x).SourceEnd)
                .ToList();
        }

    }
}