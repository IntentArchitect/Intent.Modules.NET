using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AspNetCore.IntegrationTesting;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD.FactoryExtensions.TestImplementations
{
    internal class TestDataFactoryHelper
    {
        public static void PopulateTestDataFactory(ICSharpFileBuilderTemplate template, List<CrudMap> crudTests)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var keyAliases = GetKeyAliases(crudTests);

                var @class = file.Classes.First();
                @class.AddField("Dictionary<string, object>", "_idTracker", f => f.PrivateReadOnly().WithAssignment("new()"));

                @class.AddMethod("T", "CreateCommand", method =>
                {
                    method
                        .AddGenericParameter("T")
                        .AddStatement("var fixture = new Fixture();")
                        .AddStatement("fixture.RepeatCount = 1;")
                        .AddStatement("fixture.Customizations.Add(new PopulateIdsSpecimenBuilder(_idTracker));");

                    var autoFixtureVersions = NugetPackages.AutoFixture(template.OutputTarget).Version.Split(".");

                    // The fix is only required for versions of AutoFixture < 5
                    // Strictly speaking, this is required for all versions of AutoFixture before 5.0.0-preview0011, but the assumption
                    // is a preview version will not be added into Intent as the default
                    if (autoFixtureVersions.Length > 0 && int.TryParse(autoFixtureVersions[0], out int majorVersion) && majorVersion < 5 )
                    {
                        if (crudTests is not null && crudTests.Any(t => t.Entity is not null &&
                            t.Entity.Attributes is not null && t.Entity.Attributes.Any(a => a.Type.HasDateType())))
                        {
                            method.AddStatement($"fixture.Customize<DateOnly>(o => o.FromFactory(({template.UseType("System.DateTime")} dt) => DateOnly.FromDateTime(dt)));");
                        }
                    }

                    method.AddStatement("return fixture.Create<T>();");
                });

                foreach (var crudTest in crudTests)
                {
                    if (crudTest.Dependencies.Any() || crudTest.OwningAggregate != null)
                    {
                        @class.AddMethod("Task", $"Create{crudTest.Entity.Name}Dependencies", method =>
                        {
                            method.Async();
                            var owningAggragateId = "";
                            if (crudTest.OwningAggregate != null)
                            {
                                method.WithReturnType($"Task<{template.GetTypeName(crudTest.OwningAggregate.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference)}>");
                                owningAggragateId = $"{crudTest.OwningAggregate.Name.ToParameterName()}Id";
                                method.AddStatement($"var {owningAggragateId} = await Create{crudTest.OwningAggregate.Name}();");
                            }
                            foreach (var dependency in crudTest.Dependencies.Where(d => d.EntityName != crudTest.OwningAggregate?.Name))
                            {
                                method.AddStatement($"await Create{dependency.EntityName}();");
                            }
                            if (crudTest.OwningAggregate != null)
                            {
                                method.AddStatement($"return {owningAggragateId};");
                            }
                        });

                    }

                    @class.AddMethod($"Task<{template.GetTypeName(crudTest.Entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference)}>", $"Create{crudTest.Entity.Name}", method =>
                    {
                        method.Async();

                        var owningAggragateId = crudTest.OwningAggregate != null ? $"{crudTest.OwningAggregate?.Name.ToParameterName()}Id" : null;
                        var dtoModel = crudTest.Create.Inputs.First();
                        var sutId = $"{crudTest.Entity.Name.ToParameterName()}Id";

                        if (crudTest.Dependencies.Any() || crudTest.OwningAggregate != null)
                        {
                            method.AddStatement($"{(crudTest.OwningAggregate != null ? $"var {owningAggragateId} = " : "")}await Create{crudTest.Entity.Name}Dependencies();", s => s.SeparatedFromNext());
                        }

                        method
                            .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(_factory.CreateClient());", s => s.SeparatedFromNext())
                            .AddStatement($"var command = CreateCommand<{template.GetTypeName(dtoModel.TypeReference)}>();");

                        if (crudTest.ResponseDtoIdField is null)
                        {
                            method.AddStatement($"var {sutId} = await client.{crudTest.Create.Name}Async(command);");
                        }
                        else
                        {
                            method.AddStatement($"var dto = await client.{crudTest.Create.Name}Async(command);");
                            method.AddStatement($"var {sutId} = dto.{crudTest.ResponseDtoIdField};");
                        }
                        method.AddStatement($"_idTracker[\"{sutId.ToPascalCase()}\"] = {sutId};");
                        if (keyAliases.TryGetValue(sutId.ToPascalCase(), out var aliases))
                        {
                            foreach (var alias in aliases)
                            {
                                method.AddStatement($"_idTracker[\"{alias}\"] = {sutId};");
                            }
                        }
                        if (crudTest.OwningAggregate != null)
                        {
                            method.WithReturnType($"Task<({template.GetTypeName(crudTest.OwningAggregate.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference)} {owningAggragateId.ToPascalCase()}, {template.GetTypeName(crudTest.Entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference)} {sutId.ToPascalCase()})>");
                            method.AddStatement($"return ({owningAggragateId}, {sutId});");
                        }
                        else
                        {
                            method.AddStatement($"return {sutId};");
                        }
                    });
                }
            });

        }

        internal static string? GetDtoPkFieldName(IHttpEndpointModel operation)
        {
            var mapping = GetMapEntityQueryMapping(operation.InternalElement);
            if (mapping != null) // We only doing Test for services on new mapping system
            {
                foreach (var mappedEnd in mapping.MappedEnds.Where(me => me.MappingType == "Filter Mapping"))
                {
                    var attributeModel = (mappedEnd.TargetElement as IElement)?.AsAttributeModel();
                    if (attributeModel is not null && attributeModel.HasStereotype("Primary Key"))
                    {
                        return mappedEnd.SourceElement.Name;
                    }

                }
            }
            return null;
        }
        private static IElementToElementMapping? GetMapEntityQueryMapping(IElement mappedElement)
        {
            return mappedElement.AssociatedElements.FirstOrDefault(a => a.Association.SpecializationTypeId == "9ea0382a-4617-412a-a8c8-af987bbce226")//Update Entity Action
                   ?.Mappings.FirstOrDefault(mappingModel => mappingModel.TypeId == "25f25af9-c38b-4053-9474-b0fabe9d7ea7");// Query Entity Mapping
        }


        private static Dictionary<string, HashSet<string>> GetKeyAliases(List<CrudMap> crudTests)
        {
            var keyAliases = new Dictionary<string, HashSet<string>>();
            foreach (var crudTest in crudTests)
            {
                foreach (var dependency in crudTest.Dependencies)
                {
                    foreach (var mapping in dependency.Mappings)
                    {
                        //Alias for related Entity Id e.g. CustomerId could be MyCustomerId or some other rename in the DTO
                        string alias = mapping.SourceElement.Name;

                        var relatedClass = GetRelatedClass(mapping);

                        if (relatedClass != null)
                        {
                            string entityId = $"{relatedClass.Name}Id";
                            if (alias == entityId)
                                continue;
                            if (!keyAliases.TryGetValue(entityId, out var aliases))
                            {
                                aliases = new HashSet<string>();
                                keyAliases[entityId] = aliases;
                            }
                            aliases.Add(alias);
                        }
                    }
                }
                if (crudTest.Update != null)
                {
                    var dtoFieldPkName = GetDtoPkFieldName(crudTest.Update);
                    if (dtoFieldPkName != null)
                    {
                        string entityId = $"{crudTest.Entity.Name}Id";
                        if (dtoFieldPkName != entityId && string.Compare(dtoFieldPkName, "Id", true) != 0)
                        {
                            if (!keyAliases.TryGetValue(entityId, out var aliases))
                            {
                                aliases = new HashSet<string>();
                                keyAliases[entityId] = aliases;
                            }
                            aliases.Add(dtoFieldPkName);
                        }
                    }
                }
            }

            return keyAliases;
        }

        private static ClassModel? GetRelatedClass(IElementToElementMappedEnd mapping)
        {
            var attributeModel = (mapping.TargetElement as IElement)?.AsAttributeModel();
            if (attributeModel is not null && attributeModel.HasStereotype("Foreign Key"))
            {
                var ae = attributeModel.GetStereotype("Foreign Key")?.GetProperty<IElement>("Association")?.AsAssociationTargetEndModel();
                if (ae != null)
                {
                    var cm = ae.Class;
                    if (ae.Class.Id == attributeModel.Class.Id)
                    {
                        cm = ae.OtherEnd().Class;
                    }
                    return cm;
                }
            }
            return null;
        }
    }
}
