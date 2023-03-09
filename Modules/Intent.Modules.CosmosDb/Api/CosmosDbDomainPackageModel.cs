using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Api
{
	[IntentManaged(Mode.Fully)]
	public class CosmosDbDomainPackageModel : IHasStereotypes, IMetadataModel
	{
		public const string SpecializationType = "CosmosDb Domain Package";
		public const string SpecializationTypeId = "77015F6A-F90F-4B42-B60F-ED8140DABD35"; // i used the newguid tool in vs to generate this, not sure if it is bound to an already existing intent id

		[IntentManaged(Mode.Ignore)]
		public CosmosDbDomainPackageModel(IPackage package)
		{
			if (!SpecializationType.Equals(package.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
			}

			UnderlyingPackage = package;
		}

		public IPackage UnderlyingPackage { get; }
		public string Id => UnderlyingPackage.Id;
		public string Name => UnderlyingPackage.Name;
		public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;
		public string FileLocation => UnderlyingPackage.FileLocation;

		public IList<TypeDefinitionModel> Types => UnderlyingPackage.ChildElements
			.GetElementsOfType(TypeDefinitionModel.SpecializationTypeId)
			.Select(x => new TypeDefinitionModel(x))
			.ToList();

		public IList<EnumModel> Enums => UnderlyingPackage.ChildElements
			.GetElementsOfType(EnumModel.SpecializationTypeId)
			.Select(x => new EnumModel(x))
			.ToList();

		public IList<CommentModel> Comments => UnderlyingPackage.ChildElements
			.GetElementsOfType(CommentModel.SpecializationTypeId)
			.Select(x => new CommentModel(x))
			.ToList();

		public IList<CosmosDBDiagramModel> Diagrams => UnderlyingPackage.ChildElements
			.GetElementsOfType(CosmosDBDiagramModel.SpecializationTypeId)
			.Select(x => new CosmosDBDiagramModel(x))
			.ToList();

		public IList<FolderModel> Folders => UnderlyingPackage.ChildElements
			.GetElementsOfType(FolderModel.SpecializationTypeId)
			.Select(x => new FolderModel(x))
			.ToList();

		public IList<ClassModel> Classes => UnderlyingPackage.ChildElements
			.GetElementsOfType(ClassModel.SpecializationTypeId)
			.Select(x => new ClassModel(x))
			.ToList();

	}
}