using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates.MessageExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class MessageExtensionsTemplate : CSharpTemplateBase<MessageModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.DomainMapping.MessageExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageExtensionsTemplate(IOutputTarget outputTarget, MessageModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource("Domain.Enum");

            CSharpFile = new CSharpFile($"{Model.InternalElement.Package.Name.ToPascalCase()}", this.GetFolderPath())
                .AddClass($"{Model.Name.RemoveSuffix("Event")}EventExtensions", @class =>
                {
                    @class.Static();
                    var messageTemplate = GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, model);
                    @class.AddMethod(GetTypeName(model.InternalElement), $"MapTo{messageTemplate.ClassName}", method =>
                    {
                        method.Static();
                        method.AddParameter(GetTypeName(model.GetMapFromDomainMapping()), "projectFrom", param => param.WithThisModifier());

                        var domainEntity = ((IElement)model.GetMapFromDomainMapping().Element).AsClassModel();
                        
                        var codeLines = new CSharpStatementAggregator();
                        codeLines.Add($"return new {GetTypeName(model.InternalElement)}");
                        codeLines.Add(new CSharpStatementBlock()
                            .AddStatements(model.Properties.Select(x => $"{x.Name.ToCSharpIdentifier(CapitalizationBehaviour.AsIs)} = projectFrom.{string.Join(".", x.InternalElement.MappedElement.Path.Select(y => y.Name))},"))
                            //.AddStatements(GetMessagePropertyAssignments("projectFrom", domainEntity, model.Properties))
                            .WithSemicolon());
                        method.AddStatements(codeLines.ToList());
                    });
                });
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
        
        // When we do need more advanced mapping, we can use this. 
        
        // private IEnumerable<CSharpStatement> GetMessagePropertyAssignments(string domainEntityVar, ClassModel domainModel, IList<PropertyModel> messageProperties)
        // {
        //     var codeLines = new List<CSharpStatement>();
        //     foreach (var propertyModel in messageProperties)
        //     {
        //         var mappedPropertyElement = propertyModel.InternalElement.MappedElement; 
        //         
        //         if (mappedPropertyElement?.Element == null
        //             && domainModel.Attributes.All(p => p.Name != propertyModel.Name))
        //         {
        //             codeLines.Add($"#warning No matching property found for {propertyModel.Name}");
        //             continue;
        //         }
        //
        //         var entityVarExpr = !string.IsNullOrWhiteSpace(domainEntityVar) ? $"{domainEntityVar}." : string.Empty;
        //         switch (mappedPropertyElement?.Element?.SpecializationTypeId)
        //         {
        //             default:
        //                 var mappedPropertyName = mappedPropertyElement?.Element?.Name ?? "<null>";
        //                 codeLines.Add($"#warning No matching type for Domain: {mappedPropertyName} and Message: {propertyModel.Name}");
        //                 break;
        //             case null:
        //             case AttributeModel.SpecializationTypeId:
        //                 var attribute = mappedPropertyElement.Element.AsAttributeModel()
        //                                 ?? domainModel.Attributes.First(p => p.Name == propertyModel.Name);
        //                 
        //                 codeLines.Add($"{propertyModel.Name.ToPascalCase()} = {entityVarExpr}{attribute.Name.ToPascalCase()},");
        //
        //                 break;
        //             case AssociationTargetEndModel.SpecializationTypeId:
        //             {
        //                 var association = mappedPropertyElement.Element.AsAssociationTargetEndModel();
        //                 var targetType = association.Element.AsClassModel();
        //                 var attributeName = association.Name.ToPascalCase();
        //
        //                 if (association.Association.AssociationType == AssociationType.Aggregation)
        //                 {
        //                     codeLines.Add($@"#warning Field not a composite association: {propertyModel.Name.ToPascalCase()}");
        //                     break;
        //                 }
        //
        //                 if (association.Multiplicity is Multiplicity.One or Multiplicity.ZeroToOne)
        //                 {
        //                     if (association.IsNullable)
        //                     {
        //                         codeLines.Add(
        //                             $"{propertyModel.Name.ToPascalCase()} = {entityVarExpr}{attributeName} != null ? {GetCreateMethodName(targetType, attributeName)}({entityVarExpr}{propertyModel.Name.ToPascalCase()}) : null,");
        //                     }
        //                     else
        //                     {
        //                         codeLines.Add($"{entityVarExpr}{attributeName} = {GetCreateMethodName(targetType, attributeName)}({entityVarExpr}{propertyModel.Name.ToPascalCase()}),");
        //                     }
        //                 }
        //                 else
        //                 {
        //                     codeLines.Add(
        //                         $"{entityVarExpr}{attributeName} = {entityVarExpr}{propertyModel.Name.ToPascalCase()}{(association.IsNullable ? "?" : "")}.Select({GetCreateMethodName(targetType, attributeName)}).ToList(),");
        //                 }
        //
        //                 var @class = this.CSharpFile.Classes.First();
        //                 @class.AddMethod(this.GetTypeName(targetType.InternalElement),
        //                     GetCreateMethodName(targetType, attributeName),
        //                     method => method.Private()
        //                         .AddAttribute(CSharpIntentManagedAttribute.Fully())
        //                         .AddParameter(this.GetTypeName((IElement)propertyModel.TypeReference.Element), "dto")
        //                         .AddStatement($"return new {targetType.Name.ToPascalCase()}")
        //                         .AddStatement(new CSharpStatementBlock()
        //                             .AddStatements(GetMessagePropertyAssignments("projectFrom", targetType,
        //                                 ((IElement)propertyModel.TypeReference.Element).ChildElements.Where(x => x.IsPropertyModel()).Select(x => x.AsPropertyModel()).ToList()))
        //                             .WithSemicolon()));
        //             }
        //                 break;
        //         }
        //     }
        //
        //     return codeLines;
        // }
        
        // private string GetCreateMethodName(ClassModel classModel, [CanBeNull] string attributeName)
        // {
        //     return $"Create{(!string.IsNullOrEmpty(attributeName) ? attributeName : string.Empty)}{classModel.Name.ToPascalCase()}";
        // }
    }
}