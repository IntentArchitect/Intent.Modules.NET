using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataContractGeneralizationModel = Intent.Modelers.Domain.Api.DataContractGeneralizationModel;
using GeneralizationModel = Intent.Modelers.Domain.Api.GeneralizationModel;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

namespace Intent.Modules.Application.Dtos.Mapperly.Templates
{
    internal static class MappingHelper
    {

        internal static ICSharpFileBuilderTemplate GetEntityTemplate(IntentTemplateBase template, DTOModel templateModel)
        {
            if (template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, templateModel.Mapping.ElementId, out ICSharpFileBuilderTemplate entityTemplate) ||
                template.TryGetTemplate(TemplateRoles.Domain.ValueObject, templateModel.Mapping.ElementId, out entityTemplate) ||
                template.TryGetTemplate(TemplateRoles.Domain.DataContract, templateModel.Mapping.ElementId, out entityTemplate))
            {
                return entityTemplate;
            }

            return entityTemplate;
        }

        //If your persistence layer has different persistence models from the domain models
        //you need them registered with AutoMapper for OData to work.
        internal static bool RequiresPersistenceMappings(ISoftwareFactoryExecutionContext application, IntentTemplateBase template, IElement? mappedElement, out string persistenceContractName)
        {
            persistenceContractName = null;

            if (mappedElement == null) { return false; }

            if (!(application.InstalledModules.Any(m => m.ModuleId == "Intent.CosmosDB") && application.InstalledModules.Any(m => m.ModuleId == "Intent.AspNetCore.ODataQuery")))
            {
                return false;
            }

            if (!IsCosmosModel(mappedElement))
            {
                return false;
            }

            if (!application.TemplateExists("Intent.CosmosDB.CosmosDBDocumentInterface", mappedElement.Id))
            {
                return false;
            }

            var docInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.CosmosDB.CosmosDBDocumentInterface", mappedElement.Id);
            persistenceContractName = template.GetTypeName(docInterfaceTemplate);
            return true;
        }

        internal enum MethodName { ForMember, ForPath }


        internal static (string Expression, MethodName MethodName) GetMappingExpression(ICSharpFileBuilderTemplate template, DTOFieldModel field)
        {
            var pathTargets = field.Mapping.Path;

            if (pathTargets.Count != 1
                || !pathTargets.First().Element.IsAssociationEndModel()
                || !template.GetTypeInfo(field.TypeReference).IsPrimitive)
            {
                var (path, methodName) = GetPath(pathTargets);

                return ($"src.{path}", methodName);
            }

            var association = pathTargets.First().Element.AsAssociationEndModel().Association;
            return (association.SourceEnd.Multiplicity, association.TargetEnd.Multiplicity) switch
            {
                (Multiplicity.ZeroToOne, Multiplicity.ZeroToOne) => GetPk(pathTargets),
                (Multiplicity.ZeroToOne, Multiplicity.One) => GetPk(pathTargets),
                (Multiplicity.ZeroToOne, Multiplicity.Many) => GetMultiplePk(template, pathTargets),
                (Multiplicity.One, Multiplicity.ZeroToOne) => GetPk(pathTargets),
                (Multiplicity.One, Multiplicity.One) => GetPk(pathTargets),
                (Multiplicity.One, Multiplicity.Many) => GetMultiplePk(template, pathTargets),
                (Multiplicity.Many, Multiplicity.ZeroToOne) => GetLocalFk(pathTargets),
                (Multiplicity.Many, Multiplicity.One) => GetLocalFk(pathTargets),
                (Multiplicity.Many, Multiplicity.Many) => GetMultiplePk(template, pathTargets),
                _ => throw new InvalidOperationException($"Problem resolving association {association.SourceEnd.Multiplicity} -> {association.TargetEnd.Multiplicity}")
            };
        }

        private static void AddFieldMapping(CSharpMethodChainStatement statement, DTOFieldModel field, string mapping)
        {
            statement.AddChainStatement(mapping, chain =>
            {
                chain.AddMetadata("field", field);
            });
        }

        private static bool IsCosmosModel(IElement? mappedElement)
        {
            if (!mappedElement.Package.HasStereotype("Document Database"))
                return false;
            var setting = mappedElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
            if (setting != null && setting.Id != "3e1a00f7-c6f1-4785-a544-bbcb17602b31")//CosmosDB Provider)
            {
                return false;
            }
            return true;
        }

        private static (string Expression, MethodName MethodName) GetPk(IList<IElementMappingPathTarget> pathTargets)
        {
            var (path, methodName) = GetPath(pathTargets);

            return ($"src.{path}.Id", methodName);
        }

        private static (string Expression, MethodName MethodName) GetMultiplePk(ICSharpFileBuilderTemplate template, IList<IElementMappingPathTarget> pathTargets)
        {
            template.AddUsing("System.Linq");

            var (path, methodName) = GetPath(pathTargets);

            return ($"src.{path}.Select(x => x.Id).ToArray()", methodName);
        }

        private static (string Expression, MethodName MethodName) GetLocalFk(IList<IElementMappingPathTarget> pathTargets)
        {
            var association = pathTargets.First().Element.AsAssociationEndModel();

            return ($"src.{association.Name.ToPascalCase()}Id", MethodName.ForMember);
        }

        private static (string Path, MethodName MethodName) GetPath(IEnumerable<IElementMappingPathTarget> pathTargets)
        {
            var path = string.Join(".", pathTargets
                .Where(pathTarget => pathTarget.Specialization != "Generalization Target End" &&
                                     pathTarget.Specialization != GeneralizationModel.SpecializationType &&
                                     pathTarget.Specialization != "Data Contract Generalization Target End" &&
                                     pathTarget.Specialization != DataContractGeneralizationModel.SpecializationType)
                .Select(pathTarget =>
                {
                    var nullForgivingOperator = pathTarget.Element?.TypeReference.IsNullable == true ? "!" : string.Empty;
                    var operationCall = pathTarget.Specialization == OperationModel.SpecializationType ? "()" : string.Empty;

                    return $"{pathTarget.Name}{operationCall}{nullForgivingOperator}";
                })).TrimEnd('!');

            return (Path: path, MethodName: MethodName.ForMember);
        }

        public static string BuildNullChecks(string expression)
        {
            var rawParts = expression.Split('.');
            var checks = new List<string>();

            // a bit rudamentary, but basically if the expression already contains a ternary operation
            // then don't add a null check as we will assume this is performing that. This is for custom mappings
            if(expression.Contains(" ? ") && expression.Contains(" : "))
            {
                return string.Empty;
            }

            var currentPath = rawParts[0]; // e.g. "src"

            for (int i = 1; i < rawParts.Length - 1; i++)
            {
                // Keep clean version of the part for building the path
                var part = rawParts[i].Replace("!", "");
                currentPath += "." + part;

                // Only add a check if the original part DID have !
                if (rawParts[i].Contains("!"))
                {
                    checks.Add($"{currentPath} != null");
                }
            }

            var joinedCheck = string.Join(" && ", checks);
            return !string.IsNullOrWhiteSpace(joinedCheck) ? $"{joinedCheck} ?" : string.Empty;
        }
    }
}
