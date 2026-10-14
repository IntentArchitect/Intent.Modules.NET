using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using CommonTypesOperationModel = Intent.Modules.Common.Types.Api.OperationModel;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Shared building blocks for the Domain guardrail contract templates. Each generated file is merged
    /// into the developer's existing file: it guarantees the modelled members exist, and never implements
    /// or rewrites behaviour. Element names are always used verbatim.
    /// </summary>
    internal static class ContractBuilderExtensions
    {
        private const string OperationCollectionFormat = "System.Collections.Generic.IEnumerable<{0}>";

        /// <summary>
        /// Registers every Domain contract role as a type source and formats Domain collections as <c>ICollection&lt;T&gt;</c>.
        /// </summary>
        public static void ConfigureContractTypeResolution<TModel>(this CSharpTemplateBase<TModel> template)
        {
            template.SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            foreach (var role in GuardrailsContractsDomainRoles.All)
            {
                template.AddTypeSource(role);
            }
        }

        /// <summary>
        /// The merge directives every contract file carries (user decision): the file-wide
        /// <c>[assembly: DefaultIntentManaged(Mode.Merge)]</c>, plus a default for constructors and methods,
        /// <c>[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]</c>.
        /// Same as the Services module's; keep the two in step.
        /// </summary>
        public static CSharpFile IntentManagedContractMerge(this CSharpFile file)
        {
            return file
                .IntentManagedMerge()
                .AddAssemblyAttribute(CSharpDefaultIntentManagedAttribute.Merge()
                    .WithSignatureFully()
                    .AddArgument("Attributes = Mode.Merge")
                    .AddArgument("Comments = Mode.Merge")
                    .AddArgument("Targets = Targets.Constructors | Targets.Methods"));
        }

        // Names are always passed as strings: the builder's model overloads (e.g. AddProperty(type, model)) Pascal-case
        // the name, and contracts must keep the codebase's own casing exactly as modelled in the designer.
        public static void AddContractProperties(this CSharpClass @class, ICSharpTemplate template, IEnumerable<AttributeModel> attributes)
        {
            foreach (var attribute in attributes)
            {
                @class.AddProperty(template.GetPropertyTypeName(attribute.TypeReference, attribute.GetCollectionFormat()), attribute.Name, property =>
                {
                    property.RepresentsModel(attribute);
                    property.WithContractInitialValue(template, attribute.Value, attribute.TypeReference);
                });
            }
        }

        /// <summary>
        /// A Domain Event's Properties (raw elements, see <see cref="DomainEventsMetadata"/>), formatted like attribute properties.
        /// </summary>
        public static void AddDomainEventProperties(this CSharpClass @class, ICSharpTemplate template, IEnumerable<IElement> properties)
        {
            foreach (var property in properties)
            {
                @class.AddProperty(template.GetPropertyTypeName(property.TypeReference, property.GetDomainEventPropertyCollectionFormat()), property.Name, p =>
                {
                    p.RepresentsModel(property);
                    p.WithContractInitialValue(template, property.Value, property.TypeReference);
                });
            }
        }

        public static void AddContractNavigationProperties(this CSharpClass @class, ICSharpTemplate template, IEnumerable<AssociationEndModel> associationEnds)
        {
            foreach (var associationEnd in associationEnds.Where(x => x.IsNavigable))
            {
                @class.AddProperty(template.GetPropertyTypeName(associationEnd, associationEnd.GetCollectionFormat()), associationEnd.Name, property => property.RepresentsModel(associationEnd));
            }
        }

        /// <summary>
        /// Every constructor except the one marked Primary Constructor (see <see cref="AddContractPrimaryConstructor"/>), with an
        /// empty body.
        /// </summary>
        public static void AddStubbedConstructors(this CSharpClass @class, ICSharpTemplate template, IEnumerable<ClassConstructorModel> constructors)
        {
            foreach (var constructor in constructors.Where(x => !x.HasPrimaryConstructor()))
            {
                @class.AddConstructor(ctor =>
                {
                    ctor.RepresentsModel(constructor);
                    ctor.AddContractConstructorParameters(template, constructor);
                });
            }
        }

        /// <summary>
        /// The constructor marked with the Primary Constructor stereotype, or null when none is. A C# type has at most one
        /// primary constructor, and a positional record's positional parameters are its attributes, so it can't have one too.
        /// </summary>
        public static ClassConstructorModel GetContractPrimaryConstructor(this ClassModel model, ContractTypeKind kind)
        {
            var primaryConstructors = model.Constructors.Where(x => x.HasPrimaryConstructor()).ToList();
            if (primaryConstructors.Count > 1)
            {
                throw new ElementException(model.InternalElement,
                    $"Class '{model.Name}' has {primaryConstructors.Count} constructors marked Primary Constructor, but a C# type can only have one primary constructor. " +
                    "Keep the stereotype only on the constructor the code declares on the type itself.");
            }

            if (primaryConstructors.Count == 1 && kind == ContractTypeKind.PositionalRecord)
            {
                throw new ElementException(model.InternalElement,
                    $"Class '{model.Name}' is a positional record, whose attributes are its positional parameters, and it also has a constructor marked Primary Constructor. " +
                    "Use one or the other: set Type to record to keep the primary constructor's parameters, or remove the stereotype to make the attributes the positional parameters.");
            }

            return primaryConstructors.SingleOrDefault();
        }

        /// <summary>
        /// The constructor marked Primary Constructor, declared on the type itself, e.g. <c>class OrderService(IOrderRepository repository)</c>
        /// or <c>record Money(decimal Amount, string Currency)</c>. Its parameters are written as for any constructor (names verbatim,
        /// Parameter Modifier, defaults from Values); a primary constructor has no body.
        /// </summary>
        public static CSharpConstructor AddContractPrimaryConstructor(this CSharpClass @class, ICSharpTemplate template, ClassConstructorModel constructor)
        {
            CSharpConstructor primaryConstructor = null;
            @class.AddPrimaryConstructor(ctor =>
            {
                primaryConstructor = ctor;
                ctor.RepresentsModel(constructor);
                ctor.AddContractConstructorParameters(template, constructor);
            });
            return primaryConstructor;
        }

        private static void AddContractConstructorParameters(this CSharpConstructor ctor, ICSharpTemplate template, ClassConstructorModel constructor)
        {
            foreach (var parameter in constructor.Parameters)
            {
                var modifier = parameter.GetModifier();
                ctor.AddParameter(template.GetOperationTypeName(parameter.TypeReference, parameter.GetCollectionFormat()).WithModifierPrefix(modifier, isConstructorParameter: true), parameter.Name, param =>
                {
                    param.RepresentsModel(parameter);
                    param.WithContractDefaultValue(template, parameter.InternalElement, parameter.Value, parameter.TypeReference, modifier);
                });
            }
        }

        /// <summary>
        /// The attributes as properties, or for a <see cref="ContractTypeKind.PositionalRecord"/> as the positional parameters of its
        /// primary constructor, e.g. <c>record Money(decimal Amount, string Currency)</c>: in model order, names verbatim, typed as the
        /// properties they declare, with defaults from Values. Returns that primary constructor, or null when they are properties.
        /// </summary>
        public static CSharpConstructor AddContractAttributes(this CSharpClass @class, ICSharpTemplate template, ContractTypeKind kind, IEnumerable<AttributeModel> attributes)
        {
            if (kind != ContractTypeKind.PositionalRecord)
            {
                @class.AddContractProperties(template, attributes);
                return null;
            }

            return @class.AddContractPositionalParameters(attributes, (ctor, attribute) =>
                ctor.AddParameter(template.GetPropertyTypeName(attribute.TypeReference, attribute.GetCollectionFormat()), attribute.Name, param =>
                {
                    param.RepresentsModel(attribute);
                    param.WithContractDefaultValue(template, attribute.InternalElement, attribute.Value, attribute.TypeReference, modifier: null);
                }));
        }

        /// <summary>
        /// The same for a Domain Event's Properties (raw elements, see <see cref="DomainEventsMetadata"/>).
        /// </summary>
        public static CSharpConstructor AddDomainEventFields(this CSharpClass @class, ICSharpTemplate template, ContractTypeKind kind, IEnumerable<IElement> properties)
        {
            if (kind != ContractTypeKind.PositionalRecord)
            {
                @class.AddDomainEventProperties(template, properties);
                return null;
            }

            return @class.AddContractPositionalParameters(properties, (ctor, property) =>
                ctor.AddParameter(template.GetPropertyTypeName(property.TypeReference, property.GetDomainEventPropertyCollectionFormat()), property.Name, param =>
                {
                    param.RepresentsModel(property);
                    param.WithContractDefaultValue(template, property, property.Value, property.TypeReference, modifier: null);
                }));
        }

        private static CSharpConstructor AddContractPositionalParameters<TField>(this CSharpClass @class, IEnumerable<TField> fields, Action<CSharpConstructor, TField> addParameter)
        {
            CSharpConstructor primaryConstructor = null;
            @class.AddPrimaryConstructor(ctor =>
            {
                primaryConstructor = ctor;
                foreach (var field in fields)
                {
                    addParameter(ctor, field);
                }
            });
            return primaryConstructor;
        }

        /// <summary>
        /// A type with primary-constructor parameters that inherits from one passes the base's parameters on by name, e.g.
        /// <c>record Child(Guid Id, string Name) : Base(Id)</c>. The derived type must declare a parameter for each of the
        /// base's, which is how the code has to be written. Same rule as the Services module's.
        /// </summary>
        public static void PassBasePrimaryParameters(this CSharpConstructor primaryConstructor, IElement element, string elementDescription, IReadOnlyCollection<string> ownParameters, string baseDescription, IReadOnlyList<string> baseParameters)
        {
            if (baseParameters.Count == 0)
            {
                return;
            }

            if (primaryConstructor == null)
            {
                throw new ElementException(element,
                    $"{elementDescription} inherits from {baseDescription}, which declares its parameters on the type ({string.Join(", ", baseParameters)}), so it has to pass them to the base constructor. " +
                    $"Set {element.Name}'s Type Configuration Type to positional record, with attributes for those parameters, as the code declares it.");
            }

            var missing = baseParameters.Where(x => !ownParameters.Contains(x)).ToList();
            if (missing.Count > 0)
            {
                throw new ElementException(element,
                    $"{elementDescription} inherits from {baseDescription}, which declares its parameters on the type, so {element.Name}'s own primary-constructor parameters must include them to pass on to the base constructor. " +
                    $"Add the missing ones to {element.Name}, as the code declares them: {string.Join(", ", missing)}.");
            }

            primaryConstructor.CallsBase(call =>
            {
                foreach (var name in baseParameters)
                {
                    call.AddArgument(name);
                }
            });
        }

        public static void AddStubbedMethods(this CSharpClass @class, ICSharpTemplate template, IEnumerable<OperationModel> operations)
        {
            foreach (var operation in operations)
            {
                @class.AddMethod(template.GetOperationReturnType(operation), operation.Name, method =>
                {
                    method.RepresentsModel(operation);
                    foreach (var genericType in operation.GenericTypes)
                    {
                        method.AddGenericParameter(genericType);
                    }

                    if (operation.IsStatic)
                    {
                        method.Static();
                    }

                    foreach (var parameter in operation.Parameters)
                    {
                        var modifier = parameter.GetModifier();
                        method.AddParameter(template.GetOperationTypeName(parameter.TypeReference, parameter.GetCollectionFormat()).WithModifierPrefix(modifier, isConstructorParameter: false), parameter.Name, param =>
                        {
                            param.RepresentsModel(parameter);
                            param.ApplyModifier(modifier);
                            param.WithContractDefaultValue(template, parameter.InternalElement, parameter.Value, parameter.TypeReference, modifier);
                        });
                    }

                    if (operation.IncludeCancellationToken())
                    {
                        method.AddParameter(template.UseType("System.Threading.CancellationToken"), "cancellationToken", param => param.WithDefaultValue("default"));
                    }

                    if (operation.IsAbstract)
                    {
                        // An abstract member cannot have a body (so no async keyword), and C# requires its declaring class to be abstract.
                        method.Abstract();
                        if (!@class.IsAbstract)
                        {
                            @class.Abstract();
                        }
                        return;
                    }

                    // Under Mode.Merge the existing method's modifiers are replaced with these, so async must be emitted
                    // to keep async/await implementations compiling.
                    if (operation.IsAsync())
                    {
                        // Async() with no argument converts a ValueTask return type to Task; keep a modelled ValueTask as is.
                        if (method.ReturnTypeInfo.IsValueTask())
                        {
                            method.Async(asValueTask: true);
                        }
                        else
                        {
                            method.Async();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// An explicit Async Configuration stereotype wins; otherwise an operation is async when its name ends with "Async".
        /// </summary>
        public static bool IsAsync(this OperationModel operation)
        {
            var asyncConfiguration = operation.GetAsyncConfiguration();
            return asyncConfiguration?.IsAsync() ?? operation.Name.EndsWith("Async", StringComparison.Ordinal);
        }

        /// <summary>
        /// Async operations get a CancellationToken parameter unless the Async Configuration stereotype turns it off.
        /// </summary>
        private static bool IncludeCancellationToken(this OperationModel operation)
        {
            return operation.IsAsync() && (operation.GetAsyncConfiguration()?.IncludeCancellationToken() ?? true);
        }

        // The stereotype accessor is generated against the Common.Types operation model; both wrap the same designer element.
        private static OperationModelStereotypeExtensions.AsyncConfiguration GetAsyncConfiguration(this OperationModel operation)
        {
            return new CommonTypesOperationModel(operation.InternalElement).GetAsyncConfiguration();
        }

        private static string GetOperationReturnType(this ICSharpTemplate template, OperationModel operation)
        {
            var hasReturnType = operation.TypeReference?.Element != null;
            if (!operation.IsAsync())
            {
                return hasReturnType ? template.GetOperationTypeName(operation.TypeReference, operation.GetCollectionFormat()) : "void";
            }

            if (!hasReturnType)
            {
                return template.UseType("System.Threading.Tasks.Task");
            }

            // A return type already modelled as Task / Task<T> / ValueTask / ValueTask<T> is not wrapped again.
            var returnType = template.GetOperationTypeName(operation.TypeReference, operation.GetCollectionFormat());
            return IsAwaitableType(returnType)
                ? returnType
                : $"{template.UseType("System.Threading.Tasks.Task")}<{returnType}>";
        }

        private static bool IsAwaitableType(string typeName)
        {
            var genericStart = typeName.IndexOf('<');
            var name = (genericStart >= 0 ? typeName.Substring(0, genericStart) : typeName).TrimEnd('?');
            var lastDot = name.LastIndexOf('.');
            name = lastDot >= 0 ? name.Substring(lastDot + 1) : name;
            return name is "Task" or "ValueTask";
        }

        /// <summary>
        /// Properties format collections with the template default (<c>ICollection&lt;T&gt;</c>) unless a Collection Type
        /// stereotype supplied <paramref name="collectionFormat"/> (see <see cref="ContractCollectionTypes"/>).
        /// </summary>
        private static string GetPropertyTypeName(this ICSharpTemplate template, ITypeReference typeReference, string collectionFormat)
        {
            return collectionFormat == null
                ? template.GetTypeName(typeReference)
                : template.GetTypeName(typeReference, template.UseType(collectionFormat));
        }

        /// <summary>
        /// Operation parameters and return types format collections as <c>IEnumerable&lt;T&gt;</c>, mirroring Intent.Entities,
        /// unless a Collection Type stereotype supplied <paramref name="collectionFormat"/> (see <see cref="ContractCollectionTypes"/>).
        /// UseType adds the format's using; a format with no namespace (e.g. <c>{0}[]</c>) passes through unchanged.
        /// </summary>
        private static string GetOperationTypeName(this ICSharpTemplate template, ITypeReference typeReference, string collectionFormat)
        {
            return template.GetTypeName(typeReference, template.UseType(collectionFormat ?? OperationCollectionFormat));
        }
    }
}
