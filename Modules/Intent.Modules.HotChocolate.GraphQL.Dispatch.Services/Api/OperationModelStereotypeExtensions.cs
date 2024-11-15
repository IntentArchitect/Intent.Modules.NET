using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.HotChocolate.GraphQL.Dispatch.Services.Api
{
    public static class OperationModelStereotypeExtensions
    {
        public static GraphQLEnabled GetGraphQLEnabled(this OperationModel model)
        {
            var stereotype = model.GetStereotype(GraphQLEnabled.DefinitionId);
            return stereotype != null ? new GraphQLEnabled(stereotype) : null;
        }


        public static bool HasGraphQLEnabled(this OperationModel model)
        {
            return model.HasStereotype(GraphQLEnabled.DefinitionId);
        }

        public static bool TryGetGraphQLEnabled(this OperationModel model, out GraphQLEnabled stereotype)
        {
            if (!HasGraphQLEnabled(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new GraphQLEnabled(model.GetStereotype(GraphQLEnabled.DefinitionId));
            return true;
        }

        public class GraphQLEnabled
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b7dc433a-8d6d-4cae-88e3-780e5aa0c418";

            public GraphQLEnabled(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}