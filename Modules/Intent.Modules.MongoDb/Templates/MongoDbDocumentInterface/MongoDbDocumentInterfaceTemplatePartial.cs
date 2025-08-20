using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MongoDb.Templates.MongoDbDocumentInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MongoDbDocumentInterfaceTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.MongoDb.MongoDbDocumentInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MongoDbDocumentInterfaceTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name}Document", @interface =>
                {
                    foreach (var genericType in model.GenericTypes)
                    {
                        @interface.AddGenericParameter(genericType);
                    }

                    if (model.ParentClass != null)
                    {
                        var genericTypeArguments = model.ParentClass.GenericTypes.Any()
                            ? $"<{string.Join(", ", model.ParentClassTypeReference.GenericTypeParameters.Select(GetTypeName))}>"
                            : string.Empty;
                        @interface.ExtendsInterface($"{this.GetMongoDbDocumentInterfaceName(model.ParentClass)}{genericTypeArguments}");
                    }
                })
                .OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var entityPropertyIds = EntityStateFileBuilder.CSharpFile.Classes.First().Properties
                        .Select(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel
                                ? metadataModel.Id
                                : null)
                        .Where(x => x != null)
                        .ToHashSet();

                    var attributes = Model.Attributes
                        .Where(x => entityPropertyIds.Contains(x.Id))
                        .ToList();
                    var associationEnds = Model.AssociatedClasses
                        .Where(x => entityPropertyIds.Contains(x.Id) && x.IsNavigable)
                        .ToList();

                    foreach (var attribute in attributes)
                    {
                        string typeName = GetTypeName(attribute.TypeReference);

                        @interface.AddProperty(typeName, attribute.Name.ToPascalCase(), p => p.WithoutSetter());
                    }

                    foreach (var associationEnd in associationEnds)
                    {
                        @interface.AddProperty(GetTypeName(associationEnd), associationEnd.Name.ToPascalCase(), p => p.WithoutSetter());
                    }
                }, 1000);
        }

        public ICSharpFileBuilderTemplate EntityStateFileBuilder => GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model);

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