using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.HotChocolate.GraphQL.Dispatch.MediatR.Api
{
    public static class QueryModelStereotypeExtensions
    {
        public static GraphQLEnabled GetGraphQLEnabled(this QueryModel model)
        {
            var stereotype = model.GetStereotype("ea187e1a-7df2-48d1-b523-4720a4b69b5b");
            return stereotype != null ? new GraphQLEnabled(stereotype) : null;
        }


        public static bool HasGraphQLEnabled(this QueryModel model)
        {
            return model.HasStereotype("ea187e1a-7df2-48d1-b523-4720a4b69b5b");
        }

        public static bool TryGetGraphQLEnabled(this QueryModel model, out GraphQLEnabled stereotype)
        {
            if (!HasGraphQLEnabled(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new GraphQLEnabled(model.GetStereotype("ea187e1a-7df2-48d1-b523-4720a4b69b5b"));
            return true;
        }

        public class GraphQLEnabled
        {
            private IStereotype _stereotype;

            public GraphQLEnabled(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}