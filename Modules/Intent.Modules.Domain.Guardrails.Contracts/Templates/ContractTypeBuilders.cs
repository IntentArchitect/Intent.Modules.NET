using System;
using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;
using EnumModel = Intent.Modules.Common.Types.Api.EnumModel;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The one place each Domain contract kind's C# type is built. A template calls its own kind's
    /// method for its own type; the owner of a multi-type file calls <see cref="AddContract"/> for
    /// every other element that shares its file. <paramref name="template"/> is always the template
    /// whose file is being built, so types and usings resolve into that file. Names are used verbatim.
    /// </summary>
    internal static class ContractTypeBuilders
    {
        /// <summary>
        /// Dispatches on the element's type to the matching builder.
        /// </summary>
        public static void AddContract(CSharpFile file, ICSharpTemplate template, IElement element)
        {
            if (element.IsClassModel())
            {
                AddEntity(file, template, element.AsClassModel());
            }
            else if (element.IsEnumModel())
            {
                AddEnum(file, template, element.AsEnumModel());
            }
            else if (element.IsValueObjectModel())
            {
                AddValueObject(file, template, element.AsValueObjectModel());
            }
            else if (element.IsRepositoryModel())
            {
                AddRepository(file, template, element.AsRepositoryModel());
            }
            else if (element.IsDataContractModel())
            {
                AddDataContract(file, template, element.AsDataContractModel());
            }
            else if (element.IsDomainServiceModel())
            {
                AddDomainService(file, template, element.AsDomainServiceModel());
            }
            else if (element.IsDomainEvent())
            {
                AddDomainEvent(file, template, element);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported element type '{element.SpecializationType}' ({element.Id}) for a Domain guardrail contract.");
            }
        }

        public static void AddEntity(CSharpFile file, ICSharpTemplate template, ClassModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
                {
                    @class.RepresentsModel(model);
                    foreach (var genericType in model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    if (model.ParentClassTypeReference != null)
                    {
                        @class.WithBaseType(template.GetTypeName(model.ParentClassTypeReference));
                    }

                    @class.AddContractProperties(template, model.Attributes);
                    @class.AddContractNavigationProperties(template, model.AssociatedClasses);
                    @class.AddStubbedConstructors(template, model.Constructors);
                    @class.AddStubbedMethods(template, model.Operations);
                    @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), template.IsInheritedByAnyClass(model));
                },
                baseKind: model.ParentClass?.GetContractTypeKind());
        }

        public static void AddEnum(CSharpFile file, ICSharpTemplate template, EnumModel model)
        {
            file.AddEnum(model.Name, @enum =>
            {
                // No RepresentsModel here: Intent.Modules.Common.CSharp (3.10.5-3.10.11) throws for CSharpEnum and CSharpEnumLiteral.
                foreach (var literal in model.Literals)
                {
                    @enum.AddLiteral(literal.Name, string.IsNullOrWhiteSpace(literal.Value) ? null : literal.Value);
                }

                @enum.ApplyContractAccessModifier(template, model.InternalElement, model.GetEnumConfiguration()?.AccessModifier().Value);
            });
        }

        public static void AddValueObject(CSharpFile file, ICSharpTemplate template, ValueObjectModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
            {
                @class.RepresentsModel(model);
                @class.AddContractProperties(template, model.Attributes);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }

        public static void AddRepository(CSharpFile file, ICSharpTemplate template, RepositoryModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
            {
                @class.RepresentsModel(model);
                @class.AddStubbedMethods(template, model.Operations);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }

        public static void AddDataContract(CSharpFile file, ICSharpTemplate template, DataContractModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
                {
                    @class.RepresentsModel(model);
                    foreach (var genericType in model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    // BaseDataContract is the target of the Data Contract Generalization association.
                    if (model.BaseDataContract != null)
                    {
                        @class.WithBaseType(template.GetTypeName(GuardrailsContractsDomainRoles.DataContract, model.BaseDataContract));
                    }

                    @class.AddContractProperties(template, model.Attributes);
                    @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), template.IsInheritedByAnyDataContract(model));
                },
                baseKind: model.BaseDataContract?.GetContractTypeKind());
        }

        /// <summary>
        /// A Domain Event, read from raw metadata (see <see cref="DomainEventsMetadata"/>). Events have no operations;
        /// Domain Event Origin associations (which operations raise an event) generate nothing.
        /// </summary>
        public static void AddDomainEvent(CSharpFile file, ICSharpTemplate template, IElement model)
        {
            var baseEvent = model.GetBaseDomainEvent();
            file.AddContractType(model, model.Name, model.GetDomainEventContractTypeKind(), @class =>
                {
                    @class.RepresentsModel(model);
                    foreach (var genericType in model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType.Name);
                    }

                    if (model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    if (baseEvent != null)
                    {
                        @class.WithBaseType(template.GetTypeName(GuardrailsContractsDomainRoles.DomainEvent, baseEvent));
                    }

                    @class.AddDomainEventProperties(template, model.GetDomainEventProperties());
                    @class.ApplyContractModifiers(template, model, model.GetDomainEventModifiers(), template.IsInheritedByAnyDomainEvent(model));
                },
                baseKind: baseEvent?.GetDomainEventContractTypeKind());
        }

        public static void AddDomainService(CSharpFile file, ICSharpTemplate template, DomainServiceModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
            {
                @class.RepresentsModel(model);
                @class.AddStubbedMethods(template, model.Operations);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
            });
        }
    }
}
