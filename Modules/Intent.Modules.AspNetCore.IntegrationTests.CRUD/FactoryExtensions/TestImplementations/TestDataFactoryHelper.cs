using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
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
                        .AddStatement("fixture.Customizations.Add(new PopulateIdsSpecimenBuilder(_idTracker));")
                        .AddStatement("return fixture.Create<T>();")
                    ;
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
