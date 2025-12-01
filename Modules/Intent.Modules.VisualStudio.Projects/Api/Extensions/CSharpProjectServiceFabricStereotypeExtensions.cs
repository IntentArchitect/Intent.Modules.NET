using Intent.Modules.Common;

namespace Intent.Modules.VisualStudio.Projects.Api.Extensions
{
    public static class CSharpProjectServiceFabricStereotypeExtensions
    {
        public static bool HasServiceFabric(this CSharpProjectNETModel model) => model.HasStereotype(ServiceFabricStereotype.Id);

        public static ServiceFabricStereotype GetServiceFabric(this CSharpProjectNETModel model)
        {
            var stereotype = model.GetStereotype(ServiceFabricStereotype.Id);
            return new ServiceFabricStereotype(stereotype);
        }
    }
}
