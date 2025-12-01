namespace Intent.Modules.VisualStudio.Projects.Events.ServiceFabric;

public abstract class ServiceRegistrationRequiredBase
{
    protected ServiceRegistrationRequiredBase(string name, string serviceTypeName)
    {
        Name = name;
        ServiceTypeName = serviceTypeName;
    }

    public string Name { get; }
    public string ServiceTypeName { get; }
}