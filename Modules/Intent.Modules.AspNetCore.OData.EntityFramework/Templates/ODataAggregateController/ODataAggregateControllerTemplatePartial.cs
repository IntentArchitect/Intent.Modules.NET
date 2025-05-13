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
                .AddUsing("System.Linq")
                .AddUsing("Microsoft.AspNetCore.OData.Routing.Attributes")
                .AddClass($"{Model.Name.RemoveSuffix("Controller", "Service").Pluralize()}Controller", @class =>
                {
                    @class.WithBaseType("ODataController");
                    @class.AddMetadata("model", Model);
                    @class.AddAttribute("[EnableQuery]");
                    @class.AddAttribute("""[ODataRouteComponent("odata")]""");

                    var dbContextInstance = DbContextManager.GetDbContext(Model);

                    @class.AddConstructor(constructor =>
                    {
                        constructor.AddParameter(dbContextInstance.GetTypeName(this), "context", p => p.IntroduceReadonlyField());
                    });

                    if (!model.TryGetExposeAsOData(out var stereotype))
                    {
                        return;
                    }

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
                            method.Async();
                            method.AddAttribute(new CSharpAttribute("[HttpGet]"));

                            var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));
                            method.AddStatement($"var {Model.Name.ToCamelCase()} = await _context.{Model.Name.Pluralize().ToPascalCase()}.FirstOrDefaultAsync({linqQuery});");
                            method.AddStatement($"return {Model.Name.ToCamelCase()} == null ? NotFound() : Ok({Model.Name.ToCamelCase()});", s => s.SeparatedFromPrevious());
                        });
                    }

                    if (stereotype.Post())
                    {
                        @class.AddMethod("IActionResult", "Post", method =>
                        {
                            method.Async();
                            method.AddAttribute(new CSharpAttribute("[HttpPost]"));

                            method.AddParameter($"{GetTypeName(Model.InternalElement)}", $"{Model.Name.ToCamelCase()}", p => p.AddAttribute("[FromBody]"));
                            method.AddStatement($"await _context.{Model.Name.Pluralize().ToPascalCase()}.AddAsync({Model.Name.ToCamelCase()});");
                            method.AddStatement($"await _context.SaveChangesAsync();");
                            method.AddStatement($"return Created({Model.Name.ToCamelCase()});", s => s.SeparatedFromPrevious());
                        });
                    }

                    if (stereotype.Delete())
                    {
                        @class.AddMethod("ActionResult", "Delete", method =>
                        {
                            method.Async();
                            method.AddAttribute(new CSharpAttribute("[HttpDelete]"));

                            var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));

                            method.AddStatement($"var {Model.Name.ToCamelCase()} = await _context.{Model.Name.Pluralize().ToPascalCase()}.SingleOrDefaultAsync({linqQuery});");
                            method.AddIfStatement($"{Model.Name.ToCamelCase()} is not null", ifStatement =>
                            {
                                ifStatement.AddStatement($"_context.{Model.Name.Pluralize().ToPascalCase()}.Remove({Model.Name.ToCamelCase()});");
                            });
                            method.AddStatement("await _context.SaveChangesAsync();");
                            method.AddStatement("return NoContent();", s => s.SeparatedFromPrevious());
                        });
                    }

                    if (stereotype.Patch())
                    {
                        @class.AddMethod("ActionResult", "Patch", method =>
                        {
                            method.Async();
                            method.AddAttribute(new CSharpAttribute("[HttpPatch]"));
                            var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));
                            method.AddParameter($"Delta<{GetTypeName(Model.InternalElement)}>", "delta", parameter =>
                            {
                                parameter.AddAttribute("[FromBody]");
                            });

                            method.AddStatement($"var {Model.Name.ToCamelCase()} = await _context.{Model.Name.Pluralize().ToPascalCase()}.SingleOrDefaultAsync({linqQuery});");
                            method.AddIfStatement($"{Model.Name.ToCamelCase()} is null", ifStatement =>
                            {
                                ifStatement.AddStatement($"return NotFound();");
                            });

                            method.AddStatement($"delta.Patch({Model.Name.ToCamelCase()});");

                            method.AddStatement("await _context.SaveChangesAsync();");
                            method.AddStatement($"return Updated({Model.Name.ToCamelCase()});", statement => statement.SeparatedFromPrevious());
                        });
                    }

                    if (stereotype.Put())
                    {
                        @class.AddMethod("ActionResult", "Put", method =>
                        {
                            method.Async();
                            method.AddAttribute(new CSharpAttribute("[HttpPut]"));
                            var linqQuery = GeneratePrimaryKeyParameters(model, method, c => c.AddAttribute("[FromODataUri]"));
                            method.AddParameter($"{GetTypeName(Model.InternalElement)}", "update", parameter =>
                            {
                                parameter.AddAttribute("[FromBody]");
                            });

                            method.AddStatement($"var {Model.Name.ToCamelCase()} = await _context.{Model.Name.Pluralize().ToPascalCase()}.AsNoTracking().SingleOrDefaultAsync({linqQuery});");
                            method.AddIfStatement($"{Model.Name.ToCamelCase()} is null", ifStatement =>
                            {
                                ifStatement.AddStatement($"return NotFound();");
                            });

                            var primaryKeys = model.Attributes.Where(att => att.HasStereotype("Primary Key")).ToArray();
                            if (primaryKeys.Length == 1)
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

                            method.AddStatement($"_context.Entry(update).State = EntityState.Modified;");
                            method.AddStatement("await _context.SaveChangesAsync();");
                            method.AddStatement($"return Updated({Model.Name.ToCamelCase()});", statement => statement.SeparatedFromPrevious());
                        });
                    }
                });
        }

        private string GeneratePrimaryKeyParameters(ClassModel model, CSharpClassMethod method, Action<CSharpParameter> configure = null)
        {
            const string lambdaParameter = "m";
            var primaryKeys = model.Attributes.Where(att => att.HasStereotype("Primary Key")).ToList();
            var conditions = new List<string>();

            foreach (var primaryKey in primaryKeys)
            {
                var propertyName = primaryKey.Name.ToPascalCase();
                var paramName = primaryKeys.Count == 1 ? "key" : $"key{propertyName}";
                var primaryKeyType = GetTypeName(primaryKey);

                method.AddParameter(primaryKeyType, paramName, primaryKeys.Count == 1 ? null : configure);
                conditions.Add($"{lambdaParameter}.{propertyName} == {paramName}");
            }

            var query = $"{lambdaParameter} => {string.Join(" && ", conditions)}";
            return query;
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