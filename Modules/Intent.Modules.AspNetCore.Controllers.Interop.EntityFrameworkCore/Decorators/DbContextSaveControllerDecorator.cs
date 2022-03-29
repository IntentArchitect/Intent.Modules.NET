using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
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

        [IntentManaged(Mode.Fully)]
        private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public DbContextSaveControllerDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            Priority = 100;
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
            if (operationModel.GetHttpSettings().Verb().IsGET())
            {
                return string.Empty;
            }
            return $@"
            await _dbContext.SaveChangesAsync(cancellationToken);";
        }
    }
}