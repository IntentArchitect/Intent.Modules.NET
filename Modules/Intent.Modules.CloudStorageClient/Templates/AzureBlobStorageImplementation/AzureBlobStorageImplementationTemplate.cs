﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.CloudStorageClient.Templates.AzureBlobStorageImplementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.CloudStorageClient\Templates\AzureBlobStorageImplementation\AzureBlobStorageImplementationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class AzureBlobStorageImplementationTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.IO;\r\nusing System.Threading;\r\nusing System.Threading.Tasks;\r\nusing Azure.Storage.Blobs;\r\nusing Microsoft.Extensions.Configuration;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.CloudStorageClient\Templates\AzureBlobStorageImplementation\AzureBlobStorageImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.CloudStorageClient\Templates\AzureBlobStorageImplementation\AzureBlobStorageImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n    private readonly BlobServiceClient _client;\r\n\r\n        public ");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.CloudStorageClient\Templates\AzureBlobStorageImplementation\AzureBlobStorageImplementationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IConfiguration configuration)\r\n        {\r\n            _client = new BlobServiceClient(configuration.GetValue<string>(\"AzureBlobStorage\"));\r\n        }\r\n\r\n        public Task<Stream> DownloadContentAsync(Uri cloudStorageLocation, CancellationToken cancellationToken = default)\r\n        {\r\n            var blobUriBuilder = new BlobUriBuilder(cloudStorageLocation);\r\n            return DownloadContentAsync(blobUriBuilder.BlobContainerName, blobUriBuilder.BlobName, cancellationToken);\r\n        }\r\n        \r\n        public async Task<Stream> DownloadContentAsync(string containerName, string blobName, CancellationToken cancellationToken = default)\r\n        {\r\n            var containerClient = _client.GetBlobContainerClient(containerName);\r\n            var blobClient = containerClient.GetBlobClient(blobName);\r\n            var result = await blobClient.DownloadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);\r\n            return result.Value.Content;\r\n        }\r\n\r\n        public Task UploadContent(Uri cloudStorageLocation, Stream streamToUpload, bool overwrite = true, CancellationToken cancellationToken = default)\r\n        {\r\n            var blobUriBuilder = new BlobUriBuilder(cloudStorageLocation);\r\n            return UploadContent(blobUriBuilder.BlobContainerName, blobUriBuilder.BlobName, overwrite, cancellationToken);\r\n        }\r\n\r\n        public async Task UploadContent(string containerName, string blobName, Stream streamToUpload, bool overwrite = true, CancellationToken cancellationToken = default)\r\n        {\r\n            var containerClient = _client.GetBlobContainerClient(containerName);\r\n            var blobClient = containerClient.GetBlobClient(blobName);\r\n            await blobClient.UploadAsync(streamToUpload, overwrite, cancellationToken).ConfigureAwait(false);\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
