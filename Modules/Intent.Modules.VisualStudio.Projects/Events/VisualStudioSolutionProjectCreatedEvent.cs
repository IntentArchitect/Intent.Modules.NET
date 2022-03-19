namespace Intent.Modules.VisualStudio.Projects.Events
{
    public class VisualStudioSolutionProjectCreatedEvent
    {
        public VisualStudioSolutionProjectCreatedEvent(string projectId, string filePath)
        {
            ProjectId = projectId;
            FilePath = filePath;
        }

        public string ProjectId { get; set; }
        public string FilePath { get; set; }
    }
}