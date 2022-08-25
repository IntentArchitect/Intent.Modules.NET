using Intent.Modules.VisualStudio.Projects.Templates;

namespace Intent.Modules.VisualStudio.Projects.Events
{
    public class VisualStudioProjectCreatedEvent
    {
        public IVisualStudioProjectTemplate TemplateInstance { get; }

        public VisualStudioProjectCreatedEvent(IVisualStudioProjectTemplate templateInstance)
        {
            TemplateInstance = templateInstance;
        }
    }
}