using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Controllers.Templates.UploadFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;


namespace Intent.Modules.AspNetCore.Controllers.Templates.DownloadFile
{
    internal class DownloadFileTypeSource : ITypeSource
    {
        private readonly DownloadFileTemplate _template;

        public DownloadFileTypeSource(DownloadFileTemplate template)
        {
            _template = template;
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            if (typeInfo?.Element?.Id == FileTransferHelper.DownloadFileTypeId)
            {
                return CSharpResolvedTypeInfo.Create(
                    resolvedTypeInfo: ResolvedTypeInfo.Create(
                        name: _template.ClassName,
                        isPrimitive: false,
                        typeReference: typeInfo,
                        template: _template),
                    genericTypeParameters: null);
            }

            return null;
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            yield break;
        }

        public ICollectionFormatter CollectionFormatter { get; }
        public INullableFormatter NullableFormatter { get; }
    }
}
