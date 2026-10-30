using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.ValueObjects.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObject
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValueObjectTemplate : CSharpTemplateBase<ValueObjectModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ValueObjects.ValueObject";

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValueObjectTemplate(IOutputTarget outputTarget, ValueObjectModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());

            var type = ExecutionContext.Settings.GetValueObjectSettings().ValueObjectType().AsEnum();
            switch (type)
            {
                case ValueObjectSettings.ValueObjectTypeOptionsEnum.Class:
                    DeclareValueObjectClassType(outputTarget);
                    break;
                case ValueObjectSettings.ValueObjectTypeOptionsEnum.Record:
                    DeclareValueObjectRecordType();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid Value Object Type: {type}");
            }
        }

        private void DeclareValueObjectClassType(IOutputTarget outputTarget)
        {
            CSharpFile.AddClass(Model.Name, @class =>
            {
                @class.RepresentsModel(Model);
                @class.WithBaseType(this.GetValueObjectBaseName());
                if (Model.HasSerializationSettings())
                {
                    @class.AddMetadata("serialization", Model.GetSerializationSettings().Type().Value);
                }

                var associationEnds = GetValueObjectAssociationEnds();
                var nullableEnabled = outputTarget.GetProject().IsNullableAwareContext() && outputTarget.GetProject().NullableEnabled;
                var hasCollections = Model.Attributes.Any(x => x.TypeReference.IsCollection) || associationEnds.Any(x => x.IsCollection);
                if (hasCollections)
                {
                    // Count() is used in GetEqualityComponents; the builder drops this where implicit usings cover it
                    CSharpFile.AddUsing("System.Linq");
                }

                @class.AddConstructor(ctor =>
                {
                    @class.AddMethod(
                        returnType: $"{UseType("System.Collections.Generic.IEnumerable")}<object>",
                        name: "GetEqualityComponents", method =>
                        {
                            method.Protected()
                                .Override();
                            if (hasCollections)
                            {
                                method.AddStatement("// Collections are prefixed with their count so adjacent collections cannot run together");
                            }

                            if (!Model.Attributes.Any() && !associationEnds.Any())
                            {
                                method.AddStatement("yield break;");
                            }

                            foreach (var attribute in Model.Attributes)
                            {
                                ctor.AddParameter(GetTypeName(attribute), attribute.Name.ToCamelCase(), param =>
                                {
                                    param.IntroduceProperty(prop =>
                                    {
                                        prop.RepresentsModel(attribute);
                                        prop.PrivateSetter();
                                        AddEqualityComponent(method, prop.Name, attribute.TypeReference, nullableEnabled);
                                    });
                                });
                            }

                            foreach (var associationEnd in associationEnds)
                            {
                                ctor.AddParameter(GetTypeName(associationEnd), associationEnd.Name.ToCamelCase(), param =>
                                {
                                    param.IntroduceProperty(prop =>
                                    {
                                        prop.RepresentsModel(associationEnd);
                                        // Read by other modules (e.g. Intent.CosmosDB) to know which association ends the class exposes
                                        prop.AddMetadata("model", associationEnd);
                                        prop.PrivateSetter();
                                        AddEqualityComponent(method, prop.Name, associationEnd, nullableEnabled);
                                    });
                                });
                            }
                        });
                });

                if (Model.Attributes.Any() || associationEnds.Any())
                {
                    var ctorCount = @class.Constructors.Count;
                    if (outputTarget.GetProject().IsNullableAwareContext() && outputTarget.GetProject().NullableEnabled)
                    {
                        @class.AddNullForgivingConstructor(ctor =>
                        {
                            ctor.AddAttribute("IntentMerge");
                            ctor.Protected();
                        });
                    }

                    // this is basically for backward compability. If the null forgiving ctor is not added,
                    // then a private empty ctor is added
                    if (ctorCount == @class.Constructors.Count)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.Protected();
                        });
                    }
                }
                ;
            });
        }

        private void DeclareValueObjectRecordType()
        {
            CSharpFile.AddRecord(Model.Name, record =>
            {
                if (Model.HasSerializationSettings())
                {
                    record.AddMetadata("serialization", Model.GetSerializationSettings().Type().Value);
                }

                record.AddPrimaryConstructor(ctor =>
                {
                    foreach (var modelAttribute in Model.Attributes)
                    {
                        ctor.AddParameter(GetTypeName(modelAttribute), modelAttribute.Name);
                    }

                    foreach (var associationEnd in GetValueObjectAssociationEnds())
                    {
                        ctor.AddParameter(GetTypeName(associationEnd), associationEnd.Name.ToPascalCase());
                    }
                });
            });
        }

        /// <summary>
        /// The navigable association ends from this Value Object to other Value Objects. Associations to
        /// anything other than a Value Object are not part of the Value Object's shape and are skipped.
        /// </summary>
        private IReadOnlyList<AssociationEndModel> GetValueObjectAssociationEnds()
        {
            return Model.InternalElement.AssociatedElements
                .Where(x => x.IsAssociationEndModel())
                .Select(x => x.AsAssociationEndModel())
                .Where(x => x.IsTargetEnd() && x.IsNavigable && x.Element.IsValueObjectModel())
                .ToList();
        }

        private static void AddEqualityComponent(CSharpClassMethod method, string propertyName, ITypeReference typeReference, bool nullableEnabled)
        {
            if (!typeReference.IsCollection)
            {
                method.AddStatement(typeReference.IsNullable && nullableEnabled
                    ? $"yield return {propertyName}!;"
                    : $"yield return {propertyName};");
                return;
            }

            // Yield the elements rather than the collection itself, otherwise two Value Objects holding equal
            // collections are never equal (collections compare by reference).
            // Prefix the elements with the count: without it, items from adjacent collections run together and
            // [a, b] + [c] equals [a] + [b, c]. It also guarantees at least one component per collection, so
            // ValueObject.GetHashCode (Aggregate without a seed) does not throw when every collection is empty.
            // A null collection counts as empty, matching the previous behaviour.
            if (typeReference.IsNullable)
            {
                method.AddStatement($"yield return {propertyName}?.Count() ?? 0;");
                method.AddIfStatement($"{propertyName} != null", @if => @if.AddForEachStatement("item", propertyName, forEach => forEach.AddStatement("yield return item;")));
            }
            else
            {
                method.AddStatement($"yield return {propertyName}.Count();");
                method.AddForEachStatement("item", propertyName, forEach => forEach.AddStatement("yield return item;"));
            }
        }

        private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !(typeInfo.IsPrimitive
                     || typeInfo.IsNullable == true
                     || (typeInfo.TypeReference != null && typeInfo.TypeReference.Element.IsEnumModel()));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}