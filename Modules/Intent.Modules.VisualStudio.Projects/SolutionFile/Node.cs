namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal abstract class Node
    {
        public abstract void Visit(Writer writer);
    }
}