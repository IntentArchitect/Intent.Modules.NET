using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Integration.HttpClients.Shared;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Utils;

namespace Intent.Modules.Integration.HttpClients.InteractionStrategies
{
    internal class CallImplicitServiceProxyInteractionStrategy : IInteractionStrategy
    {
        private readonly IApplication _application;

        public CallImplicitServiceProxyInteractionStrategy(IApplication application)
        {
            _application = application;
        }

        public bool IsMatch(IElement interaction)
        {
            if (!IsPerformInvocationTargetEndModel(interaction))
            {
                return false;
            }

            var element = interaction.TypeReference.Element as IElement;
            var isMatch = element != null &&
                          element.HasHttpSettings() &&
                          element.Package.ApplicationId != _application.Id; // There seems to be a bug in v4.5.0-beta.2 where the element's application ID is wrong

            return isMatch;

            // TODO: JL Copied for now so no reference needs to be added 
            static bool IsPerformInvocationTargetEndModel(ICanBeReferencedType type)
            {
                return type is IAssociationEnd { SpecializationTypeId: "093e5909-ffe4-4510-b3ea-532f30212f3c" };
            }
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interaction)
        {
            var element = (IElement)interaction.TypeReference.Element;


            var handlerClass = method.Class;
            var template = (ICSharpFileBuilderTemplate)handlerClass.File.Template;

            var templates = template.ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.ClientInterface);

            // We sometimes add "implicit" service proxy operations to existing service proxies if
            // they exist, in such cases the id of the parent will be the folder where the
            // original command is while the id of the template model will be that of the service
            // proxy.

            Debugger.Launch();
            var template1 = templates
                .FirstOrDefault(x =>
                {
                    if (!x.TryGetModel(out IServiceProxyModel serviceProxyModel))
                    {
                        return false;
                    }

                    // TODO JL:
                    return false;
                    //return serviceProxyModel.GetEndpoints().Any(y => y.Id == element.Id);
                });

            //var spt = template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Services.ClientInterface, element.ParentElement);
            //method.AddStatement($"// test {(spt == null)}");
        }
    }
}
