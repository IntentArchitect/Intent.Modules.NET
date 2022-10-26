using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Cosmos.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class CosmosDbContextDecorator : DecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Cosmos.CosmosDbContextDecorator";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public CosmosDbContextDecorator(ICSharpFileBuilderTemplate template, IApplication application)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                @class.AddMethod("void", "EnsureDbCreated", method =>
                {
                    method.WithComments(@"
/// <summary>
/// Checks to see whether the database exist and if not will create it
/// based on this container configuration.
/// </summary>");
                    method.AddStatement("Database.EnsureCreated();");
                });
            });
        }
    }
}