using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Dapper.Templates
{
	public static class Extensions
	{

		public static IList<AttributeModel> GetPks(this ClassModel entity)
		{
			while (entity != null)
			{
				var primaryKeys = entity.Attributes.Where(x => x.HasStereotype("Primary Key")).ToList();
				if (!primaryKeys.Any())
				{
					entity = entity.ParentClass;
					continue;
				}

				return primaryKeys;
			}
			return new List<AttributeModel>();
		}

		public static string SqlTableName(this ClassModel model)
		{
			if (string.IsNullOrEmpty( model.FindSchema()))
			{
				return $"[{model.GetTableName()}]";
			}
			return $"[{ model.FindSchema()}].[{ model.GetTableName()}]";
		}

		public static string ColumnName(this AttributeModel model)
		{
			if (model.HasStereotype("Column") && !string.IsNullOrEmpty(model.GetStereotypeProperty<string>("Column", "Name")?.Trim()))
			{
				return model.GetStereotypeProperty<string>("Column", "Name")?.Trim();
			}
			return model.Name;
		}

		public static string FindSchema(this ClassModel classModel)
		{
			IHasStereotypes currentElement = classModel.InternalElement;

			if (currentElement.HasStereotype("Table") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("Table", "Schema")?.Trim()))
			{
				return currentElement.GetStereotypeProperty<string>("Table", "Schema")?.Trim();
			}

			if (currentElement.HasStereotype("View") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("View", "Schema")?.Trim()))
			{
				return currentElement.GetStereotypeProperty<string>("View", "Schema")?.Trim();
			}

			while (currentElement != null)
			{
				if (currentElement.HasStereotype("Schema"))
				{
					return currentElement.GetStereotypeProperty<string>("Schema", "Name")?.Trim();
				}

				if (currentElement is not IElement element)
				{
					break;
				}

				currentElement = element.ParentElement ?? (IHasStereotypes)element.Package;
			}
			return null;
		}
		public static string GetTableName(this ClassModel classModel)
		{
			IHasStereotypes currentElement = classModel.InternalElement;

			if (currentElement.HasStereotype("Table") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("Table", "Name")?.Trim()))
			{
				return currentElement.GetStereotypeProperty<string>("Table", "Name")?.Trim();
			}
			return classModel.Name;
		}
	}
}
