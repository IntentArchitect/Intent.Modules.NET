using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Templates;
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

        public const string FileTransferStereotypeId = "d30e48e8-389e-4b70-84fd-e3bac44cfe19";
        public const string StreamTypeId = "fd4ead8e-92e9-47c2-97a6-81d898525ea0";

        public static bool IsFileDownloadOperation(IControllerOperationModel operation)
        {
            return operation.InternalElement.HasStereotype(FileTransferStereotypeId) &&
                HasStreamField(operation.ReturnType);
        }

        public static FileInfo GetDownloadTypeInfo(IControllerOperationModel operation)
        {
            var returnDto = ((IElement)operation.ReturnType.Element);
            return GetDtoFieldInfo(returnDto);
        }

        public static FileInfo GetUploadTypeInfo(IControllerOperationModel operation)
        {
            if (operation.Parameters.Any(p => IsStreamType(p.TypeReference)))
            {
                string streamFieldName = "Stream";
                string? fileNameField = default;
                string? contentTypeField = default;
                string? contentLengthField = default;
                foreach (var child in operation.Parameters)
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

            var dto = operation.Parameters.FirstOrDefault(d => d.Source == HttpInputSource.FromBody);
            if (dto != null && dto.TypeReference != null && dto.TypeReference.Element != null)
            {
                return GetDtoFieldInfo((IElement)dto.TypeReference.Element);
            }
            return null;
        }

        public static string GetStreamFieldName(ITypeReference type)
        {
            if (type == null) return null;
            return ((IElement)type.Element).ChildElements.FirstOrDefault(c => FileTransferHelper.IsStreamType(c.TypeReference)).Name;
        }

        public static bool IsFileUploadOperation(IControllerOperationModel operation)
        {
            if (!operation.InternalElement.HasStereotype(FileTransferStereotypeId))
            {
                return false;
            }

            if (operation.Parameters.Any(p => IsStreamType(p.TypeReference)))
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
                return HasStreamField(dto.TypeReference);
            }
            return false;
        }

        public static void AddControllerStreamLogic(ICSharpFileBuilderTemplate template, CSharpClassMethod method, IControllerOperationModel operation)
        {
            var fileInfo = FileTransferHelper.GetUploadTypeInfo(operation);

            template.AddUsing("System.IO");
            template.AddUsing("System.Linq");
            var stereoType = operation.InternalElement.GetStereotype(FileTransferHelper.FileTransferStereotypeId);
            if (stereoType != null)
            {
                var maxFileSizeValue = stereoType.GetProperty("c0ea27fb-2e0b-4542-b294-da9d2b12565e")?.Value;//Maximum File Size (in bytes)
                if (!string.IsNullOrEmpty(maxFileSizeValue))
                {
                    method.AddIfStatement($"Request.ContentLength != null && Request.ContentLength > {maxFileSizeValue}", stmt =>
                    {
                        stmt.AddStatement("return BadRequest(new { error = \"File to large.\" });");
                    });
                }

                var mimeTypeFilterValue = stereoType.GetProperty("920234c1-6edb-4ceb-b6af-76f459bfb348")?.Value;//Mime Type Filter

                if (!string.IsNullOrEmpty(mimeTypeFilterValue) && mimeTypeFilterValue.Trim() != "*/*")
                {
                    string mimetypes = string.Join(",", mimeTypeFilterValue.Replace(Environment.NewLine, "\n").Split("\n").Select(s => s.Trim().ToLower()));
                    method.AddStatement($"var mimeTypeFilter = new HashSet<string>(@\"{mimetypes}\".Split(','));");
                    method.AddIfStatement($"Request.ContentType == null || !mimeTypeFilter.Contains(Request.ContentType.ToLower())", stmt =>
                    {
                        stmt.AddStatement("return BadRequest(new { error = $\"Invalid file type. {Request.ContentType} not allowed.\" });");
                    });
                }
            }

            method.AddStatement("Stream stream;");
            if (fileInfo.HasFilename())
            {
                template.AddUsing("System.Net.Http.Headers");
                method.AddStatements($@"string? {fileInfo.FileNameField.ToParameterName()} = null;
            if (Request.Headers.TryGetValue(""Content-Disposition"", out var headerValues))
            {{
                string? header = headerValues;
                if (header != null)
                {{
                    var contentDisposition = ContentDispositionHeaderValue.Parse(header);
                    {fileInfo.FileNameField.ToParameterName()} = contentDisposition?.FileName;
                }}
            }}".ConvertToStatements());

            }
            method.AddIfStatement(@"Request.ContentType != null && (Request.ContentType == ""application/x-www-form-urlencoded"" || Request.ContentType.StartsWith(""multipart/form-data"")) && Request.Form.Files.Any()", stmt =>
            {
                stmt.AddStatements(@"var file = Request.Form.Files[0];
            if (file == null || file.Length == 0)
                throw new ArgumentException(""File is empty"");
            stream = file.OpenReadStream();".ConvertToStatements());
                if (fileInfo.HasFilename())
                {
                    stmt.AddStatement($"{fileInfo.FileNameField.ToParameterName()} ??= file.Name;");
                }
            });
            method.AddElseStatement(stmt =>
            {
                stmt.AddStatement("stream = Request.Body;");
            });
        }

        public static bool IsStreamType(ITypeReference? typeReference)
        {
            return typeReference != null &&
                typeReference.Element != null &&
                typeReference.Element.Id == StreamTypeId;
        }

        public static string GetStreamFieldName(IControllerOperationModel operation)
        {
            var dto = operation.Parameters.First(d => d.Source == HttpInputSource.FromBody);
            return GetStreamFieldName(dto.TypeReference);
        }

        public static bool NeedsFileUploadInfrastructure(IMetadataManager metadataManager, string applicationId)
        {
            return metadataManager.Services(applicationId).Elements.Any(x => x.HasStereotype(FileTransferStereotypeId));
        }

        public record FileInfo(string StreamField, string? FileNameField, string? ContentTypeField, string? ContentLemgthField)
        {
            public bool HasFilename() => !string.IsNullOrEmpty(FileNameField);
            public bool HasContentType() => !string.IsNullOrEmpty(ContentTypeField);
            public bool HasContentLength() => !string.IsNullOrEmpty(ContentLemgthField);
        };
        private static bool HasStreamField(ITypeReference returnType)
        {
            if (returnType == null) return false;
            return ((IElement)returnType.Element).ChildElements.Any(c => FileTransferHelper.IsStreamType(c.TypeReference));
        }

        private static FileInfo GetDtoFieldInfo(IElement returnDto)
        {
            string streamFieldName = "Stream";
            string? fileNameField = default;
            string? contentTypeField = default;
            string? contentLengthField = default;
            foreach (var child in returnDto.ChildElements)
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

    }
}
