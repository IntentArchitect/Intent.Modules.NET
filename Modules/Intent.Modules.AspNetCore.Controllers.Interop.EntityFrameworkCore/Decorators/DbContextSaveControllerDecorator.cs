using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DbContextSaveControllerDecorator : ControllerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Controllers.Interop.EntityFrameworkCore.DbContextSaveControllerDecorator";

        private readonly ControllerTemplate _template;

        public DbContextSaveControllerDecorator(ControllerTemplate template)
        {
            _template = template;
            Priority = 10;
        }

        public override string EnterClass()
        {
            return $@"
        private readonly {_template.GetTypeName(DbContextTemplate.TemplateId)} _dbContext;";
        }

        public override IEnumerable<string> ConstructorParameters()
        {
            return new[] { $"{_template.GetTypeName(DbContextTemplate.TemplateId)} dbContext" };
        }

        public override string ConstructorImplementation()
        {
            return $@"
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));";
        }

        public override string MidOperationBody(OperationModel operationModel)
        {
            return $@"
            await _dbContext.SaveChangesAsync();";
        }
    }
}