using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageRepositoryBase
{
    internal static class MetadataNames
    {
        public const string EnqueueStatement = nameof(EnqueueStatement);
        public const string DocumentDeclarationStatement = nameof(DocumentDeclarationStatement);
        public const string DocumentsDeclarationStatement = nameof(DocumentsDeclarationStatement);
        public const string PagedDocumentsDeclarationStatement = nameof(PagedDocumentsDeclarationStatement);
        public const string QueryDefinitionDeclarationStatement = nameof(QueryDefinitionDeclarationStatement);
    }
}
