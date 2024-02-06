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
                    if (crudTest.Dependencies.Any())
                    {
                        @class.AddMethod("Task", $"Create{crudTest.Entity.Name}Dependencies", method =>
                        {
                            method.Async();
                            foreach (var dependency in crudTest.Dependencies)
                            {
                                method.AddStatement($"await Create{dependency.EntityName}();");
                            }
                        });

                    }

                    @class.AddMethod($"Task<{template.GetTypeName(crudTest.Entity.Attributes.FirstOrDefault(a => a.HasStereotype("Primary Key"))?.TypeReference)}>", $"Create{crudTest.Entity.Name}", method =>
                    {
                        method.Async();

                        var dtoModel = crudTest.Create.Inputs.First();
                        var sutId = $"{crudTest.Entity.Name.ToParameterName()}Id";
                        if (crudTest.Dependencies.Any())
                        {
                            method.AddStatement($"await Create{crudTest.Entity.Name}Dependencies();", s => s.SeparatedFromNext());
                        }

                        method
                            .AddStatement($"var client = new {template.GetTypeName("Intent.AspNetCore.IntegrationTesting.HttpClient", crudTest.Proxy.Id)}(_factory.CreateClient());", s => s.SeparatedFromNext())
                            .AddStatement($"var command = CreateCommand<{template.GetTypeName(dtoModel.TypeReference)}>();")
                            .AddStatement($"var {sutId} = await client.{crudTest.Create.Name}Async(command){(crudTest.IdField != null ? $".{crudTest.IdField}" : "")};")
                            .AddStatement($"_idTracker[\"{sutId.ToPascalCase()}\"] = {sutId};");
                        if (keyAliases.TryGetValue(sutId.ToPascalCase(), out var aliases))
                        {
                            foreach (var alias in aliases)
                            {
                                method.AddStatement($"_idTracker[\"{alias}\"] = {sutId};");
                            }
                        }
                        method
                            .AddStatement($"return {sutId};");
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
