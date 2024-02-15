using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.AspNetCore.Controllers.Templates.UploadFile
{
    internal class UploadFileTypeSource : ITypeSource
    {
        private readonly UploadFileTemplate _template;

        public UploadFileTypeSource(UploadFileTemplate template)
        {
            _template = template;
        }

        public IResolvedTypeInfo GetType(ITypeReference typeInfo)
        {
            if (typeInfo?.Element?.Id == FileTransferHelper.UploadFileTypeId)
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
