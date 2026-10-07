using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Domain.Guardrails.Contracts.Api;
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

        // Names are always passed as strings: the builder's model overloads (e.g. AddProperty(type, model)) Pascal-case
        // the name, and contracts must keep the codebase's own casing exactly as modelled in the designer.
        public static void AddContractProperties<TModel>(this CSharpClass @class, CSharpTemplateBase<TModel> template, IEnumerable<AttributeModel> attributes)
        {
            foreach (var attribute in attributes)
            {
                @class.AddProperty(template.GetTypeName(attribute.TypeReference), attribute.Name, property => property.RepresentsModel(attribute));
            }
        }

        public static void AddContractNavigationProperties<TModel>(this CSharpClass @class, CSharpTemplateBase<TModel> template, IEnumerable<AssociationEndModel> associationEnds)
        {
            foreach (var associationEnd in associationEnds.Where(x => x.IsNavigable))
            {
                @class.AddProperty(template.GetTypeName(associationEnd), associationEnd.Name, property => property.RepresentsModel(associationEnd));
            }
        }

        public static void AddStubbedConstructors<TModel>(this CSharpClass @class, CSharpTemplateBase<TModel> template, IEnumerable<ClassConstructorModel> constructors)
        {
            foreach (var constructor in constructors)
            {
                @class.AddConstructor(ctor =>
                {
                    ctor.RepresentsModel(constructor);
                    foreach (var parameter in constructor.Parameters)
                    {
                        ctor.AddParameter(template.GetOperationTypeName(parameter.TypeReference), parameter.Name, param =>
                        {
                            param.RepresentsModel(parameter);
                            if (!string.IsNullOrWhiteSpace(parameter.Value))
                            {
                                param.WithDefaultValue(parameter.Value);
                            }
                        });
                    }
                });
            }
        }

        public static void AddStubbedMethods<TModel>(this CSharpClass @class, CSharpTemplateBase<TModel> template, IEnumerable<OperationModel> operations)
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
                        method.AddParameter(template.GetOperationTypeName(parameter.TypeReference), parameter.Name, param =>
                        {
                            param.RepresentsModel(parameter);
                            if (!string.IsNullOrWhiteSpace(parameter.Value))
                            {
                                param.WithDefaultValue(parameter.Value);
                            }
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
                        method.Async();
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

        private static string GetOperationReturnType<TModel>(this CSharpTemplateBase<TModel> template, OperationModel operation)
        {
            var hasReturnType = operation.TypeReference?.Element != null;
            if (!operation.IsAsync())
            {
                return hasReturnType ? template.GetOperationTypeName(operation.TypeReference) : "void";
            }

            if (!hasReturnType)
            {
                return template.UseType("System.Threading.Tasks.Task");
            }

            // A return type already modelled as Task / Task<T> / ValueTask / ValueTask<T> is not wrapped again.
            var returnType = template.GetOperationTypeName(operation.TypeReference);
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
        /// Operation parameters and return types format collections as <c>IEnumerable&lt;T&gt;</c>, mirroring Intent.Entities.
        /// </summary>
        private static string GetOperationTypeName<TModel>(this CSharpTemplateBase<TModel> template, ITypeReference typeReference)
        {
            return template.GetTypeName(typeReference, template.UseType(OperationCollectionFormat));
        }
    }
}
