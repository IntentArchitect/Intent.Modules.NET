using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Integration.HttpClients.Shared
{
    public class FileTransferHelper
    {

        public const string FileTransferStereotypeId = "d30e48e8-389e-4b70-84fd-e3bac44cfe19";
        public const string StreamTypeId = "fd4ead8e-92e9-47c2-97a6-81d898525ea0";

        public static bool IsStreamType(ITypeReference? typeReference)
        {
            return typeReference != null &&
                typeReference.Element != null &&
                typeReference.Element.Id == StreamTypeId;
        }
        public static bool IsFileDownloadOperation(IHttpEndpointModel operation)
        {
            return operation.InternalElement.HasStereotype(FileTransferStereotypeId) &&
                HasStreamField(operation.ReturnType);
        }

        public static bool IsFileUploadOperation(IHttpEndpointModel operation)
        {
            if (!operation.InternalElement.HasStereotype(FileTransferStereotypeId))
            {
                return false;
            }

            if (operation.Inputs.Any(p => IsStreamType(p.TypeReference)))
            {
                return true;
            }
            var dto = operation.Inputs.FirstOrDefault(d => d.Source == HttpInputSource.FromBody);
            if (dto == null)
            {
                return false;
            }
            if (dto.TypeReference != null && dto.TypeReference.Element != null)
            {
                return HasStreamField(dto.TypeReference);
            }
            return false;
        }

        public static FileInfo GetDownloadTypeInfo(IHttpEndpointModel operation)
        {
            var returnDto = ((IElement)operation.ReturnType.Element);
            return GetDtoFieldInfo(returnDto);
        }

        public static FileInfo GetUploadTypeInfo(IHttpEndpointModel operation)
        {
            if (operation.Inputs.Any(p => IsStreamType(p.TypeReference)))
            {
                string streamFieldName = "Stream";
                string? fileNameField = default;
                string? contentTypeField = default;
                string? contentLengthField = default;
                foreach (var child in operation.Inputs)
                {
                    if (IsStreamType(child.TypeReference))
                    {
                        streamFieldName = child.Name;
                    }
                    else if (string.Equals("filename", child.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        fileNameField = child.Name;
                    }
                    else if (string.Equals("contenttype", child.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        contentTypeField = child.Name;
                    }
                    else if (string.Equals("contentlength", child.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        contentLengthField = child.Name;
                    }
                }
                return new FileInfo(streamFieldName, fileNameField, contentTypeField, contentLengthField);
            }

            var dto = operation.Inputs.FirstOrDefault(d => d.Source == HttpInputSource.FromBody);
            if (dto != null && dto.TypeReference != null && dto.TypeReference.Element != null)
            {
                return GetDtoFieldInfo((IElement)dto.TypeReference.Element, dto.Name);
            }
            return null;
        }

        public static string GetStreamFieldName(ITypeReference type)
        {
            if (type == null) return null;
            return ((IElement)type.Element).ChildElements.FirstOrDefault(c => FileTransferHelper.IsStreamType(c.TypeReference)).Name;
        }

        public static string GetStreamFieldName(IHttpEndpointModel operation)
        {
            var dto = operation.Inputs.First(d => d.Source == HttpInputSource.FromBody);
            return GetStreamFieldName(dto.TypeReference);
        }

        private static bool HasStreamField(ITypeReference returnType)
        {
            if (returnType == null) return false;
            return ((IElement)returnType.Element).ChildElements.Any(c => FileTransferHelper.IsStreamType(c.TypeReference));
        }
        public record FileInfo(string StreamField, string? FileNameField, string? ContentTypeField, string? ContentLengthField, string? DtoPropertyName = null)
        {
            public bool HasFilename() => !string.IsNullOrEmpty(FileNameField);
            public bool HasContentType() => !string.IsNullOrEmpty(ContentTypeField);
            public bool HasContentLength() => !string.IsNullOrEmpty(ContentLengthField);
        };

        private static FileInfo GetDtoFieldInfo(IElement dto, string? dtoPropertyName = null)
        {
            string streamFieldName = "Stream";
            string? fileNameField = default;
            string? contentTypeField = default;
            string? contentLengthField = default;
            foreach (var child in dto.ChildElements)
            {
                if (IsStreamType(child.TypeReference))
                {
                    streamFieldName = child.Name;
                }
                else if (string.Equals("filename", child.Name, StringComparison.OrdinalIgnoreCase))
                {
                    fileNameField = child.Name;
                }
                else if (string.Equals("contenttype", child.Name, StringComparison.OrdinalIgnoreCase))
                {
                    contentTypeField = child.Name;
                }
                else if (string.Equals("contentlength", child.Name, StringComparison.OrdinalIgnoreCase))
                {
                    contentLengthField = child.Name;
                }
            }
            return new FileInfo(streamFieldName, fileNameField, contentTypeField, contentLengthField, dtoPropertyName);
        }


    }
}
