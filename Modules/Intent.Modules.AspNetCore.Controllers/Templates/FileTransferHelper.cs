using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.Controllers.Templates
{
    public class FileTransferHelper
    {
        
        public const string UploadFileTypeId = "bb8b4603-1e92-43cb-97ed-31ca0a8c374c";
        //public const string UploadFileTypeId = "9758e896-f0d0-4339-ae71-ed0d1939831b";
        public const string DownloadFileTypeId = "922c1803-671b-4412-8bc1-e743754e35d5";
        //public const string DownloadFileTypeId = "66c99837-a0ea-4422-842c-49ecffe29db4";


        public static bool IsFileDownload(IControllerOperationModel operation)
        {
            return FileTransferHelper.IsDownloadFileType(operation.ReturnType);
        }

        public static bool IsFileUpoad(IControllerOperationModel operation)
        {
            if (operation.Parameters.Any(p => IsUploadFileType( p.TypeReference)))
            {
                return true;
            }
            var dto = operation.Parameters.FirstOrDefault(d => d.Source == HttpInputSource.FromBody);
            if (dto == null)
            {
                return false;
            }
            if (dto.TypeReference != null && dto.TypeReference.Element != null)
            {
                return ((IElement)dto.TypeReference.Element).ChildElements.Any(c => FileTransferHelper.IsUploadFileType(c.TypeReference));
            }
            return false;
        }

        public static bool IsUploadFileType(ITypeReference? typeReference)
        {
            return typeReference != null &&
                typeReference.Element != null &&
                typeReference.Element.Id == UploadFileTypeId;

        }

        public static bool IsDownloadFileType(ITypeReference? typeReference)
        {
            return typeReference != null &&
                typeReference.Element != null &&
                typeReference.Element.Id == DownloadFileTypeId;

        }

        public static string GetFileTransferFieldName(IControllerOperationModel operation)
        {
            var dto = operation.Parameters.First(d => d.Source == HttpInputSource.FromBody);
            return ((IElement)dto.TypeReference?.Element).ChildElements.First(c => FileTransferHelper.IsUploadFileType(c.TypeReference)).Name;
        }


        public static bool NeedsFileUploadInfrastructure(IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.Services(applicationId).Elements.Any(x => IsDownloadFileType( x.TypeReference) || x.ChildElements.Any(c => IsUploadFileType( c.TypeReference)));
        }
    }
}
