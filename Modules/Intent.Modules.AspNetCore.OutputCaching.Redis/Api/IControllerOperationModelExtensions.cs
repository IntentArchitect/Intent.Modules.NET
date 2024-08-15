using Intent.AspNetCore.OutputCaching.Redis.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.AspNetCore.OutputCaching.Redis.Api.FolderModelStereotypeExtensions;

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.Api
{
	internal static class IControllerOperationModelExtensions
	{
		private static IEnumerable<Caching> FindCaching(IElement endpoint)
		{
			IHasStereotypes currentElement = endpoint;

			while (currentElement != null)
			{
				if (currentElement.HasCaching())
				{
					yield return currentElement.GetCaching();
				}

				if (currentElement is not IElement element)
				{
					break;
				}

				currentElement = element.ParentElement ?? (IHasStereotypes)element.Package;
			}
		}

		private static Caching GetCaching(this IHasStereotypes model)
		{
			var stereotype = model.GetStereotype("c090804e-8e1c-4121-8209-488982ce6a22");
			return new Caching(stereotype);
		}


		private static bool HasCaching(this IHasStereotypes model)
		{
			return model.HasStereotype("c090804e-8e1c-4121-8209-488982ce6a22");
		}

		internal static bool TryGetCaching(this IElement operation, out CachingAggregate? caching)
		{
			caching = null;

			var cachingConfigs = FindCaching(operation);
			if (cachingConfigs.Any())
			{
				if (cachingConfigs.First().NoCaching())
				{
					return false;
				}
				else
				{
					var result = new CachingAggregate();
					//This is cascading system where the value set closest to the Endpoint wins.
					foreach (var cachingConfig in cachingConfigs)
					{
						result.Policy = cachingConfig.Policy()?.Name;
						result.Tags = cachingConfig.Tags();
						if (cachingConfig.Override())
						{
							result.Duration = cachingConfig.Duration();
							result.VaryByRouteValueNames = cachingConfig.VaryByRouteValueNames();
							result.VaryByHeaderNames = cachingConfig.VaryByHeaderNames();
							result.VaryByQueryKeys = cachingConfig.VaryByQueryKeys();
							result.NoCaching = cachingConfig.NoCaching();
						}
						if (cachingConfig.Policy() != null)
						{
							var policyCaching = GetCaching(cachingConfig.Policy());
							result.Duration = policyCaching.Duration();
							result.Tags = policyCaching.Tags();
							result.VaryByRouteValueNames = policyCaching.VaryByRouteValueNames();
							result.VaryByHeaderNames = policyCaching.VaryByHeaderNames();
							result.VaryByQueryKeys = policyCaching.VaryByQueryKeys();
							result.NoCaching = policyCaching.NoCaching();
						}
					}
					caching = result;
				}
				if (caching.NoCaching == true)
					return false;
				return true;
			}

			return false;
		}

		public class CachingAggregate
		{
			private string? _policy;
			private int? _duration;
			private string? _tags;
			private string? _varyByQueryKeys;
			private string? _varyByHeaderNames;
			private string? _varyByRouteValueNames;
			private bool? _noCaching;

			public CachingAggregate()
			{
			}

			public string? Policy
			{
				get => _policy; 
				set
				{
					if (_policy == null)
					{
						_policy = value;
					}
				}
			}

			public int? Duration
			{
				get => _duration;
				set
				{
					if (_duration == null)
					{
						_duration = value;
					}
				}
			}

			public string? Tags
			{
				get => _tags;
				set
				{
					if (_tags == null)
					{
						_tags = value;
					}
				}
			}

			public string? VaryByQueryKeys
			{
				get => _varyByQueryKeys;
				set
				{
					if (_varyByQueryKeys == null)
					{
						_varyByQueryKeys = value;
					}
				}
			}

			public string? VaryByHeaderNames
			{
				get => _varyByHeaderNames;
				set
				{
					if (_varyByHeaderNames == null)
					{
						_varyByHeaderNames = value;
					}
				}
			}

			public string? VaryByRouteValueNames
			{
				get => _varyByRouteValueNames;
				set
				{
					if (_varyByRouteValueNames == null)
					{
						_varyByRouteValueNames = value;
					}
				}
			}

			public bool? NoCaching
			{
				get => _noCaching;
				set
				{
					if (_noCaching == null)
					{
						_noCaching = value;
					}
				}
			}
		}
	}
}
