using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.Shared
{
    public static partial class CrossAggregateMappingConfigurator
    {
        internal class LoadInstruction
        {
            public RepositoryInfo Repository { get; }
            public AssociationEndModel AssociationEndModel { get; }
            public string PathExpression { get; }

            public string FieldPath { get; }
            public string Variable { get; }

            public bool IsOptional { get; }

            public LoadInstruction(RepositoryInfo repository, AssociationEndModel associationEnd, string pathExpression, string parentAggregatePath, string fieldPathExpression)
            {
                Repository = repository;
                AssociationEndModel = associationEnd;
                PathExpression = pathExpression;
                Variable = pathExpression.ToCSharpIdentifier().ToCamelCase();
                FieldPath = parentAggregatePath.ToCSharpIdentifier().ToCamelCase() + fieldPathExpression;
                IsOptional = associationEnd.IsNullable;
            }
        }

    }
}