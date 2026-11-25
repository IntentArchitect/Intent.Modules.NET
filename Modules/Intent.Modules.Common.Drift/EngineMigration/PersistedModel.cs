using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// The application's model as the drift engine reads it, Intent's model interfaces, over the migration's own persistence
    /// objects, so the engine reads the model the move is changing before anything is saved. Intent's implementation of those
    /// interfaces is the client's, and the module task host reads it through caches nothing clears, so it would show the model
    /// as it was before the move.
    /// <para>
    /// Each interface is a <see cref="DispatchProxy"/>: a member a later SDK adds answers its type's default instead of failing
    /// the type's load, as a class implementing the interfaces of the SDK this compiles against would. One proxy per persistence
    /// object, so the engine's reference comparisons hold.
    /// </para>
    /// </summary>
    internal sealed class PersistedModel
    {
        private readonly string _applicationId;
        private readonly Dictionary<object, Dictionary<Type, object>> _proxies = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<string, ElementPersistable> _elements = new(StringComparer.Ordinal);
        private readonly Dictionary<ElementPersistable, (PackageModelPersistable Package, ElementPersistable Parent)> _placement = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<string, (AssociationEndPersistable End, AssociationPersistable Association, PackageModelPersistable Package)> _ends = new(StringComparer.Ordinal);
        private readonly Dictionary<AssociationPersistable, string> _sources = new(ReferenceEqualityComparer.Instance);
        private readonly Dictionary<string, List<AssociationEndPersistable>> _associated = new(StringComparer.Ordinal);
        private readonly Dictionary<string, IElement> _stubs = new(StringComparer.Ordinal);
        private readonly Dictionary<string, StereotypeDefinitionPersistable> _definitions = new(StringComparer.Ordinal);
        private readonly Dictionary<string, List<ElementPersistable>> _inFolder = new(StringComparer.Ordinal);
        private readonly Dictionary<PackageModelPersistable, List<ElementPersistable>> _roots = new(ReferenceEqualityComparer.Instance);
        private readonly HashSet<string> _packageIds = new(StringComparer.Ordinal);

        private PersistedModel(string applicationId)
        {
            _applicationId = applicationId;
        }

        /// <summary>Each designer's packages this application owns, by the name the engine's packs give the designer.</summary>
        public Dictionary<string, IReadOnlyList<IPackage>> Designers { get; } = new(StringComparer.Ordinal);

        /// <summary>Each designer's packages other applications own: read for what they say, never claimed on.</summary>
        public Dictionary<string, IReadOnlyList<IPackage>> References { get; } = new(StringComparer.Ordinal);

        /// <summary>Every package read, with the designer it belongs to (null for a module's package).</summary>
        public List<(PackageModelPersistable Package, ApplicationDesignerPersistable Designer)> Packages { get; } = new();

        /// <param name="designerNames">The designers the engine reads, by id, with the name its packs give each.</param>
        /// <param name="moduleCache">The solution's <c>.cache/modules</c> folder, where installed modules' packages are.</param>
        public static PersistedModel Read(ApplicationPersistable application, IReadOnlyDictionary<string, string> designerNames, string moduleCache)
        {
            var model = new PersistedModel(application.Id);
            foreach (var designer in application.GetDesigners())
            {
                var packages = designer.GetPackages(false, false).ToList();
                foreach (var package in packages)
                {
                    model.Index(package, designer);
                }

                if (!designerNames.TryGetValue(designer.Id, out var name))
                {
                    continue;
                }

                var owned = packages.Where(x => string.Equals(x.ApplicationId, application.Id, StringComparison.OrdinalIgnoreCase)).Select(model.Package).ToList();
                var referenced = packages.Where(x => !string.Equals(x.ApplicationId, application.Id, StringComparison.OrdinalIgnoreCase)).Select(model.Package).ToList();
                if (owned.Count > 0)
                {
                    model.Designers[name] = owned;
                }

                if (referenced.Count > 0)
                {
                    model.References[name] = referenced;
                }
            }

            // A type reference may point into a module's package, such as Intent.Common.Types' primitives, which a designer or a
            // package references; a stereotype's definition, with its properties' defaults, is in one too.
            var references = application.GetDesigners().SelectMany(x => x.PackageReferences ?? new List<PackageReferenceModel>())
                .Concat(model.Packages.ToList().SelectMany(x => x.Package.References ?? new List<PackageReferenceModel>()));
            var installed = application.Modules.GroupBy(x => x.ModuleId, StringComparer.OrdinalIgnoreCase).ToDictionary(x => x.Key, x => x.First().Version, StringComparer.OrdinalIgnoreCase);
            foreach (var reference in references)
            {
                if (reference.PackageId != null && !model._packageIds.Contains(reference.PackageId)
                    && (TryLoad(reference, application.Id) ?? FromCache(reference, moduleCache, installed)) is { } loaded)
                {
                    model.Index(loaded, null);
                }
            }

            model.Link();
            return model;
        }

        /// <summary>The persistence element of an id, or null.</summary>
        public ElementPersistable Element(string id) => id != null && _elements.TryGetValue(id, out var element) ? element : null;

        /// <summary>The package an element sits in.</summary>
        public PackageModelPersistable PackageOf(ElementPersistable element) => _placement.TryGetValue(element, out var place) ? place.Package : null;

        /// <summary>An association end of an id (a target end's is its association's), with its association and package, or null.</summary>
        public (AssociationEndPersistable End, AssociationPersistable Association, PackageModelPersistable Package)? End(string id) =>
            id != null && _ends.TryGetValue(id, out var end) ? end : null;

        /// <summary>The id of the element or end an association starts from.</summary>
        public string SourceOf(AssociationPersistable association) => _sources.TryGetValue(association, out var source) ? source : null;

        private static PackageModelPersistable TryLoad(PackageReferenceModel reference, string applicationId)
        {
            try
            {
                return reference.TryLoadPackage(applicationId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// A module's package read from the solution's module cache, for when the host doesn't answer Intent's request for it
        /// (<c>TryLoadPackage</c> asks the client): the installed version's folder, else any of the module's. A package that won't
        /// load leaves its types as named stand-ins (<see cref="Stub"/>).
        /// </summary>
        private static PackageModelPersistable FromCache(PackageReferenceModel reference, string moduleCache, IReadOnlyDictionary<string, string> installed)
        {
            if (string.IsNullOrWhiteSpace(reference.Module) || moduleCache == null || !System.IO.Directory.Exists(moduleCache))
            {
                return null;
            }

            var folders = System.IO.Directory.GetDirectories(moduleCache, reference.Module + ".*")
                .OrderByDescending(x => installed.TryGetValue(reference.Module, out var version) && System.IO.Path.GetFileName(x) == $"{reference.Module}.{version}")
                .ToList();
            foreach (var file in folders.SelectMany(x => System.IO.Directory.GetFiles(x, "*.pkg.config", System.IO.SearchOption.AllDirectories)))
            {
                try
                {
                    if (System.Xml.Linq.XDocument.Load(file).Root?.Attribute("id")?.Value == reference.PackageId)
                    {
                        return PackageModelPersistable.Load(file);
                    }
                }
                catch (Exception)
                {
                    // An unreadable package file is skipped.
                }
            }

            return null;
        }

        private void Index(PackageModelPersistable package, ApplicationDesignerPersistable designer)
        {
            if (package?.Id == null || !_packageIds.Add(package.Id))
            {
                return;
            }

            Packages.Add((package, designer));
            foreach (var definition in package.StereotypeDefinitions ?? new List<StereotypeDefinitionPersistable>())
            {
                if (definition?.Id != null)
                {
                    _definitions.TryAdd(definition.Id, definition);
                }
            }

            // A package lists its elements' files flat, each naming the folder it is in (the package's own id at its root), and
            // Intent hands a folder's elements over as its children.
            var files = package.ChildElements ?? new List<ElementPersistable>();
            var folders = files.Where(x => x?.Id != null).GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
            var roots = _roots[package] = new List<ElementPersistable>();
            foreach (var element in files.Where(x => x?.Id != null))
            {
                if (element.ParentFolderId != null && element.ParentFolderId != package.Id && element.ParentFolderId != element.Id
                    && folders.TryGetValue(element.ParentFolderId, out var folder))
                {
                    (_inFolder.TryGetValue(folder.Id, out var members) ? members : _inFolder[folder.Id] = new List<ElementPersistable>()).Add(element);
                    Index(element, package, folder);
                }
                else
                {
                    roots.Add(element);
                    Index(element, package, null);
                }
            }

            foreach (var (association, source) in GuardrailsModelCleanup.AssociationSources(package))
            {
                _sources[association] = source;
                if (association.SourceEnd?.Id != null)
                {
                    _ends[association.SourceEnd.Id] = (association.SourceEnd, association, package);
                }

                if (association.TargetEnd?.Id != null)
                {
                    _ends[association.TargetEnd.Id] = (association.TargetEnd, association, package);
                }
            }
        }

        private void Index(ElementPersistable element, PackageModelPersistable package, ElementPersistable parent)
        {
            if (element?.Id == null)
            {
                return;
            }

            _elements[element.Id] = element;
            _placement[element] = (package, parent);
            foreach (var child in element.ChildElements ?? new List<ElementPersistable>())
            {
                Index(child, package, element);
            }
        }

        /// <summary>
        /// Each element's and end's associated elements: the far end of every association it is at one end of, which is the
        /// end Intent hands over as a navigation from it.
        /// </summary>
        private void Link()
        {
            foreach (var (association, source) in _sources)
            {
                if (association.SourceEnd == null || association.TargetEnd == null)
                {
                    continue;
                }

                if (source != null)
                {
                    Associated(source).Add(association.TargetEnd);
                }

                if (association.TargetEnd.TypeReference?.TypeId is { Length: > 0 } target)
                {
                    Associated(target).Add(association.SourceEnd);
                }
            }
        }

        private List<AssociationEndPersistable> Associated(string id) =>
            _associated.TryGetValue(id, out var ends) ? ends : _associated[id] = new List<AssociationEndPersistable>();

        // ---- Proxies ----

        private IPackage Package(PackageModelPersistable package) => Proxy<IPackage>(package, member => member switch
        {
            "Id" => package.Id,
            "Name" or "DisplayText" => package.Name,
            "SpecializationType" => package.SpecializationType,
            "SpecializationTypeId" => package.SpecializationTypeId,
            "Comment" => package.Comment,
            "ApplicationId" => package.ApplicationId,
            "FileLocation" => package.AbsolutePath,
            "Stereotypes" => Stereotypes(package.Stereotypes),
            "ChildElements" => _roots.GetValueOrDefault(package, new List<ElementPersistable>()).Select(Element).ToList(),
            "Package" => Package(package),
            _ => Unset
        });

        private IElement Element(ElementPersistable element) => Proxy<IElement>(element, member =>
        {
            var (package, parent) = _placement.GetValueOrDefault(element);
            return member switch
            {
                "Id" => element.Id,
                "Name" => element.Name,
                "DisplayText" => element.Display ?? element.Name,
                "SpecializationType" => element.SpecializationType,
                "SpecializationTypeId" => element.SpecializationTypeId,
                "Comment" => element.Comment,
                "Value" => element.Value,
                "ExternalReference" => element.ExternalReference,
                "IsAbstract" => element.IsAbstract,
                "IsStatic" => element.IsStatic,
                "Order" => element.Order ?? 0,
                "IsChild" => parent != null,
                "ParentId" => parent?.Id,
                "ParentElement" => parent == null ? null : Element(parent),
                "Package" => package == null ? null : Package(package),
                "ChildElements" => (element.ChildElements ?? new List<ElementPersistable>()).Concat(_inFolder.GetValueOrDefault(element.Id, new List<ElementPersistable>()))
                    .Distinct(ReferenceEqualityComparer.Instance).Cast<ElementPersistable>().Select(Element).ToList(),
                "TypeReference" => element.TypeReference == null ? null : TypeReference(element.TypeReference, null),
                "GenericTypes" => (element.GenericTypes ?? new List<GenericType>()).Select(GenericType).ToList(),
                "Stereotypes" => Stereotypes(element.Stereotypes),
                "Metadata" => Metadata(element.Metadata?.Select(x => (x.Key, x.Value))),
                "AssociatedElements" => AssociatedOf(element.Id),
                "OwnedAssociations" => _sources.Where(x => x.Value == element.Id).Select(x => Association(x.Key)).ToList(),
                "GetParentPath" => Ancestors(element).Select(Element).ToList(),
                _ => Unset
            };
        });

        private IAssociationEnd End(AssociationEndPersistable end, AssociationPersistable association, PackageModelPersistable package) => Proxy<IAssociationEnd>(end, member =>
        {
            var isTarget = ReferenceEquals(association.TargetEnd, end);
            var other = isTarget ? association.SourceEnd : association.TargetEnd;
            return member switch
            {
                "Id" => end.Id,
                "Name" => end.Name,
                "DisplayText" => end.Display ?? end.Name,
                "SpecializationType" => end.SpecializationType,
                "SpecializationTypeId" => end.SpecializationTypeId,
                "Comment" => end.Comment,
                "Value" => end.Value,
                "ExternalReference" => end.ExternalReference,
                "Order" => end.Order,
                "Package" => Package(package),
                // A nested source end needn't name its element: it is the element the association sits under.
                "TypeReference" => end.TypeReference == null ? null : TypeReference(end.TypeReference, isTarget ? null : SourceOf(association)),
                "IsNavigable" => end.TypeReference?.IsNavigable ?? false,
                "Stereotypes" => Stereotypes(end.Stereotypes),
                "Metadata" => Metadata(end.Metadata?.Select(x => (x.Key, x.Value))),
                "AssociatedElements" => AssociatedOf(end.Id),
                "Association" => Association(association),
                "OtherEnd" => other == null ? null : End(other, association, package),
                "IsTargetEnd" => isTarget,
                "IsSourceEnd" => !isTarget,
                "ChildElements" => new List<IElement>(),
                _ => Unset
            };
        });

        private IAssociation Association(AssociationPersistable association) => Proxy<IAssociation>(association, member =>
        {
            var package = association.TargetEnd?.Id is { } id && _ends.TryGetValue(id, out var end) ? end.Package : null;
            return member switch
            {
                "Id" => association.Id,
                "SpecializationType" => association.AssociationType,
                "SpecializationTypeId" => association.AssociationTypeId,
                "Comment" => association.Comment,
                "SourceEnd" => association.SourceEnd == null ? null : End(association.SourceEnd, association, package),
                "TargetEnd" => association.TargetEnd == null ? null : End(association.TargetEnd, association, package),
                "IsSelfReference" => SourceOf(association) != null && SourceOf(association) == association.TargetEnd?.TypeReference?.TypeId,
                _ => Unset
            };
        });

        private ITypeReference TypeReference(TypeReferencePersistable reference, string elementId) => Proxy<ITypeReference>(reference, member => member switch
        {
            "ElementId" => elementId ?? reference.TypeId,
            "Element" => Referenced(elementId ?? reference.TypeId, reference.TypeName),
            "IsNullable" => reference.IsNullable,
            "IsCollection" => reference.IsCollection,
            "GenericTypeId" => reference.GenericTypeId,
            "GenericTypeParameters" => (reference.GenericTypeParameters ?? new List<TypeReferencePersistable>()).Select(x => TypeReference(x, null)).ToList(),
            "Stereotypes" => Stereotypes(reference.Stereotypes),
            _ => Unset
        });

        private IGenericType GenericType(GenericType type) => Proxy<IGenericType>(type, member => member switch
        {
            "Id" => type.Id,
            "Name" => type.Name,
            _ => Unset
        });

        private List<IStereotype> Stereotypes(IEnumerable<StereotypePersistable> stereotypes) =>
            (stereotypes ?? Enumerable.Empty<StereotypePersistable>()).Select(Stereotype).ToList();

        private IStereotype Stereotype(StereotypePersistable stereotype) => Proxy<IStereotype>(stereotype, (member, args) =>
        {
            var properties = (stereotype.Properties ?? new List<StereotypePropertyPersistable>()).Select(Property).ToList();
            switch (member)
            {
                case "Name":
                    return stereotype.Name;
                case "DefinitionId":
                    return stereotype.DefinitionId;
                case "Definition":
                    // Its properties' defaults: the engine counts only a property set away from its default as stated.
                    return stereotype.DefinitionId != null && _definitions.TryGetValue(stereotype.DefinitionId, out var definition) ? Definition(definition) : null;
                case "Properties":
                    return properties;
                case "GetProperty":
                    return properties.FirstOrDefault(x => x.Key == (string)args[0]);
                case "TryGetProperty":
                    args[1] = properties.FirstOrDefault(x => x.Key == (string)args[0]);
                    return args[1] != null;
                default:
                    return Unset;
            }
        });

        private IStereotypeDefinition Definition(StereotypeDefinitionPersistable definition) => Proxy<IStereotypeDefinition>(definition, member => member switch
        {
            "Id" => definition.Id,
            "Name" => definition.Name,
            "Properties" => (definition.Properties ?? new List<StereotypePropertyDefinitionPersistable>()).Select(PropertyDefinition).ToList(),
            _ => Unset
        });

        private IStereotypePropertyDefinition PropertyDefinition(StereotypePropertyDefinitionPersistable property) => Proxy<IStereotypePropertyDefinition>(property, member => member switch
        {
            "Id" => property.Id,
            "Name" => property.Name,
            "DefaultValue" => property.DefaultValue,
            _ => Unset
        });

        /// <summary>
        /// A property as Intent hands it over: an item list as its rows; one naming a model element as a lookup, its typed value
        /// the element; one naming several as a list of them; anything else as text.
        /// </summary>
        private IStereotypeProperty Property(StereotypePropertyPersistable property)
        {
            if (property.ItemList is { Count: > 0 } rows)
            {
                var items = rows.Select(Stereotype).ToArray();
                return Proxy<IStereotypeProperty<IStereotype[]>>(property, (member, _, typed) => typed && member == "Value" ? items : PropertyMember(property, member));
            }

            if (Lookup(property.Value) is { } element)
            {
                return Proxy<IStereotypeProperty<IElement>>(property, (member, _, typed) => typed && member == "Value" ? element : PropertyMember(property, member));
            }

            if (Lookups(property.Value) is { } elements)
            {
                return Proxy<IStereotypeProperty<IElement[]>>(property, (member, _, typed) => typed && member == "Value" ? elements : PropertyMember(property, member));
            }

            return Proxy<IStereotypeProperty>(property, member => PropertyMember(property, member));
        }

        private static object PropertyMember(StereotypePropertyPersistable property, string member) => member switch
        {
            "Key" => property.Name,
            "Value" => property.Value,
            "PropertyDefinitionId" => property.DefinitionId,
            _ => Unset
        };

        private IElement Lookup(string value) => value != null && _elements.TryGetValue(value, out var element) ? Element(element) : null;

        private IElement[] Lookups(string value)
        {
            if (value == null || !value.StartsWith("[", StringComparison.Ordinal))
            {
                return null;
            }

            try
            {
                var ids = JsonSerializer.Deserialize<string[]>(value);
                return ids is { Length: > 0 } && ids.All(x => x != null && _elements.ContainsKey(x)) ? ids.Select(x => Element(_elements[x])).ToArray() : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>What a type reference points at: an element, an association end, or a stand-in named as the reference names it.</summary>
        private ICanBeReferencedType Referenced(string id, string name)
        {
            if (id == null)
            {
                return null;
            }

            if (_elements.TryGetValue(id, out var element))
            {
                return Element(element);
            }

            if (_ends.TryGetValue(id, out var end))
            {
                return End(end.End, end.Association, end.Package);
            }

            return Stub(id, name);
        }

        /// <summary>
        /// A type no package read declares, such as a primitive of a module package that wouldn't load: a Type-Definition of the
        /// name the reference records, which is how the engine compares a primitive.
        /// </summary>
        private IElement Stub(string id, string name)
        {
            if (!_stubs.TryGetValue(id, out var stub))
            {
                stub = _stubs[id] = Proxy<IElement>(new object(), member => member switch
                {
                    "Id" => id,
                    "Name" or "DisplayText" => name,
                    "SpecializationType" => "Type-Definition",
                    _ => Unset
                });
            }

            return stub;
        }

        private List<IAssociationEnd> AssociatedOf(string id) =>
            _associated.TryGetValue(id, out var ends)
                ? ends.Where(x => x.Id != null && _ends.ContainsKey(x.Id)).Select(x => End(x, _ends[x.Id].Association, _ends[x.Id].Package)).ToList()
                : new List<IAssociationEnd>();

        private IEnumerable<ElementPersistable> Ancestors(ElementPersistable element)
        {
            var ancestors = new List<ElementPersistable>();
            for (var parent = _placement.GetValueOrDefault(element).Parent; parent != null; parent = _placement.GetValueOrDefault(parent).Parent)
            {
                ancestors.Insert(0, parent);
            }

            return ancestors;
        }

        private static Dictionary<string, string> Metadata(IEnumerable<(string Key, string Value)> entries)
        {
            var metadata = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var (key, value) in entries ?? Enumerable.Empty<(string, string)>())
            {
                if (key != null)
                {
                    metadata[key] = value;
                }
            }

            return metadata;
        }

        // ---- The proxy ----

        /// <summary>A member a proxy doesn't answer, which then gives its type's default.</summary>
        private static readonly object Unset = new();

        private T Proxy<T>(object source, Func<string, object> members) where T : class =>
            Proxy<T>(source, (member, _, _) => members(member));

        private T Proxy<T>(object source, Func<string, object[], object> members) where T : class =>
            Proxy<T>(source, (member, args, _) => members(member, args));

        /// <param name="members">A member's name (a property's without <c>get_</c>), its arguments, and whether it is declared
        /// on a generic interface, as <c>IStereotypeProperty&lt;T&gt;.Value</c> is beside <c>IStereotypeProperty.Value</c>.</param>
        private T Proxy<T>(object source, Func<string, object[], bool, object> members) where T : class
        {
            if (!_proxies.TryGetValue(source, out var byType))
            {
                byType = _proxies[source] = new Dictionary<Type, object>();
            }

            if (byType.TryGetValue(typeof(T), out var existing))
            {
                return (T)existing;
            }

            var proxy = DispatchProxy.Create<T, ModelProxy>();
            ((ModelProxy)(object)proxy).Members = members;
            byType[typeof(T)] = proxy;
            return proxy;
        }

        /// <summary>The proxy every interface is implemented by; see <see cref="PersistedModel"/>.</summary>
        internal class ModelProxy : DispatchProxy
        {
            internal Func<string, object[], bool, object> Members { get; set; }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                if (targetMethod == null)
                {
                    return null;
                }

                if (targetMethod.Name == nameof(Equals) && args?.Length == 1)
                {
                    return ReferenceEquals(this, args[0]);
                }

                var name = targetMethod.IsSpecialName && targetMethod.Name.StartsWith("get_", StringComparison.Ordinal) ? targetMethod.Name[4..] : targetMethod.Name;
                var value = Members(name, args ?? Array.Empty<object>(), targetMethod.DeclaringType is { IsGenericType: true });
                return ReferenceEquals(value, Unset) ? Default(targetMethod.ReturnType) : value;
            }

            /// <summary>A type's default: an empty list for any sequence, so an unread member never fails the engine with a null.</summary>
            private static object Default(Type type)
            {
                if (type == typeof(void) || type == typeof(string))
                {
                    return null;
                }

                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }

                if (type.IsArray)
                {
                    return Array.CreateInstance(type.GetElementType()!, 0);
                }

                if (type.IsGenericType && type.GetGenericArguments() is { Length: 2 } pair && type.IsAssignableFrom(typeof(Dictionary<,>).MakeGenericType(pair)))
                {
                    return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(pair));
                }

                if (type.IsGenericType && type.GetGenericArguments() is { Length: 1 } item && type.IsAssignableFrom(typeof(List<>).MakeGenericType(item)))
                {
                    return Activator.CreateInstance(typeof(List<>).MakeGenericType(item));
                }

                return null;
            }
        }
    }
}
