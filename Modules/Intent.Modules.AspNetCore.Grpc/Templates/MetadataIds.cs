using System.Runtime.CompilerServices;
using Intent.AspNetCore.Grpc.Api;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.AspNetCore.Grpc.Templates;
internal static class MetadataIds
{
    public const string ExposeWithGrpcStereotypeId = ServiceModelStereotypeExtensions.ExposeWithGRPC.DefinitionId;
    public const string CommandElementTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
    public const string QueryElementTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";
    //public const string ParameterTypeId = ParameterModel.SpecializationTypeId;
    //public const string DtoFieldTypeId = DTOFieldModel.SpecializationTypeId;
    public const string DtoTypeId = DTOModel.SpecializationTypeId;
    public const string PagedResultTypeId = "9204e067-bdc8-45e7-8970-8a833fdc5253";
    public const string FolderTypeId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";
    public const string FolderOptionsStereotypeId = "66fd9e66-42c7-4ef9-a778-b68e009272b9";
    public const string FolderOptionsStereotypePropertyId = "96df2fa6-7361-4e43-9acf-dbcea23b650a";
    public const string VisualStudioDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
    public const string TemplateOutputTypeId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
    public const string VisualStudioFolderTypeId = "3407a825-1331-4f3f-89a4-901903ed97ce";
    public const string ApiSettingsGroupId = "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb";
    public const string ApiDefaultSecuritySettingsId = "061a559a-0d54-4eb1-8c70-ed0baa238a59";
}