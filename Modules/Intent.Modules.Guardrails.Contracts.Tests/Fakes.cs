using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using NSubstitute;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>Designer metadata built from NSubstitute fakes, for the model readers.</summary>
    internal static class Fakes
    {
        public const string ApplicationId = "app";

        public static IStereotype Stereotype(string name, params (string Key, string Value)[] properties)
        {
            var stereotype = Substitute.For<IStereotype>();
            stereotype.Name.Returns(name);
            var values = properties.ToDictionary(x => x.Key, x =>
            {
                var property = Substitute.For<IStereotypeProperty>();
                property.Key.Returns(x.Key);
                property.Value.Returns(x.Value);
                return property;
            });
            stereotype.Properties.Returns(values.Values.ToList());
            stereotype.TryGetProperty(Arg.Any<string>(), out Arg.Any<IStereotypeProperty>()).Returns(call =>
            {
                var found = values.TryGetValue((string)call[0], out var property);
                call[1] = property;
                return found;
            });
            return stereotype;
        }

        public static IPackage Package(string applicationId = ApplicationId, params IStereotype[] stereotypes)
        {
            var package = Substitute.For<IPackage>();
            package.ApplicationId.Returns(applicationId);
            package.Stereotypes.Returns(stereotypes.ToList());
            package.ChildElements.Returns(new List<IElement>());
            return package;
        }

        /// <summary>An element added to its parent (an element or a package).</summary>
        public static IElement Element(object parent, string specialization, string name, params IStereotype[] stereotypes)
        {
            var element = Substitute.For<IElement>();
            element.Id.Returns($"{specialization}:{name}");
            element.Name.Returns(name);
            element.SpecializationType.Returns(specialization);
            element.Stereotypes.Returns(stereotypes.ToList());
            element.ChildElements.Returns(new List<IElement>());

            // NSubstitute would otherwise answer with a new fake, so walking up the parents would never end.
            element.ParentElement.Returns((IElement)null);
            element.TypeReference.Returns((ITypeReference)null);
            switch (parent)
            {
                case IElement parentElement:
                    var package = parentElement.Package;
                    var siblings = (List<IElement>)parentElement.ChildElements;
                    element.ParentElement.Returns(parentElement);
                    element.Package.Returns(package);
                    siblings.Add(element);
                    break;
                case IPackage parentPackage:
                    var children = (List<IElement>)parentPackage.ChildElements;
                    element.Package.Returns(parentPackage);
                    children.Add(element);
                    break;
            }

            return element;
        }

        public static IElement Typed(this IElement element, string typeName, bool nullable = false, bool collection = false)
        {
            var type = Type(typeName, nullable, collection);
            element.TypeReference.Returns(type);
            return element;
        }

        public static ITypeReference Type(string typeName, bool nullable = false, bool collection = false, string id = null)
        {
            var type = Substitute.For<ICanBeReferencedType>();
            type.Name.Returns(typeName);
            type.Id.Returns(id ?? $"type:{typeName}");
            var reference = Substitute.For<ITypeReference>();
            reference.Element.Returns(type);
            reference.IsNullable.Returns(nullable);
            reference.IsCollection.Returns(collection);
            return reference;
        }

        public static IAssociation Association(string id, IElement source, string sourceEnd, bool sourceCollection, IElement target, string targetEnd, bool targetNullable)
        {
            var association = Substitute.For<IAssociation>();
            association.Id.Returns(id);
            var sourceEndElement = Substitute.For<IAssociationEnd>();
            sourceEndElement.Id.Returns($"{id}:source");
            sourceEndElement.Name.Returns(sourceEnd);
            var sourceType = Type(source.Name, collection: sourceCollection, id: source.Id);
            sourceEndElement.TypeReference.Returns(sourceType);
            var targetEndElement = Substitute.For<IAssociationEnd>();
            targetEndElement.Id.Returns($"{id}:target");
            targetEndElement.Name.Returns(targetEnd);
            var targetType = Type(target.Name, nullable: targetNullable, id: target.Id);
            targetEndElement.TypeReference.Returns(targetType);
            association.SourceEnd.Returns(sourceEndElement);
            association.TargetEnd.Returns(targetEndElement);
            return association;
        }

        public static IDesigner Designer(IEnumerable<IPackage> packages, IEnumerable<IAssociation> associations = null)
        {
            var designer = Substitute.For<IDesigner>();
            designer.Packages.Returns(packages.ToList());
            designer.Associations.Returns((associations ?? Enumerable.Empty<IAssociation>()).ToList());
            return designer;
        }
    }
}
