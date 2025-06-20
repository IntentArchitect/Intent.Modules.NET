using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.IaC.Terraform.Templates;

public class ApplicationInfo(IApplicationConfig applicationConfig) : IMetadataModel
{
    public string Name => applicationConfig.Name;
    public string Id => applicationConfig.Id;
}