using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.CosmosDb.Api
{
	[IntentManaged(Mode.Fully, Signature = Mode.Fully)]
	public class CosmosDBDiagramModel : IMetadataModel, IHasStereotypes, IHasName
	{
		public const string SpecializationType = "CosmosDB Diagram";
		public const string SpecializationTypeId = "B5D07203-4667-4BCF-ADB4-04792C416784"; // i used the newguid tool in vs to generate this, not sure if it is bound to an already existing intent id
		protected readonly IElement _element;

		[IntentManaged(Mode.Fully)]
		public CosmosDBDiagramModel(IElement element, string requiredType = SpecializationType)
		{
			if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
			}
			_element = element;
		}

		public string Id => _element.Id;

		public string Name => _element.Name;

		public string Comment => _element.Comment;

		public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

		public IElement InternalElement => _element;

		public override string ToString()
		{
			return _element.ToString();
		}

		public bool Equals(CosmosDBDiagramModel other)
		{
			return Equals(_element, other?._element);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((CosmosDBDiagramModel)obj);
		}

		public override int GetHashCode()
		{
			return _element != null ? _element.GetHashCode() : 0;
		}
	}

	[IntentManaged(Mode.Fully)]
	public static class CosmosDBDiagramModelExtensions
	{

		public static bool IsCosmosDBDiagramModel(this ICanBeReferencedType type)
		{
			return type != null && type is IElement element && element.SpecializationTypeId == CosmosDBDiagramModel.SpecializationTypeId;
		}

		public static CosmosDBDiagramModel AsCosmosDBDiagramModel(this ICanBeReferencedType type)
		{
			return type.IsCosmosDBDiagramModel() ? new CosmosDBDiagramModel((IElement)type) : null;
		}
	}
}