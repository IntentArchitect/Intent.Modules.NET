using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Drift;
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

        /// <summary>
        /// A Class. Its primary-constructor parameters come from its attributes when it's a positional record, or from the
        /// constructor marked Primary Constructor. On a record those parameters are the record's properties, so attributes with
        /// the same names aren't declared again; on a class they're plain parameters and every attribute stays a property.
        /// </summary>
        public static void AddEntity(CSharpFile file, ICSharpTemplate template, ClassModel model)
        {
            var kind = model.GetContractTypeKind();
            var primaryConstructorModel = model.GetContractPrimaryConstructor(kind);
            file.AddContractType(model.InternalElement, model.Name, kind, @class =>
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

                CSharpConstructor primaryConstructor;
                if (primaryConstructorModel != null)
                {
                    primaryConstructor = @class.AddContractPrimaryConstructor(template, primaryConstructorModel);
                    var parameterNames = primaryConstructorModel.Parameters.Select(x => x.Name).ToList();
                    @class.AddContractProperties(template, kind.IsRecord()
                        ? model.Attributes.Where(x => !parameterNames.Contains(x.Name))
                        : model.Attributes);
                }
                else
                {
                    primaryConstructor = @class.AddContractAttributes(template, kind, model.Attributes);
                }

                @class.AddContractNavigationProperties(template, model.AssociatedClasses);
                @class.AddStubbedConstructors(template, model.Constructors);
                @class.AddStubbedMethods(template, model.Operations);

                // A Class can also call a base primary constructor from an ordinary constructor (": base(...)"), so base
                // arguments are only written when this Class declares parameters on the type itself.
                if (primaryConstructor != null && model.ParentClass != null)
                {
                    primaryConstructor.PassBasePrimaryParameters(model.InternalElement, $"Class '{model.Name}'",
                        primaryConstructor.Parameters.Select(x => x.Name).ToList(), $"Class '{model.ParentClass.Name}'", GetPrimaryParameterNames(model.ParentClass));
                }

                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), template.IsInheritedByAnyClass(model));
                AddModellable(@class, typeParameters: true, parameterList: true, properties: true, methods: true, constructors: true);
            },
                baseKind: model.ParentClass?.GetContractTypeKind());
        }

        public static void AddEnum(CSharpFile file, ICSharpTemplate template, EnumModel model)
        {
            file.AddEnum(model.Name, @enum =>
            {
                @enum.AddMetadata(Modellable.MetadataKey, new Modellable { Kinds = new[] { "enum" } });

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
            var kind = model.GetContractTypeKind();
            file.AddContractType(model.InternalElement, model.Name, kind, @class =>
            {
                @class.RepresentsModel(model);
                @class.AddContractAttributes(template, kind, model.Attributes);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
                AddModellable(@class, parameterList: kind.IsRecord(), properties: true);
            });
        }

        public static void AddRepository(CSharpFile file, ICSharpTemplate template, RepositoryModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
            {
                @class.RepresentsModel(model);
                @class.AddStubbedMethods(template, model.Operations);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
                AddModellable(@class, methods: true);
            });
        }

        public static void AddDataContract(CSharpFile file, ICSharpTemplate template, DataContractModel model)
        {
            var kind = model.GetContractTypeKind();
            file.AddContractType(model.InternalElement, model.Name, kind, @class =>
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

                var primaryConstructor = @class.AddContractAttributes(template, kind, model.Attributes);
                if (model.BaseDataContract != null && model.BaseDataContract.GetContractTypeKind() == ContractTypeKind.PositionalRecord)
                {
                    primaryConstructor.PassBasePrimaryParameters(model.InternalElement, $"Data Contract '{model.Name}'",
                        model.Attributes.Select(x => x.Name).ToList(), $"Data Contract '{model.BaseDataContract.Name}'", model.BaseDataContract.Attributes.Select(x => x.Name).ToList());
                }

                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), template.IsInheritedByAnyDataContract(model));
                AddModellable(@class, typeParameters: true, parameterList: kind.IsRecord(), properties: true);
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
            var kind = model.GetDomainEventContractTypeKind();
            file.AddContractType(model, model.Name, kind, @class =>
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

                var properties = model.GetDomainEventProperties().ToList();
                var primaryConstructor = @class.AddDomainEventFields(template, kind, properties);
                if (baseEvent != null && baseEvent.GetDomainEventContractTypeKind() == ContractTypeKind.PositionalRecord)
                {
                    primaryConstructor.PassBasePrimaryParameters(model, $"Domain Event '{model.Name}'",
                        properties.Select(x => x.Name).ToList(), $"Domain Event '{baseEvent.Name}'", baseEvent.GetDomainEventProperties().Select(x => x.Name).ToList());
                }

                @class.ApplyContractModifiers(template, model, model.GetDomainEventModifiers(), template.IsInheritedByAnyDomainEvent(model));
                AddModellable(@class, typeParameters: true, parameterList: kind.IsRecord(), properties: true);
            },
                baseKind: baseEvent?.GetDomainEventContractTypeKind());
        }

        /// <summary>
        /// The parameters a Class declares on the type: its attributes when it's a positional record, otherwise its Primary
        /// Constructor's parameters, otherwise none.
        /// </summary>
        private static IReadOnlyList<string> GetPrimaryParameterNames(ClassModel model)
        {
            if (model.GetContractTypeKind() == ContractTypeKind.PositionalRecord)
            {
                return model.Attributes.Select(x => x.Name).ToList();
            }

            return model.Constructors.FirstOrDefault(x => x.HasPrimaryConstructor())?.Parameters.Select(x => x.Name).ToList()
                ?? new List<string>();
        }

        public static void AddDomainService(CSharpFile file, ICSharpTemplate template, DomainServiceModel model)
        {
            file.AddContractType(model.InternalElement, model.Name, model.GetContractTypeKind(), @class =>
            {
                @class.RepresentsModel(model);
                @class.AddStubbedMethods(template, model.Operations);
                @class.ApplyContractModifiers(template, model.InternalElement, model.GetTypeConfiguration()?.Modifiers(), isInherited: false);
                AddModellable(@class, methods: true);
            });
        }

        /// <summary>
        /// Records on a type what its element can state (<see cref="Modellable"/>), the only parts the drift check
        /// compares. Type Configuration declares a class or a record; only a Class has constructors and a Primary
        /// Constructor, and otherwise only a positional record puts parameters on the type. A Repository's or Domain
        /// Service's primary constructor, which holds its injected dependencies, is nothing the model can state.
        /// </summary>
        private static void AddModellable(CSharpClass type, bool typeParameters = false, bool parameterList = false, bool properties = false,
            bool methods = false, bool constructors = false)
        {
            type.AddMetadata(Modellable.MetadataKey, new Modellable
            {
                Kinds = new[] { "class", "record" },
                TypeParameters = typeParameters,
                ParameterList = parameterList,
                Properties = properties,
                Methods = methods,
                Constructors = constructors
            });
        }
    }
}
