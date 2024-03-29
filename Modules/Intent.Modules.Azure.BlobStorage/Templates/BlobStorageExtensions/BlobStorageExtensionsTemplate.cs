// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Azure.BlobStorage.Templates.BlobStorageExtensions
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Azure.BlobStorage\Templates\BlobStorageExtensions\BlobStorageExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class BlobStorageExtensionsTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.IO;\r\nusing System.Text;\r\nusing System.Threading;\r\nusi" +
                    "ng System.Threading.Tasks;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nna" +
                    "mespace ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Azure.BlobStorage\Templates\BlobStorageExtensions\BlobStorageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// Contains extension methods for the <see cref=\"IBl" +
                    "obStorage\"/> interface.\r\n    /// </summary>\r\n    public static class ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Azure.BlobStorage\Templates\BlobStorageExtensions\BlobStorageExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        /// <summary>\r\n        /// Uploads a string content to a specifi" +
                    "ed cloud storage location.\r\n        /// </summary>\r\n        /// <param name=\"sto" +
                    "rage\">The blob storage instance to which the string will be uploaded.</param>\r\n " +
                    "       /// <param name=\"cloudStorageLocation\">The URI specifying where to upload" +
                    " the string.</param>\r\n        /// <param name=\"stringContent\">The string content" +
                    " to be uploaded.</param>\r\n        /// <param name=\"cancellationToken\">An optiona" +
                    "l token to cancel the asynchronous operation.</param>\r\n        /// <returns>The " +
                    "URI of the uploaded blob.</returns>\r\n        public static Task<Uri> UploadStrin" +
                    "gAsync(this IBlobStorage storage, Uri cloudStorageLocation, string stringContent" +
                    ", CancellationToken cancellationToken = default)\r\n        {\r\n            var str" +
                    "eam = new MemoryStream(Encoding.UTF8.GetBytes(stringContent));\r\n            retu" +
                    "rn storage.UploadAsync(cloudStorageLocation, stream, cancellationToken);\r\n      " +
                    "  }\r\n\r\n        /// <summary>\r\n        /// Uploads a string content to a specific" +
                    " blob in a given container.\r\n        /// </summary>\r\n        /// <param name=\"st" +
                    "orage\">The blob storage instance to which the string will be uploaded.</param>\r\n" +
                    "        /// <param name=\"containerName\">The name of the blob container.</param>\r" +
                    "\n        /// <param name=\"blobName\">The name of the blob.</param>\r\n        /// <" +
                    "param name=\"stringContent\">The string content to be uploaded.</param>\r\n        /" +
                    "// <param name=\"cancellationToken\">An optional token to cancel the asynchronous " +
                    "operation.</param>\r\n        /// <returns>The URI of the uploaded blob.</returns>" +
                    "\r\n        public static Task<Uri> UploadStringAsync(this IBlobStorage storage, s" +
                    "tring containerName, string blobName, string stringContent, CancellationToken ca" +
                    "ncellationToken = default)\r\n        {\r\n            var stream = new MemoryStream" +
                    "(Encoding.UTF8.GetBytes(stringContent));\r\n            return storage.UploadAsync" +
                    "(containerName, blobName, stream, cancellationToken);\r\n        }\r\n\r\n        /// " +
                    "<summary>\r\n        /// Downloads the content of a blob from a specified cloud st" +
                    "orage location as a string.\r\n        /// </summary>\r\n        /// <param name=\"st" +
                    "orage\">The blob storage instance from which the string will be downloaded.</para" +
                    "m>\r\n        /// <param name=\"cloudStorageLocation\">The URI specifying the blob t" +
                    "o be downloaded.</param>\r\n        /// <param name=\"cancellationToken\">An optiona" +
                    "l token to cancel the asynchronous operation.</param>\r\n        /// <returns>The " +
                    "downloaded string content.</returns>\r\n        public static async Task<string> D" +
                    "ownloadAsStringAsync(this IBlobStorage storage, Uri cloudStorageLocation, Cancel" +
                    "lationToken cancellationToken = default)\r\n        {\r\n            var result = aw" +
                    "ait storage.DownloadAsync(cloudStorageLocation, cancellationToken);\r\n           " +
                    " var text = await new StreamReader(result).ReadToEndAsync();\r\n            return" +
                    " text;\r\n        }\r\n\r\n        /// <summary>\r\n        /// Downloads the content of" +
                    " a specific blob in a given container as a string.\r\n        /// </summary>\r\n    " +
                    "    /// <param name=\"storage\">The blob storage instance from which the string wi" +
                    "ll be downloaded.</param>\r\n        /// <param name=\"containerName\">The name of t" +
                    "he blob container.</param>\r\n        /// <param name=\"blobName\">The name of the b" +
                    "lob.</param>\r\n        /// <param name=\"cancellationToken\">An optional token to c" +
                    "ancel the asynchronous operation.</param>\r\n        /// <returns>The downloaded s" +
                    "tring content.</returns>\r\n        public static async Task<string> DownloadAsStr" +
                    "ingAsync(this IBlobStorage storage, string containerName, string blobName, Cance" +
                    "llationToken cancellationToken = default)\r\n        {\r\n            var result = a" +
                    "wait storage.DownloadAsync(containerName, blobName, cancellationToken);\r\n       " +
                    "     var text = await new StreamReader(result).ReadToEndAsync();\r\n            re" +
                    "turn text;\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
