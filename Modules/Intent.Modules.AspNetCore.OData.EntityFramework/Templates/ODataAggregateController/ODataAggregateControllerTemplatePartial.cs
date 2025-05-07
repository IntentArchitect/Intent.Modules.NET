using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.OData.EntityFramework.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OData.EntityFramework.Templates.ODataAggregateController
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ODataAggregateControllerTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.OData.EntityFramework.ODataAggregateController";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ODataAggregateControllerTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddNugetDependency(NugetPackages.MicrosoftAspNetCoreOData(OutputTarget));
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Mvc")
                .AddUsing("Microsoft.AspNetCore.OData.Deltas")
                .AddUsing("Microsoft.AspNetCore.OData.Query")
                .AddUsing("Microsoft.AspNetCore.OData.Routing.Controllers")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddClass($"{Model.Name.RemoveSuffix("Controller", "Service").Pluralize()}Controller", @class =>
                {
                    @class.WithBaseType("ODataController");
                    @class.AddMetadata("model", Model);
                    @class.AddMetadata("modelId", Model.Id);
                    @class.AddAttribute("[EnableQuery]");

                    var dbContextInstance = DbContextManager.GetDbContext(Model);

                    @class.AddConstructor(constructor =>
                    {
                        // TODO: How to get context type
                        constructor.AddParameter(dbContextInstance.GetTypeName(this), "context", p => p.IntroduceReadonlyField());
                    });

                    if (model.TryGetExposeAsOData(out var stereotype))
                    {
                        var primaryKeys = model.Attributes.Where(att => att.HasStereotype("Primary Key"));

                        if (stereotype.GetAll())
                        {
                            @class.AddMethod($"IQueryable<{GetTypeName(Model.InternalElement)}>", "Get", method =>
                            {
                                method.AddAttribute(new CSharpAttribute("[HttpGet]"));
                                
                                method.AddStatement($"return _context.{Model.Name.Pluralize().ToPascalCase()};");
                            });
                        }

                        if (stereotype.GetById())
                        {
                            @class.AddMethod("IActionResult", "Get", method =>
                            {
                                method.AddAttribute(new CSharpAttribute("[HttpGet]"));
                                
                                var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));
                                method.AddStatement($"var {Model.Name.ToCamelCase()} = _context.{Model.Name.Pluralize().ToPascalCase()}.FirstOrDefault({linqQuery});");
                                method.AddStatement($"return {Model.Name.ToCamelCase()} == null ? NotFound() : Ok({Model.Name.ToCamelCase()});", statement => statement.SeparatedFromPrevious());
                            });
                        }

                        if (stereotype.Post())
                        {
                            @class.AddMethod($"IActionResult", "Post", method =>
                            {
                                method.AddAttribute(new CSharpAttribute("[HttpPost]"));

                                method.AddParameter($"{GetTypeName(Model.InternalElement)}", $"{Model.Name.ToCamelCase()}", parameter =>
                                {
                                    parameter.AddAttribute("[FromBody]");
                                });
                                method.AddStatement($"_context.{Model.Name.Pluralize().ToPascalCase()}.Add({Model.Name.ToCamelCase()});");
                                method.AddStatement($"_context.SaveChanges();");
                                method.AddStatement($"return Created({Model.Name.ToCamelCase()});", statement => statement.SeparatedFromPrevious());
                            });
                        }

                        if (stereotype.Delete())
                        {
                            @class.AddMethod("ActionResult", "Delete", method =>
                            {
                                method.AddAttribute(new CSharpAttribute("[HttpDelete]"));

                                var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));

                                method.AddStatement($"var {Model.Name.ToCamelCase()} = _context.{Model.Name.Pluralize().ToPascalCase()}.SingleOrDefault({linqQuery});");
                                method.AddIfStatement($"{Model.Name.ToCamelCase()} is not null", ifStatement =>
                                {
                                    ifStatement.AddStatement($"_context.{Model.Name.Pluralize().ToPascalCase()}.Remove({Model.Name.ToCamelCase()});");
                                });
                                method.AddStatement("_context.SaveChanges();");
                                method.AddStatement("return NoContent();", statement => statement.SeparatedFromPrevious());
                            });
                        }

                        if (stereotype.Patch())
                        {
                            @class.AddMethod("ActionResult", "Patch", method =>
                            {
                                method.AddAttribute(new CSharpAttribute("[HttpPatch]"));
                                var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));
                                method.AddParameter($"Delta<{GetTypeName(Model.InternalElement)}>", "delta", parameter =>
                                {
                                    parameter.AddAttribute("[FromBody]");
                                });

                                method.AddStatement($"var {Model.Name.ToCamelCase()} = _context.{Model.Name.Pluralize().ToPascalCase()}.SingleOrDefault({linqQuery});");
                                method.AddIfStatement($"{Model.Name.ToCamelCase()} is null", ifStatement =>
                                {
                                    ifStatement.AddStatement($"return NotFound();");
                                });

                                method.AddStatement($"delta.Patch({Model.Name.ToCamelCase()});");

                                method.AddStatement("_context.SaveChanges();");
                                method.AddStatement($"return Updated({Model.Name.ToCamelCase()});", statement => statement.SeparatedFromPrevious());
                            });
                        }

                        if (stereotype.Put())
                        {
                            @class.AddMethod("ActionResult", "Put", method =>
                            {
                                method.AddAttribute(new CSharpAttribute("[HttpPut]"));
                                var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));
                                method.AddParameter($"{GetTypeName(Model.InternalElement)}", "update", parameter =>
                                {
                                    parameter.AddAttribute("[FromBody]");
                                });

                                method.AddStatement($"var {Model.Name.ToCamelCase()} = _context.{Model.Name.Pluralize().ToPascalCase()}.AsNoTracking().SingleOrDefault({linqQuery});");
                                method.AddIfStatement($"{Model.Name.ToCamelCase()} is null", ifStatement =>
                                {
                                    ifStatement.AddStatement($"return NotFound();");
                                });

                                var primaryKeys = model.Attributes.Where(att => att.HasStereotype("Primary Key"));
                                if (primaryKeys.Count() == 1)
                                {
                                    method.AddStatement($"update.{primaryKeys.First().Name} = key;");
                                }
                                else
                                {
                                    foreach (var primaryKey in primaryKeys)
                                    {
                                        method.AddStatement($"update.{primaryKey.Name} = key{primaryKey.Name.ToPascalCase()};");
                                    }
                                }

                                // TODO: Make sure about the keys being correct.
                                method.AddStatement($"_context.Entry(update).State = EntityState.Modified;");

                                method.AddStatement("_context.SaveChanges();");
                                method.AddStatement($"return Updated({Model.Name.ToCamelCase()});", statement => statement.SeparatedFromPrevious());
                            });
                        }
                    }
                });
        }

        private string GeneratePrimaryKeyParameters(ClassModel model, CSharpClassMethod method, Action<CSharpParameter> configure = null)
        {
            var primaryKeys = model.Attributes.Where(att => att.HasStereotype("Primary Key"));
            var lambdaParameter = "m";
            var query = $"{lambdaParameter} => ";

            foreach (var primaryKey in primaryKeys)
            {
                if (primaryKeys.Count() == 1)
                {
                    configure = null;
                    var primaryKeyType = GetTypeName(primaryKey);
                    method.AddParameter(primaryKeyType, "key", configure);
                    query += $"{lambdaParameter}.{primaryKey.Name.ToPascalCase()} == key &&";
                }
                else
                {
                    var primaryKeyType = GetTypeName(primaryKey);
                    method.AddParameter(primaryKeyType, $"key{primaryKey.Name.ToPascalCase()}", configure);
                    query += $"{lambdaParameter}.{primaryKey.Name.ToPascalCase()} == key{primaryKey.Name.ToPascalCase()} &&";
                }
            }


            return RemoveLastOccurrence(query, " &&");
        }

        public static string RemoveLastOccurrence(string source, string toRemove)
        {
            int lastIndex = source.LastIndexOf(toRemove);
            if (lastIndex == -1)
                return source; // substring not found

            return source.Remove(lastIndex, toRemove.Length);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}